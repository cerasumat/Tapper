using System.Configuration;

namespace PCITC.MES.MM.Tapper.Svc
{
    public class ApiSetting
    {
        public string TopicModelTable => "T_MV_TOPIC_MODEL";
        public string LogEntityTable => "T_MV_NOTIFY_ENTITY";
        public string ConnectionStr => ConfigurationManager.ConnectionStrings["oracleConn"].ConnectionString;
    }
}
