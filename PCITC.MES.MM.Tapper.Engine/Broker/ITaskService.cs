using System;
using System.Collections.Generic;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public interface ITaskService
    {
        bool IsTaskExisted(string topic, int ruleId, DateTime activeTime);
        bool AddTaskToQueue(TaskEntity task, int queueId);
        IEnumerable<TaskEntity> GetWaitingTaskFromQueue(string topic, int queueId, int batchSize);
        bool UpdateTaskInQueue(TaskEntity task);
    }
}
