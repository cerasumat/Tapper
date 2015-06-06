using System.Configuration;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class NotifySettings
    {
        public int NotifyInfoStoreInterval { get; private set; }
        public string NotifyInfoTable => "T_MV_NOTIFY_ENTITY";
        public string ConnectionStr => ConfigurationManager.ConnectionStrings["oracleConn"].ConnectionString;
        public NotifySettings()
        {
            NotifyInfoStoreInterval = 1000 * 60;
        }
    }
}
