using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;

namespace PCITC.MES.MM.Tapper.Framework.Scheduling
{
    /// <summary>
    /// background worker that repeatly execute a specific method
    /// </summary>
    public class BackWorker
    {
        private readonly string _actionName;
        private readonly Action _action;
        private readonly ILogger _logger;
        private WorkerState _currentState;

        /// <summary>
        /// 返回当前work的名称
        /// </summary>
        public string ActionName
        {
            get { return _actionName; }
        }

        public BackWorker(string actionName, Action action)
        {
            _actionName = actionName;
            _action = action;
            _logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
        }

        public BackWorker Start()
        {
            if (_currentState != null && !_currentState.StopRequested) return this;

            var thread = new Thread(Loop)
            {
                Name = string.Format("{0}.Worker", _actionName),
                IsBackground = true
            };
            var state = new WorkerState();

            thread.Start(state);

            _currentState = state;
            _logger.Debug("BackWorker启动, actionName={0}, id={1}, managedThreadId={2}, nativeThreadId={3}", _actionName,
                state.Id, thread.ManagedThreadId, GetNativeThreadId(thread));

            return this;
        }

        public BackWorker Stop()
        {
            if (_currentState == null) return this;

            _currentState.StopRequested = true;
            _logger.Debug("Worker stop requested, actionName:{0}, id:{1}", _actionName, _currentState.Id);
            return this;
        }

        private void Loop(object data)
        {
            var state = (WorkerState)data;

            while (!state.StopRequested)
            {
                try
                {
                    _action();
                    Thread.Sleep(500);      //BackWorker Action Internal
                }
                catch (ThreadAbortException)
                {
                    _logger.Info("BackWorker ThreadAbortException, 尝试进行重置, actionName={0}", _actionName);
                    Thread.ResetAbort();
                    _logger.Info("ThreadAbortException已重置, actionName={0}", _actionName);
                }
                catch (Exception ex)
                {
                    _logger.Error(string.Format("BackWorker action 异常, actionName={0}", _actionName), ex);
                }
            }
        }

        private static int GetNativeThreadId(Thread thread)
        {
            var fieldInfo = typeof(Thread).GetField("DONT_USE_InternalThread", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Instance);
            var pInternalThread = (IntPtr)fieldInfo.GetValue(thread);
            var nativeId = Marshal.ReadInt32(pInternalThread, (IntPtr.Size == 8) ? 548 : 348); // found by analyzing the memory
            return nativeId;
        }

        class WorkerState
        {
            public string Id = Guid.NewGuid().ToString("N");
            public bool StopRequested;
        }
    }
}
