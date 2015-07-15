using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCITC.MES.MM.Tapper.Console
{
    public class ConsoleSetting
    {
        public string TopicModelTable => "T_MV_TOPIC_MODEL";
        public string LogEntityTable => "T_MV_NOTIFY_ENTITY";
        public string ConnectionStr => ConfigurationManager.ConnectionStrings["oracleConn"].ConnectionString;
        public string ServiceIp => ConfigurationManager.AppSettings["ServiceBaseIp"];
    }
}
