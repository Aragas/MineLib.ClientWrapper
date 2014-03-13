using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using MineLib.ClientWrapper.BigData;
using MineLib.Network.Enums;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Client;
using MineLib.Network.Packets.Client.Login;

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

            using (StatusClient SClient = new StatusClient("127.0.0.1", 25565))
            {
                var info = SClient.GetServerInfo(4);
                //Console.Read();
            }

            //Client.Login();
            //Client.RefreshSession();

            /*
             Connect()
             Send new HandshakePacket       (4, localhost, 25565, Login)
             Send new LoginStartPacket      (Username)
             Send new PluginMessagePacket   (MC|Brand, ClientBrand)
             
             */

            Client.FirePacketHandled += Client_PacketHandled;

            Client.Connect("127.0.0.1", 25565);

            Client.SendPacket(new HandshakePacket
            {
                ProtocolVersion = 4,
                ServerAddress = "127.0.0.1",
                ServerPort = 25565,
                NextState = NextState.Login,
            });

            Client.SendPacket(new LoginStartPacket {Name = "TestBot"});

            while (Client.State == ServerState.Login) {}

            Client.SendPacket(new PluginMessagePacket
            {
                Channel = "MC|Brand",
                Data = Encoding.UTF8.GetBytes("MineLib.Net")
            });

            Client.SendPacket(new ClientStatusPacket {Status = ClientStatus.Respawn});

            
            while (true) {}

        }

        private static void Client_PacketHandled(IPacket packet, int id, ServerState state)
        {
            list.Add(packet);
        }
    }
}
