using System.Threading;
using MineLib.ClientWrapper.BigData;
using MineLib.Network;
using MineLib.Network.Packets.Client;

namespace MineLib.ClientWrapper
{
    public class PlayerHandler
    {
        private Player _player;
        private NetworkHandler _network;
        private Timer _timer;

        public PlayerHandler(ref NetworkHandler network, ref Player player)
        {
            _network = network;
            _player = player;
        }

        public void Start()
        {
            _timer = new Timer(DoTick, null, 0, 50);
        }

        private void DoTick(object state)
        {
            _network.Send(new PlayerPacket { OnGround = _player.Position.OnGround });
            //_network.Send(new PlayerLookPacket { Yaw = 90, Pitch = 50, OnGround = true });
        }
    }
}
