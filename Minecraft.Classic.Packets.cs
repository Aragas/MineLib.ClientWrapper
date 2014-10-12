using System;
using MineLib.Network;
using MineLib.Network.Classic.Enums;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        private void RaisePacketHandledClassic(int id, IPacket packet, ServerState? state)
        {
            // -- Debugging
            Console.WriteLine("Classic ID: 0x" + String.Format("{0:X}", id));
            Console.WriteLine(" ");
            // -- Debugging

            switch ((PacketsServer) id)
            {
                case PacketsServer.ServerIdentification:
                    State = ServerState.ClassicPlay;
                    OnServerIdentificationClassic(packet);
                    break;

                case PacketsServer.Ping:
                    OnPingClassic(packet);
                    break;

                case PacketsServer.LevelInitialize:
                    OnLevelInitializeClassic(packet);
                    break;

                case PacketsServer.LevelDataChunk:
                    OnLevelDataChunkClassic(packet);
                    break;

                case PacketsServer.LevelFinalize:
                    OnLevelFinalizeClassic(packet);
                    break;

                case PacketsServer.SetBlock:
                    OnSetBlockClassic(packet);
                    break;

                case PacketsServer.SpawnPlayer:
                    OnSpawnPlayerClassic(packet);
                    break;

                case PacketsServer.PositionAndOrientationTeleport:
                    OnPositionAndOrientationTeleportClassic(packet);
                    break;

                case PacketsServer.PositionAndOrientationUpdate:
                    OnPositionAndOrientationUpdateClassic(packet);
                    break;

                case PacketsServer.PositionUpdate:
                    OnPositionUpdateClassic(packet);
                    break;

                case PacketsServer.OrientationUpdate:
                    OnOrientationUpdateClassic(packet);
                    break;

                case PacketsServer.DespawnPlayer:
                    OnDespawnPlayerClassic(packet);
                    break;

                case PacketsServer.Message:
                    OnMessageClassic(packet);
                    break;

                case PacketsServer.DisconnectPlayer:
                    OnDisconnectPlayerClassic(packet);
                    break;

                case PacketsServer.UpdateUserType:
                    OnUpdateUserTypeClassic(packet);
                    break;

                case PacketsServer.ExtInfo:
                    OnExtInfoClassic(packet);
                    break;

                case PacketsServer.ExtEntry:
                    OnExtEntryTypeClassic(packet);
                    break;

                case PacketsServer.SetClickDistance:
                    break;

                case PacketsServer.CustomBlockSupportLevel:
                    break;

                case PacketsServer.HoldThis:
                    break;

                case PacketsServer.SetTextHotKey:
                    break;

                case PacketsServer.ExtAddPlayerName:
                    break;

                case PacketsServer.ExtRemovePlayerName:
                    break;

                case PacketsServer.EnvSetColor:
                    break;

                case PacketsServer.MakeSelection:
                    break;

                case PacketsServer.RemoveSelection:
                    break;

                case PacketsServer.SetBlockPermission:
                    break;

                case PacketsServer.ChangeModel:
                    break;

                case PacketsServer.EnvSetMapAppearance:
                    break;

                case PacketsServer.EnvSetWeatherType:
                    break;

                case PacketsServer.HackControl:
                    break;

                case PacketsServer.ExtAddEntity2:
                    break;
            }
        }
    }
}
