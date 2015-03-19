using System;
using System.Runtime.InteropServices;
using System.Threading;
using MineLib.Network;
using MineLib.Network.Data.Anvil;

namespace MineLib.ClientWrapper
{
    public static class TestClient
    {
        public static Minecraft Client;
        //public static ResponseData ServerData;

        public static void Main(string[] args)
        {
            const string serverIp = "127.0.0.1"; //args[0];                 // localhost
            const ushort serverPort = 25565; //short.Parse(args[1]);    // 25565

            // Gettin' information about the server.
            //using (var SClient = new ServerInfoParser())
            //    ServerData = SClient.GetResponseData(ServerIP, ServerPort, 47);

            Client = new Minecraft().Initialize("TestBot", "", NetworkMode.Module, true) as Minecraft;
            Client?.BeginConnect(serverIp, serverPort, OnConnected, null);

            //Block b = new Block(0);
            //int t = Marshal.SizeOf(typeof(Block));

            while (true) { Thread.Sleep(50); }
        }

        private static void OnConnected(IAsyncResult ar)
        {
            Client.BeginConnectToServer(OnJoinedServer, null);
        }

        private static void OnJoinedServer(IAsyncResult ar)
        {
            while (Client.ConnectionState != ConnectionState.JoinedServer) { Thread.Sleep(250); }

            Client.BeginSendClientInfo(null, null);
        }
    }
}