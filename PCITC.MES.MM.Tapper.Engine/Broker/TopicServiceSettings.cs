using System.Configuration;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class TopicServiceSettings
    {
        public string TopicModelTable => "T_MV_TOPIC_MODEL";
        public string ConnectionStr => ConfigurationManager.ConnectionStrings["oracleConn"].ConnectionString;
    }
}
