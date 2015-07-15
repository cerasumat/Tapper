using System;
using CeraSumat.Utilities.Validation;

namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    [Serializable]
    public class RuleModel
    {
        //Topic(4 topics for now: TM-罐区 CU-化工 UR-炼油 PB-生产平衡
        [Required]
        public string Topic { get; set; }
        //规则模型ID
        public int RuleId { get; set; }
        //规则模型名称
        public string RuleName { get; set; }
        //宿主节点ID
        [Required]
        public int NodeId { get; set; }
        //宿主节点名称
        public string NodeName { get; set; }
        //宿主节点类型ID
        public int NodeTypeId { get; set; }
        //宿主节点名称
        public string NodeTypeName { get; set; }
        //规则生效日期
        public DateTime RuleBegDate { get; set; }
        //规则失效日期
        public DateTime RuleEndDate { get; set; }
        //周期内规则激活时间
        public DateTime RuleActiveTime { get; set; }
        //周期内规则过期时间
        public DateTime RuleExpireTime { get; set; }
        //rule acitve days of month -- <list> means that rule could be actived in a couple days of month (split by ',')
        public string RuleDaysInMonth { get; set; }
        //rule active days of week -- <list> means that rule could be actived in a couple days of week (split by ',')
        public string RuleDaysInWeek { get; set; } 
        //规则关联任务ID
        [Required]
        public int TaskId { get; set; }
        //规则关联任务参数(任务参数JSON串)
        [Required]
        public string TaskParams { get; set; }
        //规则是否为单向链表的表头
        public int IsHeader { get; set; }
        //单向链表中的下一个规则
        public int RuleIdNext { get; set; }

        public override string ToString()
        {
            return string.Format("[RuleId={0},NodeId={1},NodeType={2},TaskId={3}]", RuleId, NodeId, NodeTypeName, TaskId);
        }
    }
}
