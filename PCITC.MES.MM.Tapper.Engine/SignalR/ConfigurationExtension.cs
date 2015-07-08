using PCITC.MES.MM.Tapper.Framework.Configurations;

namespace PCITC.MES.MM.Tapper.Engine.SignalR
{
    public static class ConfigurationExtension
    {
        public static Configuration RegisterNotification(this Configuration configuration)
        {
            configuration.SetDefault<INotify, NotificationHub>(new NotificationHub());
            return configuration;
        }  
    }
}
