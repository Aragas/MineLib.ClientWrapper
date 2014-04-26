using System;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MineLib.ClientWrapper.Data.Anvil;
using MineLib.Network.BaseClients;
using MineLib.Network.Enums;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Client;
using MineLib.Network.Packets.Client.Login;

namespace MineLib.ClientWrapper
{
    public static class TestClient
    {
        public static Minecraft Client;
        private static ResponseData ServerData;
        public static Chunk[,,] hh = new Chunk[10,10,10];

        public static void Main(string[] args)
        {


            string ServerIP = "127.0.0.1";  //args[0];                 // localhost
            short ServerPort = 25565;       //short.Parse(args[1]);    // 25565

            // Gettin' information about server.
            using (var SClient = new StatusClient(ServerIP, ServerPort))
                ServerData = SClient.GetServerInfo(5);


            Client = new Minecraft("TestBot", "", false);

             /*
             Connect()
             Send new HandshakePacket       (5, localhost, 25565, Login)
             Send new LoginStartPacket      (Username)
             Send new ClientSettingsPacket  (en_GB, 15, 0, true, Normal, true)
             Send new PluginMessagePacket   (MC|Brand, ClientBrand) 
             */

            Client.Connect(ServerIP, ServerPort);

            while (!Client.Connected) { }

            Client.SendPacket(new HandshakePacket
            {
                ProtocolVersion = 5,
                ServerAddress = ServerIP,
                ServerPort = ServerPort,
                NextState = NextState.Login,
            });

            Client.SendPacket(new LoginStartPacket { Name = "TestBot" });

            while (Client.State != ServerState.Play) { }

            Client.SendPacket(new ClientSettingsPacket
            {
                Locale = "en_GB",
                ViewDistance = 15,
                ChatFlags = 0, // Nope.
                ChatColours = true,
                Difficulty = Difficulty.Normal,
                ShowCape = true
            });

            Client.SendPacket(new PluginMessagePacket
            {
                Channel = "MC|Brand",
                Data = Encoding.UTF8.GetBytes("MineLib.Net")
            });

            Client.SendPacket(new ClientStatusPacket { Status = ClientStatus.Respawn });

            while (true) { }
        }
    }
}