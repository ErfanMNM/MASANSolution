using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MASAN_SERIALIZATION.Utils
{
    public class TaskManagerHelper
    {
        private readonly Dictionary<string, Task> _runningTasks = new Dictionary<string, Task>();
        private readonly Dictionary<string, CancellationTokenSource> _taskTokens = new Dictionary<string, CancellationTokenSource>();
        private readonly object _taskLock = new object();

        public void Start(string taskName, Func<CancellationToken, Task> taskFunc, Action<string> logAction = null)
        {
            lock (_taskLock)
            {
                if (_runningTasks.ContainsKey(taskName))
                {
                    var existingTask = _runningTasks[taskName];
                    if (!(existingTask.IsCompleted || existingTask.IsCanceled || existingTask.IsFaulted))
                    {
                        logAction?.Invoke($"Task '{taskName}' đang chạy, không khởi chạy lại.");
                        return;
                    }
                }

                var cts = new CancellationTokenSource();
                _taskTokens[taskName] = cts;

                var task = Task.Run(() => taskFunc(cts.Token), cts.Token);

                _runningTasks[taskName] = task;

                task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        logAction?.Invoke($"Task '{taskName}' lỗi: {t.Exception?.GetBaseException().Message}");
                    }
                    else if (t.IsCanceled)
                    {
                        logAction?.Invoke($"Task '{taskName}' đã bị huỷ.");
                    }
                    else
                    {
                        logAction?.Invoke($"Task '{taskName}' đã hoàn thành.");
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext()); // callback trên UI thread
            }
        }

        public void Stop(string taskName, Action<string> logAction = null)
        {
            lock (_taskLock)
            {
                if (_taskTokens.ContainsKey(taskName))
                {
                    _taskTokens[taskName].Cancel();
                    logAction?.Invoke($"Đã yêu cầu dừng task '{taskName}'.");
                }
                else
                {
                    logAction?.Invoke($"Task '{taskName}' không tồn tại.");
                }
            }
        }

        public void StopAll(Action<string> logAction = null)
        {
            lock (_taskLock)
            {
                foreach (var kv in _taskTokens)
                {
                    kv.Value.Cancel();
                    logAction?.Invoke($"Đã yêu cầu dừng task '{kv.Key}'.");
                }
            }
        }

        public bool IsRunning(string taskName)
        {
            lock (_taskLock)
            {
                if (_runningTasks.ContainsKey(taskName))
                {
                    var task = _runningTasks[taskName];
                    return !(task.IsCompleted || task.IsCanceled || task.IsFaulted);
                }
                return false;
            }
        }
    }
}
