using System;
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
    public partial class Minecraft : IMinecraft, IDisposable
    {
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

        public Dictionary<int, Entity> Entities;
        public NetworkHandler Handler;

        public ThisPlayer Player; // -- Holds all user information, location, inventory and so on.
        public Dictionary<string, short> PlayerList;
        public World World; // -- Holds all of the world information. Time, chunks, ect.

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
        ///     Login to Minecraft.net and store credentials
        /// </summary>
        public void Login()
        {
            if (VerifyNames)
            {
                var result = Yggdrasil.Login(ClientName, ClientPassword);

                switch (result.Status)
                {
                    case YggdrasilStatus.Success:
                        AccessToken = result.Response.AccessToken;
                        ClientToken = result.Response.ClientToken;
                        SelectedProfile = result.Response.Profile.ID;
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
        ///     Uses a client's stored credentials to verify with Minecraft.net
        /// </summary>
        public bool RefreshSession()
        {
            if (AccessToken == null || ClientToken == null)
                return false;


            var result = Yggdrasil.RefreshSession(AccessToken, ClientToken);

            switch (result.Status)
            {
                case YggdrasilStatus.Success:
                    AccessToken = result.Response.AccessToken;
                    ClientToken = result.Response.ClientToken;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        ///     Uses a client's stored credentials to verify with Minecraft.net
        /// </summary>
        /// <param name="accessToken">Stored Access Token</param>
        /// <param name="clientToken">Stored Client Token</param>
        public bool RefreshSession(string accessToken, string clientToken)
        {
            AccessToken = accessToken;
            ClientToken = clientToken;

            if (AccessToken == null || ClientToken == null)
                return false;


            var result = Yggdrasil.RefreshSession(AccessToken, ClientToken);

            switch (result.Status)
            {
                case YggdrasilStatus.Success:
                    AccessToken = result.Response.AccessToken;
                    ClientToken = result.Response.ClientToken;
                    return true;

                default:
                    return false;
            }
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
            Player = new ThisPlayer();
            PlayerList = new Dictionary<string, short>();
            Entities = new Dictionary<int, Entity>();

            Handler = new NetworkHandler(this);

            // -- Register our event handlers.
            Handler.OnPacketHandled += RaisePacketHandled;

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
                Handler.Stop();

            // -- Reset all variables to default so we can make a new connection.

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