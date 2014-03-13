using MineLib.Network.Events;

namespace MineLib.ClientWrapper
{
    public partial class StatusClient
    {
        public event PacketHandler FirePingPacket;
        public event PacketHandler FireResponsePacket;
    }
}
