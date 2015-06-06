using PCITC.MES.MM.Tapper.Framework.TcpTransport;

namespace PCITC.MES.MM.Tapper.Framework.Configurations
{
    public class Setting
    {
        public TcpConfiguration TcpConfiguration { get; set; }

        public Setting()
        {
            TcpConfiguration = new TcpConfiguration();
        }
    }
}
