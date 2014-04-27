using System;
using System.Collections.Generic;
using MineLib.Network.Enums;
using MineLib.Network.Packets;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        // -- Debugging
        List<IPacket> packets = new List<IPacket>();
        // -- Debugging

        private void RaisePacketHandled(IPacket packet, int id, ServerState? state)
        {
            // -- Debugging
            Console.WriteLine("ID: 0x" + String.Format("{0:X}", id));
            Console.WriteLine(" ");
            packets.Add(packet);
            // -- Debugging

            switch (state)
            {
                case ServerState.Login:

                    #region Login

                    switch ((PacketsServer) id)
                    {
                        case PacketsServer.LoginDisconnect:
                            // Stop.
                            break;

                        case PacketsServer.EncryptionRequest: // -- NetworkHandler do all stuff automatic
                            break;

                        case PacketsServer.LoginSuccess:
                            State = ServerState.Play;
                            break;
                    }

                    #endregion Login

                    break;

                case ServerState.Play:

                    #region Play

                    switch ((PacketsServer) id)
                    {
                        case PacketsServer.KeepAlive:
                            OnKeepAlive(packet);
                            break;

                        case PacketsServer.JoinGame:
                            OnJoinGame(packet);
                            break;

                        case PacketsServer.ChatMessage:
                            OnChatMessage(packet);
                            break;

                        case PacketsServer.TimeUpdate:
                            OnTimeUpdate(packet);
                            break;

                        case PacketsServer.EntityEquipment:
                            OnEntityEquipment(packet);
                            break;

                        case PacketsServer.SpawnPosition:
                            OnSpawnPosition(packet);
                            break;

                        case PacketsServer.UpdateHealth:
                            OnUpdateHealth(packet);
                            break;

                        case PacketsServer.Respawn:
                            OnRespawn(packet);
                            break;

                        case PacketsServer.PlayerPositionAndLook:
                            OnPlayerPositionAndLook(packet);
                            break;

                        case PacketsServer.HeldItemChange:
                            OnHeldItemChange(packet);
                            break;

                        case PacketsServer.UseBed:
                            OnUseBed(packet);
                            break;

                        case PacketsServer.Animation:
                            OnAnimation(packet);
                            break;

                        case PacketsServer.SpawnPlayer:
                            OnSpawnPlayer(packet);
                            break;

                        case PacketsServer.CollectItem:
                            OnCollectItem(packet);
                            break;

                        case PacketsServer.SpawnObject:
                            OnSpawnObject(packet);
                            break;

                        case PacketsServer.SpawnMob:
                            OnSpawnMob(packet);
                            break;

                        case PacketsServer.SpawnPainting:
                            OnSpawnPainting(packet);
                            break;

                        case PacketsServer.SpawnExperienceOrb:
                            OnSpawnExperienceOrb(packet);
                            break;

                        case PacketsServer.EntityVelocity:
                            OnEntityVelocity(packet);
                            break;

                        case PacketsServer.DestroyEntities:
                            OnDestroyEntities(packet);
                            break;

                        case PacketsServer.Entity:
                            OnEntity(packet);
                            break;

                        case PacketsServer.EntityRelativeMove:
                            OnEntityRelativeMove(packet);
                            break;

                        case PacketsServer.EntityLook:
                            OnEntityLook(packet);
                            break;

                        case PacketsServer.EntityLookAndRelativeMove:
                            OnEntityLookAndRelativeMove(packet);
                            break;

                        case PacketsServer.EntityTeleport:
                            OnEntityTeleport(packet);
                            break;

                        case PacketsServer.EntityHeadLook:
                            OnEntityHeadLook(packet);
                            break;

                        case PacketsServer.EntityStatus:
                            OnEntityStatus(packet);
                            break;

                        case PacketsServer.AttachEntity:
                            OnAttachEntity(packet);
                            break;

                        case PacketsServer.EntityMetadata:
                            OnEntityMetadata(packet);
                            break;

                        case PacketsServer.EntityEffect:
                            OnEntityEffect(packet);
                            break;

                        case PacketsServer.RemoveEntityEffect:
                            OnRemoveEntityEffect(packet);
                            break;

                        case PacketsServer.SetExperience:
                            OnSetExperience(packet);
                            break;

                        case PacketsServer.EntityProperties:
                            OnEntityProperties(packet);
                            break;

                        case PacketsServer.ChunkData:
                            OnChunkData(packet);
                            break;

                        case PacketsServer.MultiBlockChange:
                            OnMultiBlockChange(packet);
                            break;

                        case PacketsServer.BlockChange:
                            OnBlockChange(packet);
                            break;

                        case PacketsServer.BlockAction:
                            OnBlockAction(packet);
                            break;

                        case PacketsServer.BlockBreakAnimation:
                            OnBlockBreakAnimation(packet);
                            break;

                        case PacketsServer.MapChunkBulk:
                            OnMapChunkBulk(packet);
                            break;

                        case PacketsServer.Explosion:
                            OnExplosion(packet);
                            break;

                        case PacketsServer.Effect:
                            OnEffect(packet);
                            break;

                        case PacketsServer.SoundEffect:
                            OnSoundEffect(packet);
                            break;

                        case PacketsServer.Particle:
                            OnParticle(packet);
                            break;

                        case PacketsServer.ChangeGameState:
                            OnChangeGameState(packet);
                            break;

                        case PacketsServer.SpawnGlobalEntity:
                            OnSpawnGlobalEntity(packet);
                            break;

                        case PacketsServer.OpenWindow:
                            OnOpenWindow(packet);
                            break;

                        case PacketsServer.CloseWindow:
                            OnCloseWindow(packet);
                            break;

                        case PacketsServer.SetSlot:
                            OnSetSlot(packet);
                            break;

                        case PacketsServer.WindowItems:
                            OnWindowItems(packet);
                            break;

                        case PacketsServer.WindowProperty:
                            OnWindowProperty(packet);
                            break;

                        case PacketsServer.ConfirmTransaction:
                            OnConfirmTransaction(packet);
                            break;

                        case PacketsServer.UpdateSign:
                            OnUpdateSign(packet);
                            break;

                        case PacketsServer.Maps:
                            OnMaps(packet);
                            break;

                        case PacketsServer.UpdateBlockEntity:
                            OnUpdateBlockEntity(packet);
                            break;

                        case PacketsServer.SignEditorOpen:
                            OnSignEditorOpen(packet);
                            break;

                        case PacketsServer.Statistics:
                            OnStatistics(packet);
                            break;

                        case PacketsServer.PlayerListItem:
                            OnPlayerListItem(packet);
                            break;

                        case PacketsServer.PlayerAbilities:
                            OnPlayerAbilities(packet);
                            break;

                        case PacketsServer.TabComplete:
                            OnTabComplete(packet);
                            break;

                        case PacketsServer.ScoreboardObjective:
                            OnScoreboardObjective(packet);
                            break;

                        case PacketsServer.UpdateScore:
                            OnUpdateScore(packet);
                            break;

                        case PacketsServer.DisplayScoreboard:
                            OnDisplayScoreboard(packet);
                            break;

                        case PacketsServer.Teams:
                            OnTeams(packet);
                            break;

                        case PacketsServer.PluginMessage:
                            OnPluginMessage(packet);
                            break;

                        case PacketsServer.Disconnect:
                            OnDisconnect(packet);
                            break;
                    }

                    #endregion

                    break;

                case ServerState.Status: // -- We don't use that normally.
                    break;

                default:
                    if (FirePacketHandled != null)
                        FirePacketHandled(packet, id, state);
                    break;
            }
        }

    }
}