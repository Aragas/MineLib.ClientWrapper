using System;
using System.Collections.Generic;
using MineLib.ClientWrapper.Data;
using MineLib.ClientWrapper.Data.BigData;
using MineLib.Network;

namespace MineLib.ClientWrapper
{
    /// <summary>
    /// Wrapper for Network of MineLib.Net.
    /// </summary>
    public partial class Minecraft : IMinecraftClient
    {
        #region Properties

        public string AccessToken { get; set; }

        public string ClientLogin { get; set; }
        private string _clientUsername;

        public string ClientUsername
        {
            get { return _clientUsername ?? ClientLogin; } 
            set { _clientUsername = value; }
        }
        public string ClientPassword { get; set; }
        public bool UseLogin { get; private set; }

        public string ClientToken { get; set; }

        public string SelectedProfile { get; set; }

        public string PlayerName { get; set; }
        public string ClientBrand
        {
            get { return "MineLib.Network";}
            set { throw new NotImplementedException(); }
        }

        public string ServerBrand { get; set; }

        public string ServerHost { get; set; }

        public ushort ServerPort { get; set; }

        public string ServerSalt { get; set; }

        public string ServerName { get; set; }

        public string ServerMOTD { get; set; }

        public NetworkMode Mode { get; private set; }
        public ConnectionState ConnectionState => _networkHandler.ConnectionState;

        public bool Connected => _networkHandler.Connected;

        #endregion Properties

        public bool ReducedDebugInfo;

        public World World;
        public Player Player;
        public Dictionary<int, Entity> Entities;
        public Dictionary<string, short> PlayersList;
        public PlayerTickHandler PlayerHandler;

        private INetworkHandler _networkHandler;

        /// <summary>
        /// Create a new Minecraft Instance
        /// </summary>
        /// <param name="login">The username to use when connecting to Minecraft</param>
        /// <param name="password">The password to use when connecting to Minecraft (Ignore if you are providing credentials)</param>
        /// <param name="mode"></param>
        /// <param name="nameVerification">To connect using Name Verification or not</param>
        /// <param name="serverSalt"></param>
        public IMinecraftClient Initialize(string login, string password, NetworkMode mode, bool nameVerification = false, string serverSalt = null)
        {
            ClientLogin = login;
            ClientPassword = password;
            UseLogin = nameVerification;
            Mode = mode;
            ServerSalt = serverSalt;

            AsyncReceiveHandlers = new Dictionary<Type, Action<IAsyncReceive>>();
            RegisterSupportedReceiveEvents();

            World = new World();
            Player = new Player();
            Entities = new Dictionary<int, Entity>();
            PlayersList = new Dictionary<string, short>();

            _networkHandler = new NetworkHandler();
            var modules = _networkHandler.GetModules();
            _networkHandler.Initialize(modules[1], this, true);

            return this;
        }

        private void StartPlayerTickHandler()
        {
            PlayerHandler = new PlayerTickHandler(this);
            PlayerHandler.Start();
        }

        /// <summary>
        /// Connects to the Minecraft Server. If connected, don't call EndConnect.
        /// </summary>
        /// <param name="ip">The IP of the server to connect to</param>
        /// <param name="port">The port of the server to connect to</param>
        public IAsyncResult BeginConnect(string ip, ushort port, AsyncCallback asyncCallback, object state)
        {
            ServerHost = ip;
            ServerPort = port;

            // -- Connect to the server and begin reading packets.
            return _networkHandler.BeginConnect(ip, port, asyncCallback, state);
        }

        private void EndConnect(IAsyncResult asyncResult)
        {
            //_networkHandler.EndConnect(asyncResult);
        }

        public IAsyncResult BeginDisconnect(AsyncCallback asyncCallback, object state)
        {
            return _networkHandler.BeginDisconnect(asyncCallback, state);
        }

        public void EndDisconnect(IAsyncResult asyncResult)
        {
            _networkHandler.EndDisconnect(asyncResult);
        }


        /// <summary>
        /// Connects to the Minecraft Server.
        /// </summary>
        /// <param name="ip">The IP of the server to connect to</param>
        /// <param name="port">The port of the server to connect to</param>
        public void Connect(string ip, ushort port)
        {
            ServerHost = ip;
            ServerPort = port;

            // -- Connect to the server and begin reading packets.
            _networkHandler.Connect(ip, port);
        }

         /// <summary>
        /// Disconnects from the Minecraft server.
        /// </summary>
        public void Disconnect()
        {
            _networkHandler.Disconnect();
        }


        public void Dispose()
        {
            _networkHandler?.Dispose();
        }
    }
}