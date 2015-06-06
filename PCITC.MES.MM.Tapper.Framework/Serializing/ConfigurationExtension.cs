using PCITC.MES.MM.Tapper.Framework.Configurations;

namespace PCITC.MES.MM.Tapper.Framework.Serializing
{
    public static class ConfigurationExtension
    {
        /// <summary>
        /// Use Json.Net as the json serializer.
        /// </summary>
        /// <returns></returns>
        public static Configuration UseJsonNet(this Configuration configuration)
        {
            configuration.SetDefault<IJsonSerializer, NewtonsoftJsonSerializer>(new NewtonsoftJsonSerializer());
            return configuration;
        }
    }
}
