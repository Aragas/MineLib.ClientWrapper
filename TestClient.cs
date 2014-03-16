using System;
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
        private static ResponseData ServerData;

        public static void Main(string[] args)
        {
            string ServerIP = args[0];                  // localhost
            short ServerPort = short.Parse(args[1]);    // 25565

            Client = new Minecraft("TestBot", "", false);

            Client.RefreshSession();

            /*
             Connect()
             Send new HandshakePacket       (4, localhost, 25565, Login)
             Send new LoginStartPacket      (Username)
             Send new PluginMessagePacket   (MC|Brand, ClientBrand)
             
             */

            using (var SClient = new StatusClient(ServerIP, ServerPort))
            {
                ServerData = SClient.GetServerInfo(4);
            }

            Client.Connect(ServerIP, ServerPort);

            Client.SendPacket(new HandshakePacket
            {
                ProtocolVersion = 4,
                ServerAddress = ServerIP,
                ServerPort = ServerPort,
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