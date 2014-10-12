using System.Text;
using System.Threading;
using MineLib.Network;
using MineLib.Network.Main.BaseClients;
using MineLib.Network.Main.Data;
using MineLib.Network.Main.Enums;
using MineLib.Network.Main.Packets;
using MineLib.Network.Main.Packets.Client;
using MineLib.Network.Main.Packets.Client.Login;

namespace MineLib.ClientWrapper
{
    public static class TestClientMain
    {
        public static Minecraft Client;
        public static ResponseData ServerData;

        public static void Main(string[] args)
        {
            string ServerIP = "127.0.0.1";  //args[0];                 // localhost
            short ServerPort = 25565;       //short.Parse(args[1]);    // 25565

            // Gettin' information about the server.
            using (var SClient = new ServerInfoParser())
                ServerData = SClient.GetResponseData(ServerIP, ServerPort, 47);


            Client = new Minecraft("TestBot", "", false, NetworkMode.Main);

            /*
            Connect()
            Send new HandshakePacket       (5, localhost, 25565, Login)
            Send new LoginStartPacket      (Username)
            Send new ClientSettingsPacket  (en_GB, 15, 0, true, Normal, true)
            Send new PluginMessagePacket   (MC|Brand, ClientBrand) 
            */

            Client.Connect(ServerIP, ServerPort);

            while (!Client.Connected) { Thread.Sleep(250); }

            Client.SendPacket(new HandshakePacket
            {
                ProtocolVersion = 47,
                ServerAddress = ServerIP,
                ServerPort = ServerPort,
                NextState = NextState.Login,
            });

            Client.SendPacket(new LoginStartPacket { Name = "TestBot" });

            while (Client.State != ServerState.MainPlay) { Thread.Sleep(250); }

            Client.SendPacket(new ClientSettingsPacket
            {
                Locale = "en_GB",
                ViewDistance = 15,
                ChatFlags = ChatFlags.Enabled,
                ChatColours = true,
                DisplayedSkinParts = new DisplayedSkinParts
                    {
                        CapeEnabled = true,
                        JackedEnabled = true,
                        LeftSleeveEnabled = true,
                        RightSleeveEnabled = true,
                        LeftPantsEnabled = true,
                        RightPantsEnabled = true,
                        HatEnabled = true,
                        Unused = false
                    }
            });

            Client.SendPacket(new PluginMessagePacket
            {
                Channel = "MC|Brand",
                Data = Encoding.UTF8.GetBytes("MineLib.Network")
            });

            Client.SendPacket(new ClientStatusPacket { Status = ClientStatus.Respawn });

            while (true) { Thread.Sleep(50); }
        }
    }
}