using System.Collections.Generic;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public interface IConsumerManager
    {
        void Start();
        void Shutdown();
        IList<string> GetConsumerSubscribedTopics(string consumerId);
        IList<string> GetActiveConsumers();
        int GetActiveConsumersCount();
        void UpdateConsumer(string consumerId, IList<string> subscribedTopics);
    }
}
