using System;
using System.Collections.Generic;
using MineLib.ClientWrapper.BigData;
using MineLib.Network;
using MineLib.Network.Main.Packets.Client;

namespace MineLib.ClientWrapper
{
    /// <summary>
    /// Wrapper for Network of MineLib.Net.
    /// </summary>
    public partial class Minecraft : IMinecraftClient, IDisposable
    {
        // -- Debugging
        public List<IPacket> LastPackets
        {
            get
            {
                try { return Handler.PacketsReceived.GetRange(Handler.PacketsReceived.Count - 50, 50); }
                catch { return null; }
            }
        }
        public IPacket LastPacket { get { return Handler.PacketsReceived[Handler.PacketsReceived.Count - 1]; } }
        // -- Debugging

        #region Variables

        public string AccessToken { get; set; }

        public string ClientLogin { get; set; }

        private string _clientUsername;
        public string ClientUsername { get { return _clientUsername ?? ClientLogin; } }

        public string ClientToken { get; set; }

        public string SelectedProfile { get; set; }

        public string ClientPassword { get; set; }

        public string ClientBrand { get { return "MineLib.Network";} }

        public string ServerBrand { get; set; }

        public bool VerifyNames { get; set; }

        public string ServerHost { get; set; }

        public short ServerPort { get; set; }

        public string ServerSalt { get; set; }

        public string ServerName { get; set; }

        public string ServerMOTD { get; set; }

        public ServerState State { get; set; }

        public NetworkMode Mode { get; set; }

        #endregion Variables

        public bool Connected { get { return Handler.Connected; }}

        public bool Crashed { get { return Handler.Crashed; } }

        public bool ReducedDebugInfo;

        public NetworkHandler Handler;
        public PlayerTickHandler PlayerHandler;

        public World World;
        public Player Player;
        public Dictionary<int, Entity> Entities;
        public Dictionary<string, short> PlayersList;

        /// <summary>
        /// Create a new Minecraft Instance
        /// </summary>
        /// <param name="login">The username to use when connecting to Minecraft</param>
        /// <param name="password">The password to use when connecting to Minecraft (Ignore if you are providing credentials)</param>
        /// <param name="nameVerification">To connect using Name Verification or not</param>
        /// <param name="classic">Classic mode</param>
        /// <param name="serverSalt"></param>
        public Minecraft(string login, string password, bool nameVerification = false, NetworkMode mode = NetworkMode.Main, string serverSalt = null)
        {
            ClientLogin = login;
            ClientPassword = password;
            VerifyNames = nameVerification;
            Mode = mode;
            ServerSalt = serverSalt;

            switch (Mode)
            {
                case NetworkMode.Main:
                    State = ServerState.MainLogin;
                    break;

                case NetworkMode.Classic:
                    State = ServerState.ClassicLogin;
                    break;
            }

            if (VerifyNames)
            {
                switch (Mode)
                {
                    case NetworkMode.Main:
                        MainLogin();
                        break;

                    case NetworkMode.Classic:
                        ClassicLogin();
                        break;
                }
            }
            else
            {
                AccessToken = "None";
                SelectedProfile = "None";
            }
        }

        private void StartPlayerTickHandler()
        {
            PlayerHandler = new PlayerTickHandler(this);
            PlayerHandler.Start();
        }

        /// <summary>
        /// Connects to the Minecraft Server.
        /// </summary>
        /// <param name="ip">The IP of the server to connect to</param>
        /// <param name="port">The port of the server to connect to</param>
        /// <param name="serverHash">Classic server hash</param>
        public void Connect(string ip, short port)
        {
            ServerHost = ip;
            ServerPort = port;

            if (Handler != null)
                Disconnect();

            World = new World();
            Player = new Player();
            Entities = new Dictionary<int, Entity>();
            PlayersList = new Dictionary<string, short>();

            Handler = new NetworkHandler(this, Mode);

            // -- Register our event handlers.
            switch (Mode)
            {
                case NetworkMode.Main:
                    Handler.OnPacketHandled += RaisePacketHandled;
                    break;

                case NetworkMode.Classic:
                    Handler.OnPacketHandled += RaisePacketHandledClassic;
                    break;
            }

            // -- Connect to the server and begin reading packets.
            Handler.Start();
        }

        /// <summary>
        /// Send IPacket to the Minecraft Server.
        /// </summary>
        /// <param name="packet">IPacket to sent to server</param>
        public void SendPacket(IPacket packet)
        {
            if (Handler != null && Connected)
                Handler.Send(packet);
        }

        public void SendChatMessage(string message)
        {
            if (Handler != null && Connected)
                Handler.Send(new ChatMessagePacket {Message = message});
        }

        /// <summary>
        /// Disconnects from the Minecraft server.
        /// </summary>
        public void Disconnect()
        {
            if (Handler != null)
                Handler.Dispose();

            // -- Reset all variables to default so we can make a new connection.
            State = ServerState.MainLogin;

            World = null;
            Player = null;
            PlayersList = null;
            Entities = null;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}