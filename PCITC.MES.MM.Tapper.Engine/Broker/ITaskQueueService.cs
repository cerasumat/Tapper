using System.Collections.Generic;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public interface ITaskQueueService
    {
        int QueueCount { get; }
        long TaskCount { get; }
        long ActiveTaskCount { get; }

        void Start();
        void Shutdown();
        IEnumerable<string> GetAllTopics();
        bool IsQueueExist(string topic, int queueId);
        TaskQueue GetQueue(string topic, int queueId);
        IList<int> QueryQueueIds(string topic);
        IEnumerable<TaskQueue> QueryQueues(string topic);
    }
}
