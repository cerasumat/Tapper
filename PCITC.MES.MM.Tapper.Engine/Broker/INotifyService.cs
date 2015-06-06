using System;
using PCITC.MES.MM.Tapper.Engine.Entities;

namespace PCITC.MES.MM.Tapper.Engine.Broker
{
    public interface INotifyService
    {
        void Start();
        void Shutdown();
        void AddInfoNotify(string message, TaskEntity task, Exception exp);
        void AddDebugNotify(string message, TaskEntity task, Exception exp);
        void AddWarnNotify(string message, TaskEntity task, Exception exp);
        void AddErrorNotify(string message, TaskEntity task, Exception exp);
        void AddFatalNotify(string message, TaskEntity task, Exception exp);
    }
}
