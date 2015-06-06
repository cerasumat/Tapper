using System;
using PCITC.MES.MM.Tapper.Framework.Autofac;
using PCITC.MES.MM.Tapper.Framework.Log4Net;
using PCITC.MES.MM.Tapper.Framework.Scheduling;
using PCITC.MES.MM.Tapper.Framework.Serializing;

namespace PCITC.MES.MM.Tapper.Framework.Configurations
{
    public class Configuration
    {
        public static Configuration Instance { get; private set; }

        public Setting Setting { get; private set; }

        private Configuration(Setting setting)
        {
            Setting = setting ?? new Setting();
        }

        public static Configuration Create(Setting setting = null)
        {
            if (Instance != null)
            {
                throw new Exception("已存在Configuration实例,不能重复创建.");
            }
            Instance = new Configuration(setting);
            return Instance;
        }

        public Configuration SetDefault<TService, TImplementer>(LifeStyle life = LifeStyle.Singleton)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.Register<TService, TImplementer>(life);
            return this;
        }

        public Configuration SetDefault<TService, TImplementer>(TImplementer instance)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.RegisterInstance<TService, TImplementer>(instance);
            return this;
        }

        public Configuration RegisterCommonComponents()
        {
            SetDefault<ILog4NetLoggerFactory, Log4NetLoggerFactory>();
            SetDefault<IBinarySerializer, DefaultBinarySerializer>();
            SetDefault<IJsonSerializer, NewtonsoftJsonSerializer>();
            SetDefault<IScheduleService, ScheduleService>();
            return this;
        }
        public Configuration RegisterUnhandledExceptionHandler()
        {
            var logger = ObjectContainer.Resolve<ILog4NetLoggerFactory>().Create(GetType().FullName);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => logger.Error("Unhandled exception: {0}", e.ExceptionObject);
            return this;
        }
    }
}
