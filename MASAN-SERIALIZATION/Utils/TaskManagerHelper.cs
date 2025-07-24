using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly Dictionary<string, Stopwatch> _taskTimers = new Dictionary<string, Stopwatch>();
        private readonly object _taskLock = new object();

        /// <summary>
        /// Start a task with optional progress reporting and logging.
        /// </summary>
        public void Start(
            string taskName,
            Func<CancellationToken, IProgress<int>, Task> taskFunc,
            Action<string> logAction = null,
            Action<int> progressAction = null)
        {
            lock (_taskLock)
            {
                if (_runningTasks.ContainsKey(taskName))
                {
                    var existingTask = _runningTasks[taskName];
                    if (!(existingTask.IsCompleted || existingTask.IsCanceled || existingTask.IsFaulted))
                    {
                        logAction?.Invoke($"⚠️ Task '{taskName}' đang chạy, không khởi chạy lại.");
                        return;
                    }
                }

                var cts = new CancellationTokenSource();
                _taskTokens[taskName] = cts;

                var progress = new Progress<int>(percent =>
                {
                    progressAction?.Invoke(percent);
                    logAction?.Invoke($"Task '{taskName}' tiến độ: {percent}%");
                });

                var stopwatch = Stopwatch.StartNew();
                _taskTimers[taskName] = stopwatch;

                var task = Task.Run(async () =>
                {
                    await taskFunc(cts.Token, progress);
                }, cts.Token);

                _runningTasks[taskName] = task;

                task.ContinueWith(t =>
                {
                    stopwatch.Stop();

                    if (t.IsFaulted)
                    {
                        logAction?.Invoke($"❌ Task '{taskName}' lỗi: {t.Exception?.GetBaseException().Message}");
                    }
                    else if (t.IsCanceled)
                    {
                        logAction?.Invoke($"⚠️ Task '{taskName}' đã bị huỷ.");
                    }
                    else
                    {
                        logAction?.Invoke($"✅ Task '{taskName}' đã hoàn thành sau {stopwatch.Elapsed.TotalSeconds:F1} giây.");
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        /// <summary>
        /// Stop a task by name.
        /// </summary>
        public void Stop(string taskName, Action<string> logAction = null)
        {
            lock (_taskLock)
            {
                if (_taskTokens.ContainsKey(taskName))
                {
                    _taskTokens[taskName].Cancel();
                    logAction?.Invoke($"🛑 Đã yêu cầu dừng task '{taskName}'.");
                }
                else
                {
                    logAction?.Invoke($"⚠️ Task '{taskName}' không tồn tại.");
                }
            }
        }

        /// <summary>
        /// Stop all running tasks.
        /// </summary>
        public void StopAll(Action<string> logAction = null)
        {
            lock (_taskLock)
            {
                foreach (var kv in _taskTokens)
                {
                    kv.Value.Cancel();
                    logAction?.Invoke($"🛑 Đã yêu cầu dừng task '{kv.Key}'.");
                }
            }
        }

        /// <summary>
        /// Check if a task is running.
        /// </summary>
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

        /// <summary>
        /// Get running seconds of a task.
        /// </summary>
        public double GetRunningSeconds(string taskName)
        {
            lock (_taskLock)
            {
                if (_taskTimers.ContainsKey(taskName))
                {
                    return _taskTimers[taskName].Elapsed.TotalSeconds;
                }
                return 0;
            }
        }

        /// <summary>
        /// Get the Task object by name (null if not found).
        /// </summary>
        public Task GetTask(string taskName)
        {
            lock (_taskLock)
            {
                if (_runningTasks.ContainsKey(taskName))
                {
                    return _runningTasks[taskName];
                }
                return null;
            }
        }

        /// <summary>
        /// Get task status by name: Running, Completed, Canceled, Faulted, NotFound.
        /// </summary>
        public string GetTaskStatus(string taskName)
        {
            lock (_taskLock)
            {
                if (_runningTasks.ContainsKey(taskName))
                {
                    var task = _runningTasks[taskName];
                    if (task.IsCanceled)
                        return "Canceled";
                    else if (task.IsFaulted)
                        return "Faulted";
                    else if (task.IsCompleted)
                        return "Completed";
                    else
                        return "Running";
                }
                else
                {
                    return "NotFound";
                }
            }
        }

        /// <summary>
        /// Get all task names with their statuses.
        /// </summary>
        public Dictionary<string, string> GetAllTaskStatus()
        {
            var result = new Dictionary<string, string>();

            lock (_taskLock)
            {
                foreach (var kv in _runningTasks)
                {
                    var name = kv.Key;
                    var task = kv.Value;
                    string status;

                    if (task.IsCanceled)
                        status = "Canceled";
                    else if (task.IsFaulted)
                        status = "Faulted";
                    else if (task.IsCompleted)
                        status = "Completed";
                    else
                        status = "Running";

                    result.Add(name, status);
                }
            }
            return result;
        }
    }
}
