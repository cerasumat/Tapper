using System.Collections.Concurrent;
using System.Linq;
using PCITC.MES.MM.Tapper.Engine.Entities;
using PCITC.MES.MM.Tapper.Framework.Utilities;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public class TaskQueue
    {
        private ConcurrentDictionary<string, TaskEntity> _taskDict = new ConcurrentDictionary<string, TaskEntity>();

        public string Topic { get; private set; }

        public int QueueId { get; private set; }

        public TaskQueueSettings Setting { get; private set; }

        public ConcurrentDictionary<string, TaskEntity> TaskEntities => _taskDict;

        public TaskQueue(string topic, int queueId) : this(topic, queueId, null) { }

        public TaskQueue(string topic, int queueId, TaskQueueSettings setting)
        {
            Ensure.NotNullOrEmpty(topic, "topic");
            Ensure.Nonnegative(queueId, "queueId");
            Topic = topic;
            QueueId = queueId;
            Setting = setting ?? new TaskQueueSettings();
        }

        public long TaskCount => _taskDict.Count;

        public long ActiveTaskCount
            => _taskDict.Values.Count(x => x.TaskState == TaskState.Running || x.TaskState == TaskState.Waiting);

        public TaskEntity TryGetTaskEntity(string taskCode)
        {
            TaskEntity task;
            _taskDict.TryGetValue(taskCode, out task);
            return task;
        }

        public TaskEntity TryGetTaskEntity(int ruleId, TaskState state)
        {
            return _taskDict.Values.FirstOrDefault(t => t.RuleId == ruleId && t.TaskState == state);
        }
    }
}
