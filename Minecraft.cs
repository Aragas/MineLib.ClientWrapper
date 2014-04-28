using System;
using System.Linq;
using System.Collections.Generic;
using MineLib.ClientWrapper.BigData;
using MineLib.Network;
using MineLib.Network.Enums;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Client;

namespace MineLib.ClientWrapper
{
    /// <summary>
    ///     Wrapper for Network of MineLib.Net.
    /// </summary>
    public partial class Minecraft : IMinecraftClient, IDisposable
    {
        // -- Debugging
        public List<IPacket> LastPackets
        {
            get
            {
                try
                {
                    return Packets.Skip(Math.Max(0, Packets.Count() - 50)).Take(50).ToList();
                }
                catch { }
                return null;
            }
        }
        public IPacket LastPacket { get { return Packets[Packets.Count - 1]; } }
        // -- Debugging

        #region Variables

        public string AccessToken { get; set; }

        public string ClientName { get; set; }

        public string ClientToken { get; set; }

        public string SelectedProfile { get; set; }

        public string ClientPassword { get; set; }

        public string ClientBrand { get; set; }

        public string ServerBrand { get; set; }

        public bool VerifyNames { get; set; }

        public string ServerIP { get; set; }

        public short ServerPort { get; set; }

        public ServerState State { get; set; }

        #endregion Variables

        public bool Connected { get { return Handler.Connected; }}

        public NetworkHandler Handler;
        public PlayerHandler PlayerHandler;

        public World World;
        public Player Player;
        public Dictionary<int, Entity> Entities;
        public Dictionary<string, short> PlayersList;

        /// <summary>
        ///     Create a new Minecraft Instance
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

            if(VerifyNames)
                Login();
        }

        /// <summary>
        ///     Connects to the Minecraft Server.
        /// </summary>
        /// <param name="ip">The IP of the server to connect to</param>
        /// <param name="port">The port of the server to connect to</param>
        public void Connect(string ip, short port)
        {
            ServerIP = ip;
            ServerPort = port;

            if (Handler != null)
                Disconnect();

            World = new World();
            Player = new Player();
            Entities = new Dictionary<int, Entity>();
            PlayersList = new Dictionary<string, short>();

            Handler = new NetworkHandler(this);
            //PlayerHandler = new PlayerHandler(ref Handler, ref Player);

            // -- Register our event handlers.
            Handler.OnPacketHandled += RaisePacketHandled;
            Handler.OnPacketHandledClassic += RaisePacketHandledClassic;

            // -- Connect to the server and begin reading packets.
            Handler.Start();
        }

        /// <summary>
        ///     Send IPacket to the Minecraft Server.
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
        ///     Disconnects from the Minecraft server.
        /// </summary>
        public void Disconnect()
        {
            if (Handler != null)
                Handler.Dispose();

            // -- Reset all variables to default so we can make a new connection.
            State = ServerState.Login;

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