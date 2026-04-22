using HslCommunication;
using System;

namespace MASAN_SERIALIZATION.Helpers
{
    public enum e_CameraSubSyncV2_Result
    {
        Pass,
        Timeout,
        Reject,
        SyncError,
        NoResponse,
        ReadError
    }

    public class CameraSubSyncV2Result
    {
        public e_CameraSubSyncV2_Result Result { get; set; }
        public int PreviousId { get; set; }
        public int PreviousStatus { get; set; }
        public int ExpectedId { get; set; }
        public int CurrentId { get; set; }
        public int PlcStatus { get; set; }
        public int PollCount { get; set; }
        public double ElapsedMs { get; set; }
        public string Message { get; set; }
    }

    public static class CameraSubPlcSyncV2Helper
    {
        // Overload hỗ trợ các call-site dùng ushort cho length (phổ biến với API PLC)
        public static CameraSubSyncV2Result WaitAndResolve(
            Func<string, ushort, OperateResult<int[]>> readInt32,
            string currentIdAddress,
            string currentStatusAddress,
            string historyIdStartAddress,
            string historyStatusStartAddress,
            int timeoutMs,
            int pollingIntervalMs,
            Func<int, bool> isPassStatus,
            Func<int, bool> isTimeoutStatus,
            int? previousIdSnapshot = null,
            int? previousStatusSnapshot = null)
        {
            Func<string, int, OperateResult<int[]>> adapter = (address, length) => readInt32(address, (ushort)length);

            return WaitAndResolve(
                adapter,
                currentIdAddress,
                currentStatusAddress,
                historyIdStartAddress,
                historyStatusStartAddress,
                timeoutMs,
                pollingIntervalMs,
                isPassStatus,
                isTimeoutStatus,
                previousIdSnapshot,
                previousStatusSnapshot);
        }

        public static CameraSubSyncV2Result WaitAndResolve(
            Func<string, int, OperateResult<int[]>> readInt32,
            string currentIdAddress,
            string currentStatusAddress,
            string historyIdStartAddress,
            string historyStatusStartAddress,
            int timeoutMs,
            int pollingIntervalMs,
            Func<int, bool> isPassStatus,
            Func<int, bool> isTimeoutStatus,
            int? previousIdSnapshot = null,
            int? previousStatusSnapshot = null)
        {
            var startTime = DateTime.Now;
            int maxPolls = Math.Max(1, timeoutMs / Math.Max(1, pollingIntervalMs));

            int previousId;
            int previousStatus;
            if (previousIdSnapshot.HasValue && previousStatusSnapshot.HasValue)
            {
                previousId = previousIdSnapshot.Value;
                previousStatus = previousStatusSnapshot.Value;
            }
            else
            {
                OperateResult<int[]> readBeforeId = readInt32(currentIdAddress, 1);
                OperateResult<int[]> readBeforeStatus = readInt32(currentStatusAddress, 1);
                if (!readBeforeId.IsSuccess || !readBeforeStatus.IsSuccess ||
                    readBeforeId.Content == null || readBeforeStatus.Content == null ||
                    readBeforeId.Content.Length == 0 || readBeforeStatus.Content.Length == 0)
                {
                    return new CameraSubSyncV2Result
                    {
                        Result = e_CameraSubSyncV2_Result.ReadError,
                        Message = $"Không đọc được ID/Status trước khi gửi lane: IDMsg={readBeforeId.Message}, StatusMsg={readBeforeStatus.Message}"
                    };
                }

                previousId = readBeforeId.Content[0];
                previousStatus = readBeforeStatus.Content[0];
            }

            // Theo nguyên lý mới của PLC:
            // - CurrentID đang là ID của chai hiện tại đang xử lý (không phải chai trước đó)
            // - Status ban đầu = 5, sau khi PC gửi lane thì PLC mới đổi status kết quả
            // => V2 cần theo dõi status thay đổi trên chính ID snapshot hiện tại.
            int expectedId = previousId;
            int pollCount = 0;

            while (pollCount < maxPolls)
            {
                pollCount++;

                OperateResult<int[]> readCurrentId = readInt32(currentIdAddress, 1);
                OperateResult<int[]> readCurrentStatus = readInt32(currentStatusAddress, 1);
                if (readCurrentId.IsSuccess && readCurrentStatus.IsSuccess &&
                    readCurrentId.Content != null && readCurrentStatus.Content != null &&
                    readCurrentId.Content.Length >= 1 && readCurrentStatus.Content.Length >= 1)
                {
                    int currentId = readCurrentId.Content[0];
                    int currentStatus = readCurrentStatus.Content[0];

                    if (currentId == expectedId && currentStatus == previousStatus)
                    {
                        System.Threading.Thread.Sleep(pollingIntervalMs);
                        continue;
                    }

                    if (currentId == expectedId)
                    {
                        return BuildResult(previousId, previousStatus, expectedId, currentId, currentStatus, pollCount, startTime, isPassStatus, isTimeoutStatus);
                    }

                    if (currentId > expectedId)
                    {
                        return new CameraSubSyncV2Result
                        {
                            Result = e_CameraSubSyncV2_Result.NoResponse,
                            PreviousId = previousId,
                            PreviousStatus = previousStatus,
                            ExpectedId = expectedId,
                            CurrentId = currentId,
                            PlcStatus = currentStatus,
                            PollCount = pollCount,
                            ElapsedMs = (DateTime.Now - startTime).TotalMilliseconds,
                            Message = $"ID đã chuyển sang chai khác ({currentId}) trước khi status của ID {expectedId} đổi từ {previousStatus}"
                        };
                    }
                }

                System.Threading.Thread.Sleep(pollingIntervalMs);
            }

            return new CameraSubSyncV2Result
            {
                Result = e_CameraSubSyncV2_Result.NoResponse,
                PreviousId = previousId,
                PreviousStatus = previousStatus,
                ExpectedId = expectedId,
                PollCount = pollCount,
                ElapsedMs = (DateTime.Now - startTime).TotalMilliseconds,
                Message = $"Hết timeout {timeoutMs}ms nhưng status của ID {expectedId} chưa đổi từ {previousStatus}"
            };
        }

        private static CameraSubSyncV2Result BuildResult(
            int previousId,
            int previousStatus,
            int expectedId,
            int currentId,
            int plcStatus,
            int pollCount,
            DateTime startTime,
            Func<int, bool> isPassStatus,
            Func<int, bool> isTimeoutStatus)
        {
            e_CameraSubSyncV2_Result result;

            if (isPassStatus(plcStatus))
            {
                result = e_CameraSubSyncV2_Result.Pass;
            }
            else if (isTimeoutStatus(plcStatus))
            {
                result = e_CameraSubSyncV2_Result.Timeout;
            }
            else
            {
                result = e_CameraSubSyncV2_Result.Reject;
            }

            return new CameraSubSyncV2Result
            {
                Result = result,
                PreviousId = previousId,
                PreviousStatus = previousStatus,
                ExpectedId = expectedId,
                CurrentId = currentId,
                PlcStatus = plcStatus,
                PollCount = pollCount,
                ElapsedMs = (DateTime.Now - startTime).TotalMilliseconds,
                Message = $"Resolve theo ID: Prev={previousId}, PrevStatus={previousStatus}, Expected={expectedId}, Current={currentId}, Status={plcStatus}, Result={result}"
            };
        }
    }
}
