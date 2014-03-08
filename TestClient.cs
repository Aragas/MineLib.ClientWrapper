using System.Collections.Generic;
using System.Net.Sockets;
using MineLib.ClientWrapper.BigData;
using MineLib.Network.Enums;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Client.Login;
using MineLib.Network.Packets.Server.Login;

namespace MineLib.ClientWrapper
{
    public static class TestClient
    {
        public static World World;
        static Minecraft Client;
        static List<IPacket> list = new List<IPacket>();
        static NetworkStream nStream;

        public static void Main(string[] args)
        {
            Client = new Minecraft("TestBot", "", false);

            Client.FirePacketHandled += Client_PacketHandled;

            //Client.Login();
            //Client.RefreshSession();
            

            Client.Connect("127.0.0.1", 25565);

            Client.SendPacket(new HandshakePacket
            {
                ProtocolVersion = 4,
                ServerAddress = "127.0.0.1",
                ServerPort = 25565,
                NextState = NextState.Login,
            });

            Client.SendPacket(new LoginStartPacket { Name = "TestBot" });

            //while (!Client.Ready) { }

            //Client.SendPacket(new ClientStatusPacket { Status = ClientStatus.InitialSpawn} );
            //Client.SendPacket(new ClientStatusPacket { Status = ClientStatus.Respawn });

            //Client.SendPacket(new ClientStatusPacket { Status = ClientStatus.InitialSpawn} );

            while (true)
            {
                World = Client.World;
            }
            //Console.Read();
        }

        private static void Client_PacketHandled(object sender, IPacket packet, int id, ServerState state)
        {
            list.Add(packet);
        }
    }
}
