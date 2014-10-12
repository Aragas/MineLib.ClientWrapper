using System;
using System.Collections.Generic;
using MineLib.ClientWrapper.Modern;
using MineLib.ClientWrapper.Modern.BigData;
using MineLib.Network;
using MineLib.Network.Modern.Packets.Client;

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

        public World ModernWorld;
        public Player ModernPlayer;
        public Dictionary<int, Entity> ModernEntities;
        public Dictionary<string, short> ModernPlayersList;
        public PlayerTickHandler ModernPlayerHandler;

        /// <summary>
        /// Create a new Minecraft Instance
        /// </summary>
        /// <param name="login">The username to use when connecting to Minecraft</param>
        /// <param name="password">The password to use when connecting to Minecraft (Ignore if you are providing credentials)</param>
        /// <param name="nameVerification">To connect using Name Verification or not</param>
        /// <param name="classic">Classic mode</param>
        /// <param name="serverSalt"></param>
        public Minecraft(string login, string password, NetworkMode mode, bool nameVerification = false, string serverSalt = null)
        {
            ClientLogin = login;
            ClientPassword = password;
            VerifyNames = nameVerification;
            Mode = mode;
            ServerSalt = serverSalt;

            switch (Mode)
            {
                case NetworkMode.Modern:
                    State = ServerState.ModernLogin;
                    break;

                case NetworkMode.Classic:
                    State = ServerState.ClassicLogin;
                    break;
            }

            if (VerifyNames)
            {
                switch (Mode)
                {
                    case NetworkMode.Modern:
                        ModernLogin();
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
            ModernPlayerHandler = new PlayerTickHandler(this);
            ModernPlayerHandler.Start();
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

            Handler = new NetworkHandler(this, Mode);

            switch (Mode)
            {
                case NetworkMode.Modern:
                    ModernWorld = new World();
                    ModernPlayer = new Player();
                    ModernEntities = new Dictionary<int, Entity>();
                    ModernPlayersList = new Dictionary<string, short>();

                    // -- Register our event handlers.
                    Handler.OnPacketHandled += RaisePacketHandled;
                    break;

                case NetworkMode.Classic:
                    // -- Register our event handlers.
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
            State = ServerState.ModernLogin;

            ModernWorld = null;
            ModernPlayer = null;
            ModernPlayersList = null;
            ModernEntities = null;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}