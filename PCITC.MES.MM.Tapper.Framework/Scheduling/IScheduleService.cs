using System;

namespace PCITC.MES.MM.Tapper.Framework.Scheduling
{
    public interface IScheduleService
    {
        /// <summary>
        /// 对一个以时间为周期的action进行任务调度
        /// </summary>
        /// <param name="actionName">action名称</param>
        /// <param name="action">时间基础action</param>
        /// <param name="dueTime">生效时间</param>
        /// <param name="period">调度周期</param>
        /// <returns></returns>
        int ScheduleTask(string actionName, Action action, int dueTime, int period);
        /// <summary>
        /// 关闭正在调度的任务
        /// </summary>
        /// <param name="taskId"></param>
        void ShutdownTask(int taskId);
    }
}
