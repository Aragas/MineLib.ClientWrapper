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
            string ServerIP = "127.0.0.1"; //args[0];                  // localhost
            short ServerPort = short.Parse(args[1]);    // 25565

            Client = new Minecraft("TestBot", "", false);

            //Client.RefreshSession();
            //Client.VerifySession();

            /*
             Connect()
             Send new HandshakePacket       (5, localhost, 25565, Login)
             Send new LoginStartPacket      (Username)
             Send new PluginMessagePacket   (MC|Brand, ClientBrand)
             
             */

            using (var SClient = new StatusClient(ServerIP, ServerPort))
                ServerData = SClient.GetServerInfo(5);
            

            Client.Connect(ServerIP, ServerPort);

            while (!Client.Connected) { }


            Client.SendPacket(new HandshakePacket
            {
                ProtocolVersion = 5,
                ServerAddress = ServerIP,
                ServerPort = ServerPort,
                NextState = NextState.Login,
            });

            Client.SendPacket(new LoginStartPacket {Name = "TestBot"});

            while (Client.State != ServerState.Play) {}

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

            Client.SendPacket(new ClientStatusPacket {Status = ClientStatus.Respawn});
   
            
            //Client.SendPacket(new PlayerLookPacket
            //{
            //    Yaw = 45,
            //    Pitch = 45,
            //    OnGround = true
            //});

            //int look = Convert.ToInt32(Console.ReadLine());

            //Client.SendPacket(new PlayerLookPacket
            //{
            //    Yaw = look,
            //    Pitch = look,
            //    OnGround = true
            //});

            while (true) { }
        }
    }
}