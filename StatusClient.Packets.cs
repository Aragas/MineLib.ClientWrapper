﻿using MineLib.Network.Enums;
using MineLib.Network.Packets;

namespace MineLib.ClientWrapper
{
    public partial class StatusClient
    {
        private void RaisePacketHandled(IPacket packet, int id, ServerState state)
        {
            if (state != ServerState.Status) return;

            switch ((PacketsServer)id)
            {
                case PacketsServer.Ping:
					if (FirePingPacket != null)
						FirePingPacket(packet);
                    break;

                case PacketsServer.Response:
                    if (FireResponsePacket != null)
						FireResponsePacket(packet);
                    break;
            }
        }

    }
}
