using System;
using MineLib.Network.Enums;
using MineLib.Network.Packets;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        private void RaisePacketHandledClassic(IPacket packet, int id, ServerState? state)
        {
            // -- Debugging
            Console.WriteLine("ID: 0x" + String.Format("{0:X}", id));
            Console.WriteLine(" ");
            // -- Debugging
        }
    }
}
