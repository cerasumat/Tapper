namespace PCITC.MES.MM.Tapper.Engine.SignalR
{
    public interface INotify
    {
        string GetUrl();
        void Start();
        void Stop();
        void Notify(string msg);
    }
}
