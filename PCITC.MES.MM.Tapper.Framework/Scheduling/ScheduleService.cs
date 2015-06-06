using System;
using System.Collections.Concurrent;
using System.Threading;
using PCITC.MES.MM.Tapper.Framework.Log4Net;

namespace PCITC.MES.MM.Tapper.Framework.Scheduling
{
    public class ScheduleService : IScheduleService
    {
        private readonly ConcurrentDictionary<int, TimerBaseTask> _taskDict =
            new ConcurrentDictionary<int, TimerBaseTask>();
        private readonly ILogger _logger;
        private int _maxTaskId;

        public ScheduleService(ILog4NetLoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create(GetType().FullName);
        }

        public int ScheduleTask(string actionName, Action action, int dueTime, int period)
        {
            var newTaskId = Interlocked.Increment(ref _maxTaskId);
            var timer = new Timer(GetTimerCallBack(), newTaskId, Timeout.Infinite, Timeout.Infinite);
            if (!_taskDict.TryAdd(newTaskId, new TimerBaseTask { ActionName = actionName, Action = action, Timer = timer, DueTime = dueTime, Period = period, Stopped = false }))
            {
                _logger.Error("任务加入调度队列失败, actionName={0}, dueTime={1}, period={2}", actionName, dueTime, period);
                return -1;
            }
            timer.Change(dueTime, period);
            _logger.Debug("任务加入调度队列成功, actionName={0}, dueTime={1}, period={2}", actionName, dueTime, period);
            return newTaskId;
        }

        public void ShutdownTask(int taskId)
        {
            TimerBaseTask task;
            if (!_taskDict.TryRemove(taskId, out task)) return;
            task.Stopped = true;
            task.Timer.Change(Timeout.Infinite, Timeout.Infinite);
            task.Timer.Dispose();
            _logger.Debug("成功中止调度任务, actionName={0}, dueTime={1}, period={2}", task.ActionName, task.DueTime, task.Period);
        }

        private TimerCallback GetTimerCallBack()
        {
            return obj =>
            {
                var currentTaskId = (int) obj;
                TimerBaseTask currentTast;
                if (!_taskDict.TryGetValue(currentTaskId, out currentTast)) return;
                if (currentTast.Stopped) return;
                try
                {
                    currentTast.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                    if (currentTast.Stopped) return;
                    currentTast.Action();
                }
                catch (ObjectDisposedException)
                {
                }
                catch (Exception exp)
                {
                    _logger.Error(
                        string.Format("任务调度异常:actionName={0},dueTime={1},period={2}", currentTast.ActionName,
                            currentTast.DueTime, currentTast.Period), exp);
                }
                finally
                {
                    if (!currentTast.Stopped)
                    {
                        try
                        {
                            currentTast.Timer.Change(currentTast.Period, currentTast.Period);
                        }
                        catch (ObjectDisposedException)
                        {
                        }
                    }

                }
            };
        }
    }
}
