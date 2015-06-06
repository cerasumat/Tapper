using System;

namespace PCITC.MES.MM.Tapper.Framework.Log4Net
{
    public interface ILog4NetLoggerFactory
    {
        // create a logger with the given logger name
        ILogger Create(string name);

        // create a logger with the given type
        ILogger Create(Type type);
    }
}
