using System.Threading;
using MineLib.Network;
using MineLib.Network.Modern.Packets.Client;

namespace MineLib.ClientWrapper.Modern
{
    // Not used
    public class PlayerTickHandler
    {
        private Minecraft _minecraft;
        private Timer _timer;

        public PlayerTickHandler(Minecraft minecraft)
        {
            _minecraft = minecraft;
        }

        public void Start()
        {
            _timer = new Timer(DoTick, null, 0, 50);
        }

        private void DoTick(object state)
        {
            if (_minecraft.State == ServerState.ModernPlay)
                //_minecraft.SendPacket(new PlayerPacket { OnGround = _minecraft.Player.Position.OnGround });
                _minecraft.SendPacket(new PlayerPacket { OnGround = true });

            //_network.Send(new PlayerLookPacket { Yaw = 90, Pitch = 50, OnGround = true });
        }
    }
}
