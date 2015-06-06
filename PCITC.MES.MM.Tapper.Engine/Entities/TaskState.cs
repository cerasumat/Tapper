namespace PCITC.MES.MM.Tapper.Engine.Entities
{
    public enum TaskState
    {
        Waiting = 1,
        Running = 2,
        Complete = 4,
        Failed = 8,
        Canceled = 16
    }
}
