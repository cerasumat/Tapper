using System.Configuration;

namespace PCITC.MES.MM.Tapper.Engine.Producer
{
    public class ProducerSettings
    {
        public int GetModelsFromDatabaseInterval { get; set; }
        public int GenerateTaskFromRulesInterval { get; set; }
        public int UpdateTopicQueueCountInterval { get; set; }
        public string RuleModelTable => "T_MV_RULE_MODEL"; 
        public string TaskModelTable => "T_MV_TASK_MODEL";
        public string ConnectionStr => ConfigurationManager.ConnectionStrings["oracleConn"].ConnectionString;

        public ProducerSettings()
        {
            GetModelsFromDatabaseInterval = 1000 * 600;
            GenerateTaskFromRulesInterval = 1000 * 11;
            UpdateTopicQueueCountInterval = 1000 * 60;
        }
    }
}
