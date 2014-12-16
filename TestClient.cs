using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using MineLib.ClientWrapper.Graphic;
using MineLib.Network;
using MineLib.Network.Data;
using MineLib.Network.Data.Anvil;
using MineLib.Network.Data.Structs;

namespace MineLib.ClientWrapper
{
    public static class TestClient
    {
        public static Minecraft Client;
        //public static ResponseData ServerData;

        public static void Main(string[] args)
        {
            string ServerIP = "127.0.0.1";  //args[0];                 // localhost
            ushort ServerPort = 25565;      //short.Parse(args[1]);    // 25565

            // Gettin' information about the server.
            //using (var SClient = new ServerInfoParser())
            //    ServerData = SClient.GetResponseData(ServerIP, ServerPort, 47);

            Client = new Minecraft().Create("TestBot", "", NetworkMode.ProtocolModern, true) as Minecraft;
            Client.BeginConnect(ServerIP, ServerPort, OnConnected, null);

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