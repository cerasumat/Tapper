using System.Configuration;

namespace WebApi
{
    public class ApiSetting
    {
        public string TopicModelTable => "T_MV_TOPIC_MODEL";
        public string LogEntityTable => "T_MV_NOTIFY_ENTITY";
        public string ConnectionStr => ConfigurationManager.ConnectionStrings["oracleConn"].ConnectionString;
    }
}
