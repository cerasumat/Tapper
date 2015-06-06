using System.Collections.Generic;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public interface ITopicService
    {
        IList<string> AllTopics { get; }
        TopicModel GetTopicModelByName(string topicName);
        void Start();
    }
}
