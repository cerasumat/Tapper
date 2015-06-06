using System;
using log4net;

namespace PCITC.MES.MM.Tapper.Framework.Log4Net
{
    //log4net 日志实现
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _log;

        public Log4NetLogger(ILog log)
        {
            _log = log;
        }

        public bool IsDebugEnabled
        {
            get { return _log.IsDebugEnabled; }
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void Debug(string format, object[] args)
        {
            _log.DebugFormat(format, args);
        }

        public void Debug(string message, Exception exp)
        {
            _log.Debug(message, exp);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Info(string format, object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public void Info(string message, Exception exp)
        {
            _log.Info(message, exp);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Error(string format, object[] args)
        {
            _log.ErrorFormat(format,args);
        }

        public void Error(string message, Exception exp)
        {
            _log.Error(message, exp);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }

        public void Warn(string format, object[] args)
        {
            _log.WarnFormat(format,args);
        }

        public void Warn(string message, Exception exp)
        {
            _log.Warn(message, exp);
        }

        public void Fatal(string message)
        {
            _log.Fatal(message);
        }

        public void Fatal(string format, object[] args)
        {
            _log.FatalFormat(format, args);
        }

        public void Fatal(string message, Exception exp)
        {
            _log.Fatal(message, exp);
        }
    }
}
