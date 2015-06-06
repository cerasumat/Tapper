using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;

namespace PCITC.MES.MM.Tapper.Framework.Log4Net
{
    public class Log4NetLoggerFactory : ILog4NetLoggerFactory
    {
        // Parameterized constructor
        public Log4NetLoggerFactory(string configFile)
        {
            var file = new FileInfo(configFile);
            if (!file.Exists)
            {
                file = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile));
            }
            if (file.Exists)
            {
                XmlConfigurator.ConfigureAndWatch(file);
            }
            else
            {
                BasicConfigurator.Configure(new ConsoleAppender { Layout = new PatternLayout() });
            }
        }

        // Create a logger instance
        public ILogger Create(string name)
        {
            ILogger logger;
            if(name.Contains("Producer"))
                logger=new Log4NetLogger(LogManager.GetLogger("ProducerLogger"));
            else if (name.Contains("Consumer"))
                logger = new Log4NetLogger(LogManager.GetLogger("ConsumerLogger"));
            else if(name.Contains("Broker"))
                logger = new Log4NetLogger(LogManager.GetLogger("BrokerLogger"));
            else
                logger = new Log4NetLogger(LogManager.GetLogger("FrameworkLogger"));
            return logger;
        }

        public ILogger Create(Type type)
        {
            return new Log4NetLogger(LogManager.GetLogger(type));
        }
    }
}
