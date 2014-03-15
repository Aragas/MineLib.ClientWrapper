using System;
using System.Net.Sockets;
using System.Text;
using MineLib.Network.BaseClients;
using MineLib.Network.Enums;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Client;
using MineLib.Network.Packets.Client.Login;

namespace MineLib.ClientWrapper
{
    public static class TestClient
    {
        private static Minecraft Client;
        private static NetworkStream nStream;
        private static ResponseData ServerData;

        public static void Main(string[] args)
        {
            Client = new Minecraft("TestBot", "", false);

            Client.RefreshSession();

            /*
             Connect()
             Send new HandshakePacket       (4, localhost, 25565, Login)
             Send new LoginStartPacket      (Username)
             Send new PluginMessagePacket   (MC|Brand, ClientBrand)
             
             */

            using (var SClient = new StatusClient("localhost", 25565))
            {
                ServerData = SClient.GetServerInfo(4);
            }

            Client.Connect("localhost", 25565);

            Client.SendPacket(new HandshakePacket
            {
                ProtocolVersion = 4,
                ServerAddress = "localhost",
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

            Console.Read();
        }
    }
}