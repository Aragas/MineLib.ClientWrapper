using System.Threading;
using MineLib.Network.Enums;
using MineLib.Network.Packets.Client;

namespace MineLib.ClientWrapper
{
    // Not used
    public class PlayerHandler
    {
        private Minecraft _minecraft;
        private Timer _timer;

        public PlayerHandler(Minecraft minecraft)
        {
            _minecraft = minecraft;
        }

        public void Start()
        {
            _timer = new Timer(DoTick, null, 0, 50);
        }

        private void DoTick(object state)
        {
            if (_minecraft.State == ServerState.Play)
                _minecraft.SendPacket(new PlayerPacket { OnGround = _minecraft.Player.Position.OnGround });
            //_network.Send(new PlayerLookPacket { Yaw = 90, Pitch = 50, OnGround = true });
        }
    }
}
