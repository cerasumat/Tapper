using PCITC.MES.MM.Tapper.Framework.Configurations;

namespace PCITC.MES.MM.Tapper.Framework.Log4Net
{
    public static class ConfigurationExtension
    {
        /// <summary>Use Log4Net as the logger.
        /// </summary>
        /// <returns></returns>
        public static Configuration UseLog4Net(this Configuration configuration)
        {
            return UseLog4Net(configuration, "Log4Net.config");
        }
        /// <summary>Use Log4Net as the logger.
        /// </summary>
        /// <returns></returns>
        public static Configuration UseLog4Net(this Configuration configuration, string configFile)
        {
            configuration.SetDefault<ILog4NetLoggerFactory, Log4NetLoggerFactory>(new Log4NetLoggerFactory(configFile));
            return configuration;
        }
    }
}
