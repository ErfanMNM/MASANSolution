using HslCommunication;
using System;
using System.Collections.Generic;

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
            Func<int, bool> isTimeoutStatus)
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
                isTimeoutStatus);
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
            Func<int, bool> isTimeoutStatus)
        {
            var startTime = DateTime.Now;
            int maxPolls = Math.Max(1, timeoutMs / Math.Max(1, pollingIntervalMs));

            OperateResult<int[]> readBefore = readInt32(currentIdAddress, 1);
            if (!readBefore.IsSuccess || readBefore.Content == null || readBefore.Content.Length == 0)
            {
                return new CameraSubSyncV2Result
                {
                    Result = e_CameraSubSyncV2_Result.ReadError,
                    Message = $"Không đọc được ID trước khi gửi lane: {readBefore.Message}"
                };
            }

            int previousId = readBefore.Content[0];
            int expectedId = previousId + 1;
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

                    if (currentId == previousId)
                    {
                        System.Threading.Thread.Sleep(pollingIntervalMs);
                        continue;
                    }

                    if (currentId == expectedId)
                    {
                        return BuildResult(previousId, expectedId, currentId, currentStatus, pollCount, startTime, isPassStatus, isTimeoutStatus);
                    }

                    if (currentId > expectedId)
                    {
                        OperateResult<int[]> historyIds = readInt32(historyIdStartAddress, 5);
                        OperateResult<int[]> historyStatuses = readInt32(historyStatusStartAddress, 5);

                        if (!historyIds.IsSuccess || !historyStatuses.IsSuccess ||
                            historyIds.Content == null || historyStatuses.Content == null ||
                            historyIds.Content.Length < 5 || historyStatuses.Content.Length < 5)
                        {
                            return new CameraSubSyncV2Result
                            {
                                Result = e_CameraSubSyncV2_Result.ReadError,
                                PreviousId = previousId,
                                ExpectedId = expectedId,
                                CurrentId = currentId,
                                PollCount = pollCount,
                                ElapsedMs = (DateTime.Now - startTime).TotalMilliseconds,
                                Message = $"Không đọc được vùng history ID/Status. IDMsg={historyIds.Message}, StatusMsg={historyStatuses.Message}"
                            };
                        }

                        var idToStatus = new Dictionary<int, int>();
                        for (int i = 0; i < 5; i++)
                        {
                            idToStatus[historyIds.Content[i]] = historyStatuses.Content[i];
                        }

                        if (!idToStatus.TryGetValue(expectedId, out int matchedStatus))
                        {
                            return new CameraSubSyncV2Result
                            {
                                Result = e_CameraSubSyncV2_Result.SyncError,
                                PreviousId = previousId,
                                ExpectedId = expectedId,
                                CurrentId = currentId,
                                PlcStatus = currentStatus,
                                PollCount = pollCount,
                                ElapsedMs = (DateTime.Now - startTime).TotalMilliseconds,
                                Message = $"ID nhảy từ {previousId} lên {currentId} nhưng không tìm thấy expected ID {expectedId} trong history D10..D14"
                            };
                        }

                        return BuildResult(previousId, expectedId, expectedId, matchedStatus, pollCount, startTime, isPassStatus, isTimeoutStatus);
                    }
                }

                System.Threading.Thread.Sleep(pollingIntervalMs);
            }

            return new CameraSubSyncV2Result
            {
                Result = e_CameraSubSyncV2_Result.NoResponse,
                PreviousId = previousId,
                ExpectedId = expectedId,
                PollCount = pollCount,
                ElapsedMs = (DateTime.Now - startTime).TotalMilliseconds,
                Message = $"Hết timeout {timeoutMs}ms nhưng ID chưa tăng từ {previousId}"
            };
        }

        private static CameraSubSyncV2Result BuildResult(
            int previousId,
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
                ExpectedId = expectedId,
                CurrentId = currentId,
                PlcStatus = plcStatus,
                PollCount = pollCount,
                ElapsedMs = (DateTime.Now - startTime).TotalMilliseconds,
                Message = $"Resolve theo ID: Prev={previousId}, Expected={expectedId}, Current={currentId}, Status={plcStatus}, Result={result}"
            };
        }
    }
}
