using System;
using System.Collections.Generic;
using MineLib.ClientWrapper.BigData;
using MineLib.Network;
using MineLib.Network.Enums;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Server;

namespace MineLib.ClientWrapper
{
    /// <summary>
    /// Wrapper for Network of MineLib.Net.
    /// </summary>
    public partial class Minecraft : IMinecraft, IDisposable
    {
        public string ServerIP { get; set; }

        public string ClientName, ClientPassword, AccessToken, ClientToken, SelectedProfile, ClientBrand;

        public int ServerPort { get; set; }

        public ServerState State { get; set; }

        public bool VerifyNames;

        public bool Running { get; set; }

        public NetworkHandler nh;

        public World World; // -- Holds all of the world information. Time, chunks, ect.
        public ThisPlayer Player;
        //public Player ThisPlayer; // -- Holds all user information, location, inventory and so on.
        public Dictionary<string, short> PlayerList;
        public Dictionary<int, Entity> Entities;
        private string _serverIp;
        private int _serverPort;
        private bool _running;
        private ServerState _serverState;

        /// <summary>
        /// Create a new Minecraft Instance
        /// </summary>
        /// <param name="username">The username to use when connecting to Minecraft</param>
        /// <param name="password">The password to use when connecting to Minecraft (Ignore if you are providing credentials)</param>
        /// <param name="nameVerification">To connect using Name Verification or not</param>
        public Minecraft(string username, string password, bool nameVerification = false)
        {
            ClientName = username;
            ClientPassword = password;
            VerifyNames = nameVerification;
            ClientBrand = "MineLib.Net"; // -- Used in the plugin message reporting the client brand to the server.
        }

        /// <summary>
        /// Login to Minecraft.net and store credentials
        /// </summary>
        public void Login()
        {
            if (VerifyNames)
            {
                YggdrasilStatus result = Yggdrasil.LoginAuthServer(ref ClientName, ClientPassword, ref AccessToken,
                    ref ClientToken, ref SelectedProfile);

                switch (result)
                {
                    case YggdrasilStatus.Success:
                        break;

                    default:
                        VerifyNames = false; // -- Fall back to no auth.
                        break;
                }
            }
            else
            {
                AccessToken = "None";
                SelectedProfile = "None";
            }

        }

        /// <summary>
        /// Uses a client's stored credentials to verify with Minecraft.net
        /// </summary>
        public bool RefreshSession()
        {
            if (AccessToken == null || ClientToken == null)
                return false;


            YggdrasilStatus result = Yggdrasil.RefreshSession(ref AccessToken, ref ClientToken);

            switch (result)
            {
                case YggdrasilStatus.Success:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Uses a client's stored credentials to verify with Minecraft.net
        /// </summary>
        /// <param name="accessToken">Stored Access Token</param>
        /// <param name="clientToken">Stored Client Token</param>
        public bool RefreshSession(string accessToken, string clientToken)
        {
            AccessToken = accessToken;
            ClientToken = clientToken;

            if (AccessToken == null || ClientToken == null)
                return false;


            YggdrasilStatus result = Yggdrasil.RefreshSession(ref AccessToken, ref ClientToken);

            switch (result)
            {
                case YggdrasilStatus.Success:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Connects to the Minecraft Server.
        /// </summary>
        /// <param name="ip">The IP of the server to connect to</param>
        /// <param name="port">The port of the server to connect to</param>
        public void Connect(string ip, int port)
        {
            ServerIP = ip;
            ServerPort = port;

            if (nh != null)
                Disconnect();

            World = new World();
            Player = new ThisPlayer();
            PlayerList = new Dictionary<string, short>();
            Entities = new Dictionary<int, Entity>(); 

            nh = new NetworkHandler(this);

            // -- Register our event handlers.
            nh.OnPacketHandled += RaisePacketHandled;

            // -- Connect to the server and begin reading packets.
            nh.Start();
        }

        /// <summary>
        /// Send IPacket to the Minecraft Server.
        /// </summary>
        /// <param name="packet">IPacket to sent to server</param>
        public void SendPacket(IPacket packet)
        {
            if (nh != null)
                nh.Send(packet);
        }

        public void SendChatMessage(string message)
        {
            nh.Send(new ChatMessagePacket {Message = message});
        }

        /// <summary>
        /// Disconnects from the Minecraft server.
        /// </summary>
        public void Disconnect()
        {
            if (nh != null)
                nh.Stop();

            // -- Reset all variables to default so we can make a new connection.

            Running = false;
            State = ServerState.Login;

            World = null;
            Player = null;
            PlayerList = null;
            Entities = null;
        }

        public void Dispose()
        {
            Disconnect();
        }

    }
}
