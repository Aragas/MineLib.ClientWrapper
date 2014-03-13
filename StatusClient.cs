using System;
using System.Drawing;
using System.IO;
using MineLib.ClientWrapper.Data;
using MineLib.Network;
using MineLib.Network.Enums;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Client.Status;
using MineLib.Network.Packets.Server.Status;

namespace MineLib.ClientWrapper
{
    public struct ResponseData
    {
        public int Ping;
        public ServerInfo Info;
    }

    public partial class StatusClient : IMinecraft, IDisposable
    {
        #region Variables
        private string _serverIp;
        public string ServerIP { get { return _serverIp; } set { _serverIp = value; } }

        private short _serverPort;
        public short ServerPort { get { return _serverPort; } set { _serverPort = value; } }

        private ServerState _serverState;
        public ServerState State { get { return _serverState; } set { _serverState = value; } }

        private string _accessToken;
        public string AccessToken { get { return _accessToken; } set { _accessToken = value; } }


        private string _selectedProfile;
        public string SelectedProfile { get { return _selectedProfile; } set { _selectedProfile = value; } }

        private bool _running;
        public bool Running { get { return _running; } set { _running = value; } }
        #endregion Variables

        private bool ConnectionClosed;

        private readonly NetworkHandler _handler;

        public StatusClient(string ip, short port)
        {
            State = ServerState.Status;

            ServerIP = ip;
            ServerPort = port;

            _handler = new NetworkHandler(this);

            // -- Register our event handlers.
            _handler.OnPacketHandled += RaisePacketHandled;

            // -- Connect to the server and begin reading packets.
            _handler.Start();
        }

        public ResponseData GetServerInfo(int protocolVersion)
        {
            if (ConnectionClosed) throw new Exception("Initialize new StatusClient");

            bool Ready = false;

            int Time = 0;
            int time = 0;
            int currtime = 0;

            ServerInfo Info = new ServerInfo();

            FireResponsePacket += packet => ParseResponse(packet, ref Info);

            FirePingPacket += packet =>
            {
                MineLib.Network.Packets.Server.Status.PingPacket Ping =
                    (MineLib.Network.Packets.Server.Status.PingPacket)packet;
                currtime = DateTime.UtcNow.Millisecond;
                Time = currtime - time;
                Ready = true;
            };

            PingServer(protocolVersion, out time);

            while (!Ready) { }
            Disconnect();
            return new ResponseData { Info = Info, Ping = Time };
        }

        public ServerInfo GetInfo(int protocolVersion)
        {
            if (ConnectionClosed) throw new Exception("Initialize new StatusClient");

            bool Ready = false;

            ServerInfo Info = new ServerInfo();

            FireResponsePacket += packet => { ParseResponse(packet, ref Info); Ready = true; };

            ServerInfo(protocolVersion);

            while (!Ready) { }
            Disconnect();
            return Info;
        }

        public int GetPing(int protocolVersion)
        {
            if (ConnectionClosed) throw new Exception("Initialize new StatusClient");

            bool Ready = false;

            int Time = 0;
            int time = 0;
            int currtime = 0;

            FirePingPacket += packet =>
            {
                MineLib.Network.Packets.Server.Status.PingPacket Ping =
                    (MineLib.Network.Packets.Server.Status.PingPacket)packet;
                currtime = DateTime.UtcNow.Millisecond;
                Time = currtime - time;
                Ready = true;
            };

            PingServer(protocolVersion, out time);

            while (!Ready) {}
            Disconnect();
            return Time;
        }

        private void ServerInfo(int protocolVersion)
        {
            SendPacket(new HandshakePacket
            {
                ServerAddress = ServerIP,
                ServerPort = ServerPort,
                ProtocolVersion = 4,
                NextState = NextState.Status
            });

            SendPacket(new RequestPacket());
        }

        private void PingServer(int protocolVersion, out int ping)
        {
            SendPacket(new HandshakePacket
            {
                ServerAddress = ServerIP,
                ServerPort = ServerPort,
                ProtocolVersion = 4,
                NextState = NextState.Status
            });

            SendPacket(new RequestPacket());

            ping = DateTime.UtcNow.Millisecond;

            SendPacket(new MineLib.Network.Packets.Client.Status.PingPacket { Time = ping });
        }

        private void ParseResponse(IPacket packet, ref ServerInfo info)
        {
            // Very dirty, yep.

            ResponsePacket Response = (ResponsePacket)packet;
            string Text = Response.Response;

            string[] temp;
            string description = "";
            string max = "";
            string online = "";
            string name = "";
            string protocol = "";
            string favicon = "";
            Image FIcon = null;

            temp = Text.Split(new string[] { "description\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2) { description = temp[1].Split('"')[0]; }

            temp = Text.Split(new string[] { "max\":" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2) { max = temp[1].Split(',')[0].Split('}')[0]; }

            temp = Text.Split(new string[] { "online\":" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2) { online = temp[1].Split(',')[0].Split('}')[0]; }

            temp = Text.Split(new string[] { "name\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2) { name = temp[1].Split('"')[0]; }

            temp = Text.Split(new string[] { "protocol\":" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2) { protocol = temp[1].Split('}')[0]; }

            temp = Text.Split(new string[] { "favicon\":" }, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length >= 2) { favicon = temp[1].Split(',')[1].Replace("\\u003d", "=").Split('\"')[0]; }

            // Big problems with base64 decoding.
            try
            {
                byte[] data = Convert.FromBase64String(favicon);

                using (MemoryStream ms = new MemoryStream(data))
                {
                    FIcon = Image.FromStream(ms);
                }

            }
            catch (Exception) { }

            info = new ServerInfo
            {
                Description = description,
                Players = new Players { Max = Int32.Parse(max), Online = Int32.Parse(online) },
                Version = new ServerVersion { Name = name, Protocol = Int32.Parse(protocol) },
                Favicon = FIcon
            };
        }

        private void SendPacket(IPacket packet)
        {
            if (_handler != null)
                _handler.Send(packet);
        }

        private void Disconnect()
        {
            if (_handler != null)
                _handler.Stop();

            ConnectionClosed = true;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
