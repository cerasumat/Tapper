using System;
using System.Threading;

namespace PCITC.MES.MM.Tapper.Framework.Scheduling
{
    internal class TimerBaseTask
    {
        internal string ActionName { get; set; }
        internal Action Action { get; set; }
        internal Timer Timer { get; set; }
        internal int DueTime { get; set; }
        internal int Period { get; set; }
        internal bool Stopped { get; set; }
    }
}
