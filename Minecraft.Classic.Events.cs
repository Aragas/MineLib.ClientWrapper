using MineLib.Network;
using MineLib.Network.Classic.Packets.Extension.Client;
using MineLib.Network.Classic.Packets.Server;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        private void OnServerIdentificationClassic(IPacket packet)
        {
            var serverIdentification = (ServerIdentificationPacket)packet;

            ServerName = serverIdentification.ServerName;
            ServerMOTD = serverIdentification.ServerMOTD;
            //UserType = (UserType) serverIdentification.UserType;
        }

        private void OnPingClassic(IPacket packet)
        {
            var ping = (PingPacket)packet;
        }

        private void OnLevelInitializeClassic(IPacket packet)
        {
            var levelInitialize = (LevelInitializePacket)packet;
        }

        private void OnLevelDataChunkClassic(IPacket packet)
        {
            var levelDataChunk = (LevelDataChunkPacket)packet;
        }

        private void OnLevelFinalizeClassic(IPacket packet)
        {
            var levelFinalize = (LevelFinalizePacket)packet;
        }

        private void OnSetBlockClassic(IPacket packet)
        {
            var setBlock = (SetBlockPacket)packet;
        }

        private void OnSpawnPlayerClassic(IPacket packet)
        {
            var spawnPlayer = (SpawnPlayerPacket)packet;
        }

        private void OnPositionAndOrientationTeleportClassic(IPacket packet)
        {
            var positionAndOrientationTeleport = (PositionAndOrientationTeleportPacket)packet;
        }

        private void OnPositionAndOrientationUpdateClassic(IPacket packet)
        {
            var positionAndOrientationUpdate = (PositionAndOrientationUpdatePacket)packet;
        }

        private void OnPositionUpdateClassic(IPacket packet)
        {
            var positionUpdate = (PositionUpdatePacket)packet;
        }

        private void OnOrientationUpdateClassic(IPacket packet)
        {
            var orientationUpdate = (OrientationUpdatePacket)packet;
        }

        private void OnDespawnPlayerClassic(IPacket packet)
        {
            var despawnPlayer = (DespawnPlayerPacket)packet;
        }

        private void OnMessageClassic(IPacket packet)
        {
            var message = (MessagePacket)packet;
        }

        private void OnDisconnectPlayerClassic(IPacket packet)
        {
            var disconnectPlayer = (DisconnectPlayerPacket)packet;
        }

        private void OnUpdateUserTypeClassic(IPacket packet)
        {
            var updateUserType = (UpdateUserTypePacket)packet;
        }

        private void OnExtInfoClassic(IPacket packet)
        {
            var extInfo = (ExtInfoPacket) packet;
        }

        private void OnExtEntryTypeClassic(IPacket packet)
        {
            var extEntry = (ExtEntryPacket) packet;

            SendPacket(new ExtInfoPacket
            {
                AppName = ClientBrand,
                ExtensionCount = 0,
            });

            SendPacket(new ExtEntryPacket
            {
                ExtName = null,
                Version = 0
            });
        }
    }
}
