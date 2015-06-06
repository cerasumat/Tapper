using System.Configuration;

namespace PCITC.MES.MM.Tapper.Engine.Producer
{
    public class ModelSettings
    {
        public int GetModelsFromDatabaseInterval { get; private set; }

        public string RuleModelTable => "T_MV_RULE_MODEL";
        public string TaskModelTable => "T_MV_TASK_MODEL";
        public string ConnectionStr => ConfigurationManager.ConnectionStrings["oracleConn"].ConnectionString;

        public ModelSettings()
        {
            GetModelsFromDatabaseInterval = 1000 * 600;
        }
    }
}
