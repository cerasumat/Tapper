﻿using System;

namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    [Serializable]
    public class TaskModel
    {
        //任务模型ID
        public int TaskId { get; set; }
        //任务模型名称
        public string TaskName { get; set; }
        //任务行为
        public string TaskAction { get; set; }
        //任务调度的time out(秒)
        public int TaskMaxDispatchTime { get; set; }
        //任务参数格式说明（JSON串）
        public string TaskParamFormat { get; set; }

        public override string ToString()
        {
            return string.Format("[TaskId={0},TaskFormula={1},TaskMaxDispatchTime={2}]", TaskId, TaskAction,
                TaskMaxDispatchTime);
        }
    }
}
