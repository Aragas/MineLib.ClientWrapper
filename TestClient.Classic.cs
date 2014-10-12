using System.Threading;
using MineLib.Network;
using MineLib.Network.Classic.Packets.Client;

namespace MineLib.ClientWrapper
{
    public static class TestClientClassic
    {
        public static Minecraft Client;

        public static void Main(string[] args)
        {
            string ServerIP = "127.0.0.1";  //args[0];                 // localhost
            short ServerPort = 25565;       //short.Parse(args[1]);    // 25565

            Client = new Minecraft("TestBot", "", NetworkMode.Classic, false);

            /*
            Connect()
            Send new PlayerIdentificationPacket (0x07, Login, VerKey, 0x42)
            // 0x42 Ext support
            */

            Client.Connect(ServerIP, ServerPort);

            while (!Client.Connected) { Thread.Sleep(250); }

            Client.SendPacket(new PlayerIdentificationPacket
            {
                ProtocolVersion = 0x07,
                Username = "TestBot",
                VerificationKey = Client.AccessToken,
                UnUsed = 0x42
            });

            while (Client.State != ServerState.ClassicLogin) { Thread.Sleep(250); }



            while (true) { Thread.Sleep(50); }
        }
    }
}