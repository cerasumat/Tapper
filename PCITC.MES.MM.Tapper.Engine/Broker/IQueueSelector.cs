using System.Collections.Generic;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public interface IQueueSelector
    {
        int SelectQueueId(IList<int> availableQueueIds, TaskEntity task, string routingKey);
        string SelectTopic(IList<string> topics);
        int SelectQueueIdInTopic(IList<int> availableQueueIds);
    }
}
