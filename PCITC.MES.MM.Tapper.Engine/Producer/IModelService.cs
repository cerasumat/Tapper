using System.Collections.Concurrent;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Engine.Producer
{
    public interface IModelService
    {
        ModelSettings Setting { get;}
        void Start();
        void Shutdown();
        ConcurrentDictionary<int, RuleModel> GetRuleModels();
        ConcurrentDictionary<int, TaskModel> GetTaskModels();
    }
}
