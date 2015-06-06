using System;

namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    [Serializable]
    public class TaskEntity
    { 
        //Topic(4 topics for now: TM-罐区 CU-化工 UR-炼油 PB-生产平衡
        public string Topic { get; set; }
        //entity global unique id(guid)
        public string TaskCode { get; set; }
        //生成任务实体的规则ID
        public int RuleId { get; set; }
        //生成任务实体的规则名称
        public string RuleName { get; set; }
        //生成任务实体的任务ID
        public int TaskId { get; set; }
        //生成任务实体的任务名称
        public string TaskName { get; set; }    
        //任务参数（JSON串）  
        public string TaskParameters { get; set; }  
        //任务实体宿主NodeId
        public int NodeId { get; set; }
        //任务实体宿主Node名称
        public string NodeName { get; set; }
        //任务实体宿主Node类型ID
        public int NodeTypeId { get; set; }
        //任务实体宿主Node类型名称
        public string NodeTypeName { get; set; }
        //任务行为
        public string TaskAction { get; set; }
        //任务生效时间
        public DateTime TaskActiveAt { get; set; }
        //任务过期时间
        public DateTime TaskExpireAt { get; set; }
        //任务最大调度时长(s)
        public int TaskMaxDispatchTime { get; set; }
        //任务开始调度时间
        public DateTime TaskDispatchAt { get; set; }
        //task dispatch time deadline('running' state last over this deadline would turn to 'failed' state)
        public DateTime TaskTimeOutAt
        {
            get { return TaskDispatchAt.AddSeconds(TaskMaxDispatchTime); }
        }
        //task dispatch count ( state set to 'failed' if this count >=3 and task will not be dispatched any more)
        public int TaskDispatchCount { get; set; }
        //任务状态
        public TaskState TaskState { get; set; }
        // If the task be the header of the task link
        public int IsHeader { get; set; }
        // the next task(ruleId) in the task link
        public int RuleIdNext { get; set; }

        public bool IsWaiting
        {
            get { return TaskState == TaskState.Waiting; }
        }

        public bool IsRunning
        {
            get { return TaskState == TaskState.Running; }
        }

        public bool IsComplete
        {
            get { return TaskState == TaskState.Complete; }
        }

        public bool IsFailed
        {
            get { return TaskState == TaskState.Failed; }
        }

        public bool IsCancel
        {
            get { return TaskState == TaskState.Canceled; }
        }

        public bool IsCreat
        {
            get { return TaskState == TaskState.Created; }
        }

        public bool IsExpired
        {
            get { return TaskExpireAt <= DateTime.Now; }
        }

        public override string ToString()
        {
            return
                string.Format("[TaskTopic={0},TaskName={1},Node={2},TaskDispathAt={3},TaskTimeOutAt={4},TaskState={5}]",
                    Topic, TaskName, NodeName, TaskDispatchAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    TaskTimeOutAt.ToString("yyyy-MM-dd HH:mm:ss"), TaskState);
        }
    }
}
