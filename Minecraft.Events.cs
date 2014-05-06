using System.Collections.Generic;
using System.Text;
using Ionic.Zlib;
using MineLib.ClientWrapper.BigData;
using MineLib.ClientWrapper.Data.Anvil;
using MineLib.Network.Events;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Server;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        // -- Debugging
        public readonly List<string> ChatHistory = new List<string>();
        public readonly List<string> PluginMessageUnhandled = new List<string>();
        // -- Debugging

        public event PacketsHandler FirePacketHandled;

        #region Voids

        private void OnKeepAlive(IPacket packet)
        {
            var KeepAlive = (KeepAlivePacket) packet;

            SendPacket(KeepAlive);
        }

        private void OnJoinGame(IPacket packet)
        {
            var JoinGame = (JoinGamePacket) packet;

            Player.EntityID = JoinGame.EntityID;

            World.Difficulty = JoinGame.Difficulty;
            World.Dimension = JoinGame.Dimension;
            World.GameMode = JoinGame.GameMode;
            World.LevelType = JoinGame.LevelType;
            World.MaxPlayers = JoinGame.MaxPlayers;
        }

        private void OnChatMessage(IPacket packet)
        {
            var ChatMessage = (ChatMessagePacket) packet;

            // -- Debugging
            ChatHistory.Add(ChatMessage.Message);
            // -- Debugging

            DisplayChatMessage(ChatMessage.Message);
        }

        private void OnTimeUpdate(IPacket packet)
        {
            var TimeUpdate = (TimeUpdatePacket) packet;

            World.AgeOfTheWorld = TimeUpdate.AgeOfTheWorld;
            World.TimeOfDay = TimeUpdate.TimeOfDay;
        }

        private void OnEntityEquipment(IPacket packet)
        {
            var EntityEquipment = (EntityEquipmentPacket) packet;

            if (!Entities.ContainsKey(EntityEquipment.EntityID))
                Entities.Add(EntityEquipment.EntityID, new Entity {EntityID = EntityEquipment.EntityID});

            Entities[EntityEquipment.EntityID].Equipment.Item = EntityEquipment.Item;
            Entities[EntityEquipment.EntityID].Equipment.Slot = EntityEquipment.Slot;
        }

        private void OnSpawnPosition(IPacket packet)
        {
            var SpawnPosition = (SpawnPositionPacket) packet;

            World.SpawnPosition = SpawnPosition.Coordinates;
        }

        private void OnUpdateHealth(IPacket packet)
        {
            var UpdateHealth = (UpdateHealthPacket) packet;

            Player.Health.Food = UpdateHealth.Food;
            Player.Health.FoodSaturation = UpdateHealth.FoodSaturation;
            Player.Health.Health = UpdateHealth.Health;
        }

        private void OnRespawn(IPacket packet)
        {
            var Respawn = (RespawnPacket) packet;

            World.Dimension = Respawn.Dimension;
            World.Difficulty = Respawn.Difficulty;
            World.GameMode = Respawn.GameMode;
            World.LevelType = Respawn.LevelType;

            World.Chunks.Clear();
            // And unload all chunks.
        }

        private void OnPlayerPositionAndLook(IPacket packet)
        {
            var PlayerPositionAndLook = (PlayerPositionAndLookPacket) packet;

            // Force to new position.
            Player.Position.Vector3 = PlayerPositionAndLook.Vector3;
            Player.Look.Yaw = PlayerPositionAndLook.Yaw;
            Player.Look.Pitch = PlayerPositionAndLook.Pitch;
            Player.Position.OnGround = PlayerPositionAndLook.OnGround;

            SendPacket(new Network.Packets.Client.PlayerPositionPacket
            {
                X = PlayerPositionAndLook.Vector3.X,
                HeadY = PlayerPositionAndLook.Vector3.Y,
                FeetY = PlayerPositionAndLook.Vector3.Y - 1.62,
                Z = PlayerPositionAndLook.Vector3.Z,
                OnGround = PlayerPositionAndLook.OnGround
            });

            SendPacket(new Network.Packets.Client.PlayerLookPacket
            {
                Yaw = PlayerPositionAndLook.Yaw,
                Pitch = PlayerPositionAndLook.Pitch,
                OnGround = PlayerPositionAndLook.OnGround
            });
        }

        private void OnHeldItemChange(IPacket packet)
        {
            var HeldItemChange = (HeldItemChangePacket) packet;

            Player.HeldItem = HeldItemChange.Slot;
        }

        private void OnUseBed(IPacket packet)
        {
            var UseBed = (UseBedPacket) packet;

            if (!Entities.ContainsKey(UseBed.EntityID))
                Entities.Add(UseBed.EntityID, new Entity {EntityID = UseBed.EntityID});

            Entities[UseBed.EntityID].Bed = UseBed.Coordinates;
        }

        private void OnAnimation(IPacket packet)
        {
            var Animation = (AnimationPacket) packet;

            if (!Entities.ContainsKey(Animation.EntityID))
                Entities.Add(Animation.EntityID, new Entity {EntityID = Animation.EntityID});

            Entities[Animation.EntityID].Animation = Animation.Animation;
        }

        private void OnSpawnPlayer(IPacket packet)
        {
            var SpawnPlayer = (SpawnPlayerPacket) packet;

            if (!Entities.ContainsKey(SpawnPlayer.EntityID))
                Entities.Add(SpawnPlayer.EntityID, new Entity {EntityID = SpawnPlayer.EntityID});

            Entities[SpawnPlayer.EntityID].Player.UUID = SpawnPlayer.PlayerUUID;
            Entities[SpawnPlayer.EntityID].Player.Name = SpawnPlayer.PlayerName;
            Entities[SpawnPlayer.EntityID].Position = SpawnPlayer.Vector3;

            Entities[SpawnPlayer.EntityID].Look.Yaw = SpawnPlayer.Yaw;
            Entities[SpawnPlayer.EntityID].Look.Pitch = SpawnPlayer.Pitch;

            Entities[SpawnPlayer.EntityID].Metadata = SpawnPlayer.Metadata;
        }

        private void OnCollectItem(IPacket packet)
        {
            var CollectItem = (CollectItemPacket) packet;
        }

        private void OnSpawnObject(IPacket packet)
        {
            var SpawnObject = (SpawnObjectPacket) packet;
        }

        private void OnSpawnMob(IPacket packet)
        {
            var SpawnMob = (SpawnMobPacket) packet;

            if (!Entities.ContainsKey(SpawnMob.EntityID))
                Entities.Add(SpawnMob.EntityID, new Entity {EntityID = SpawnMob.EntityID});

            Entities[SpawnMob.EntityID].Position = SpawnMob.Vector3;

            Entities[SpawnMob.EntityID].Look.Yaw = SpawnMob.Yaw;
            Entities[SpawnMob.EntityID].Look.Pitch = SpawnMob.Pitch;

            Entities[SpawnMob.EntityID].Velocity.VelocityX = SpawnMob.VelocityX;
            Entities[SpawnMob.EntityID].Velocity.VelocityY = SpawnMob.VelocityY;
            Entities[SpawnMob.EntityID].Velocity.VelocityZ = SpawnMob.VelocityZ;
            Entities[SpawnMob.EntityID].Metadata = SpawnMob.Metadata;
        }

        private void OnSpawnPainting(IPacket packet)
        {
            var SpawnPainting = (SpawnPaintingPacket) packet;
        }

        private void OnSpawnExperienceOrb(IPacket packet)
        {
            var SpawnExperienceOrb = (SpawnExperienceOrbPacket) packet;
        }

        private void OnEntityVelocity(IPacket packet)
        {
            var EntityVelocity = (EntityVelocityPacket) packet;

            if (!Entities.ContainsKey(EntityVelocity.EntityID))
                Entities.Add(EntityVelocity.EntityID, new Entity {EntityID = EntityVelocity.EntityID});

            Entities[EntityVelocity.EntityID].Velocity.VelocityX = EntityVelocity.VelocityX;
            Entities[EntityVelocity.EntityID].Velocity.VelocityY = EntityVelocity.VelocityY;
            Entities[EntityVelocity.EntityID].Velocity.VelocityZ = EntityVelocity.VelocityZ;
        }

        private void OnDestroyEntities(IPacket packet)
        {
            var DestroyEntities = (DestroyEntitiesPacket) packet;
            foreach (int t in DestroyEntities.EntityIDs)
            {
                Entities.Remove(t);
            }
        }

        private void OnEntity(IPacket packet)
        {
            var Entity = (EntityPacket) packet;
            if (!Entities.ContainsKey(Entity.EntityID))
                Entities.Add(Entity.EntityID, new Entity {EntityID = Entity.EntityID});
        }

        private void OnEntityRelativeMove(IPacket packet)
        {
            var EntityRelativeMove = (EntityRelativeMovePacket) packet;

            if (!Entities.ContainsKey(EntityRelativeMove.EntityID))
                Entities.Add(EntityRelativeMove.EntityID, new Entity {EntityID = EntityRelativeMove.EntityID});

            Entities[EntityRelativeMove.EntityID].Position = EntityRelativeMove.DeltaVector3; //Nope
        }

        private void OnEntityLook(IPacket packet)
        {
            var EntityLook = (EntityLookPacket) packet;

            if (!Entities.ContainsKey(EntityLook.EntityID))
                Entities.Add(EntityLook.EntityID, new Entity {EntityID = EntityLook.EntityID});

            Entities[EntityLook.EntityID].Look.Yaw = EntityLook.Yaw;
            Entities[EntityLook.EntityID].Look.Pitch = EntityLook.Pitch;
        }

        private void OnEntityLookAndRelativeMove(IPacket packet)
        {
            var EntityLookAndRelativeMove = (EntityLookAndRelativeMovePacket) packet;

            if (!Entities.ContainsKey(EntityLookAndRelativeMove.EntityID))
                Entities.Add(EntityLookAndRelativeMove.EntityID,
                    new Entity {EntityID = EntityLookAndRelativeMove.EntityID});

            Entities[EntityLookAndRelativeMove.EntityID].Position = EntityLookAndRelativeMove.DeltaVector3; //Nope
            Entities[EntityLookAndRelativeMove.EntityID].Look.Yaw = EntityLookAndRelativeMove.Yaw;
            Entities[EntityLookAndRelativeMove.EntityID].Look.Pitch = EntityLookAndRelativeMove.Pitch;
        }

        private void OnEntityTeleport(IPacket packet)
        {
            var EntityTeleport = (EntityTeleportPacket) packet;

            if (!Entities.ContainsKey(EntityTeleport.EntityID))
                Entities.Add(EntityTeleport.EntityID, new Entity {EntityID = EntityTeleport.EntityID});

            Entities[EntityTeleport.EntityID].Position = EntityTeleport.Vector3;
            Entities[EntityTeleport.EntityID].Look.Yaw = EntityTeleport.Yaw;
            Entities[EntityTeleport.EntityID].Look.Pitch = EntityTeleport.Pitch;
        }

        private void OnEntityHeadLook(IPacket packet)
        {
            var EntityHeadLook = (EntityHeadLookPacket) packet;

            if (!Entities.ContainsKey(EntityHeadLook.EntityID))
                Entities.Add(EntityHeadLook.EntityID, new Entity {EntityID = EntityHeadLook.EntityID});

            Entities[EntityHeadLook.EntityID].Look.Yaw = EntityHeadLook.HeadYaw;
        }

        private void OnEntityStatus(IPacket packet)
        {
            var EntityStatus = (EntityStatusPacket) packet;

            if (!Entities.ContainsKey(EntityStatus.EntityID))
                Entities.Add(EntityStatus.EntityID, new Entity {EntityID = EntityStatus.EntityID});

            Entities[EntityStatus.EntityID].Status = EntityStatus.Status;
        }

        private void OnAttachEntity(IPacket packet)
        {
            var AttachEntity = (AttachEntityPacket) packet;

            if (!Entities.ContainsKey(AttachEntity.EntityID))
                Entities.Add(AttachEntity.EntityID, new Entity {EntityID = AttachEntity.EntityID});

            Entities[AttachEntity.EntityID].Vehicle.VehicleID = AttachEntity.VehicleID;
            Entities[AttachEntity.EntityID].Leash = AttachEntity.Leash;
        }

        private void OnEntityMetadata(IPacket packet)
        {
            var EntityMetadata = (EntityMetadataPacket) packet;

            if (!Entities.ContainsKey(EntityMetadata.EntityID))
                Entities.Add(EntityMetadata.EntityID, new Entity {EntityID = EntityMetadata.EntityID});

            Entities[EntityMetadata.EntityID].Metadata = EntityMetadata.Metadata;
        }

        private void OnEntityEffect(IPacket packet)
        {
            var EntityEffect = (EntityEffectPacket) packet;

            if (Player.EntityID == EntityEffect.EntityID)
            {
                Player.Effects.Add(new PlayerEffect
                {
                    EffectID = EntityEffect.EffectID,
                    Amplifier = EntityEffect.Amplifier,
                    Duration = EntityEffect.Duration
                });
            }
            else
            {
                if (Entities.ContainsKey(EntityEffect.EntityID))
                    return;

                Entities.Add(EntityEffect.EntityID, new Entity {EntityID = EntityEffect.EntityID});
                Entities[EntityEffect.EntityID].Effects.Add(new EntityEffect
                {
                    EffectID = EntityEffect.EffectID,
                    Amplifier = EntityEffect.Amplifier,
                    Duration = EntityEffect.Duration
                });
            }
        }

        private void OnRemoveEntityEffect(IPacket packet)
        {
            var RemoveEntityEffect = (RemoveEntityEffectPacket) packet;

            if (Player.EntityID == RemoveEntityEffect.EntityID)
            {
                foreach (var effect in Player.Effects.ToArray())
                {
                    if (effect.EffectID == RemoveEntityEffect.EffectID)
                        Player.Effects.Remove(effect);
                }
            }
            else
            {
                if (!Entities.ContainsKey(RemoveEntityEffect.EntityID))
                    Entities.Add(RemoveEntityEffect.EntityID, new Entity {EntityID = RemoveEntityEffect.EntityID});

                foreach (var effect in Entities[RemoveEntityEffect.EntityID].Effects.ToArray())
                {
                    if (effect.EffectID == RemoveEntityEffect.EffectID)
                        Entities[RemoveEntityEffect.EntityID].Effects.Remove(effect);
                }
            }
        }

        private void OnSetExperience(IPacket packet)
        {
            var SetExperience = (SetExperiencePacket) packet;

            Player.Experience.ExperienceBar = SetExperience.ExperienceBar;
            Player.Experience.Level = SetExperience.Level;
            Player.Experience.TotalExperience = SetExperience.TotalExperience;
        }

        private void OnEntityProperties(IPacket packet)
        {
            var EntityProperties = (EntityPropertiesPacket) packet;

            if (!Entities.ContainsKey(EntityProperties.EntityID))
                Entities.Add(EntityProperties.EntityID, new Entity {EntityID = EntityProperties.EntityID});

            Entities[EntityProperties.EntityID].Properties = EntityProperties.Properties;
        }

        private void OnChunkData(IPacket packet) // -- Works
        {
            var ChunkData = (ChunkDataPacket) packet;

            if (World == null)
                World = new World();

            if (ChunkData.PrimaryBitMap == 0)
            {
                var cIndex = World.GetChunkIndex(ChunkData.Coordinates);

                if (cIndex != -1)
                    World.Chunks.RemoveAt(cIndex);

                return;
            }

            // -- Create new chunk
            var chunk = new Chunk
            {
                Coordinates = ChunkData.Coordinates,
                PrimaryBitMap = ChunkData.PrimaryBitMap,
                AddBitMap = ChunkData.AddBitMap,
                SkyLightSent = ChunkData.SkyLightSend,
                GroundUp = ChunkData.GroundUp
            };

            // -- Decompress the data
            byte[] decompressedData = ZlibStream.UncompressBuffer(ChunkData.Data);

            chunk.ReadChunkData(decompressedData);

            // -- Add the chunk to the world
            World.SetChunk(chunk);     
        }

        private void OnMultiBlockChange(IPacket packet) // -- Works
        {
            var MultiBlockChange = (MultiBlockChangePacket)packet;
            for (var i = 0; i < MultiBlockChange.RecordCount; i++)
            {
                var block = new Block
                {
                    Id = MultiBlockChange.RecordsArray[i].BlockID,
                    Meta = MultiBlockChange.RecordsArray[i].Metadata,
                };
                World.SetBlock(MultiBlockChange.RecordsArray[i].Coordinates, block);
            }
           
        }

        private void OnBlockChange(IPacket packet) // -- Works
        {
            var BlockChange = (BlockChangePacket) packet;

            var data = new Block
            {
                Id = BlockChange.BlockID,
                Meta = BlockChange.BlockMetadata,
                Name = Block.GetName(BlockChange.BlockID, BlockChange.BlockMetadata)
            };

            World.SetBlock(BlockChange.Coordinates, data);
        }

        private void OnBlockAction(IPacket packet)
        {
        }

        private void OnBlockBreakAnimation(IPacket packet)
        {
        }

        private void OnMapChunkBulk(IPacket packet) // -- Works in Online mode, bug in Pirate mode
        {
            var MapChunkBulk = (MapChunkBulkPacket) packet;

            var chunks = new Chunk[MapChunkBulk.ChunkColumnCount];

            byte[] DecompressedData;

            try { DecompressedData = ZlibStream.UncompressBuffer(MapChunkBulk.ChunkData); }
            catch { World.DamagedChunks.Add(MapChunkBulk); return; }

            var i = 0;
            foreach (var metadata in MapChunkBulk.MetaInformation)
            {
                chunks[i] = new Chunk
                {
                    Coordinates = metadata.Coordinates,
                    PrimaryBitMap = metadata.PrimaryBitMap,
                    AddBitMap = metadata.AddBitMap,
                    SkyLightSent = metadata.SkyLightSend,
                    GroundUp = metadata.GroundUp
                };

                DecompressedData = chunks[i].ReadChunkData(DecompressedData);
                // -- Calls the chunk class to take all of the bytes it needs, and return whats left.

                World.SetChunk(chunks[i]);
                i++;
            }
        }

        private void OnExplosion(IPacket packet)
        {
        }

        private void OnEffect(IPacket packet)
        {
            var Effect = (EffectPacket) packet;

            PlayEffect(Effect.EffectID, Effect.Coordinates, Effect.Data, Effect.DisableRelativeVolume);
        }

        private void OnSoundEffect(IPacket packet)
        {
            var SoundEffect = (SoundEffectPacket) packet;

            PlaySound(SoundEffect.SoundName, SoundEffect.Coordinates, SoundEffect.Volume, SoundEffect.Pitch);
        }

        private void OnParticle(IPacket packet)
        {
        }

        private void OnChangeGameState(IPacket packet)
        {
            var ChangeGameState = (ChangeGameStatePacket) packet;

            World.StateChanged.Reason = ChangeGameState.Reason;
            World.StateChanged.Value = ChangeGameState.Value;
        }

        private void OnSpawnGlobalEntity(IPacket packet)
        {
        }

        private void OnOpenWindow(IPacket packet)
        {
            var OpenWindow = (OpenWindowPacket) packet;

            Player.OpenWindow(OpenWindow.WindowID, OpenWindow.InventoryType, OpenWindow.WindowTitle,
                OpenWindow.NumberOfSlots, OpenWindow.UseProvidedTitle, OpenWindow.EntityID);
        }

        private void OnCloseWindow(IPacket packet)
        {
            var CloseWindow = (CloseWindowPacket) packet;

            Player.CloseWindow(CloseWindow.WindowID);
        }

        private void OnSetSlot(IPacket packet)
        {
            var SetSlot = (SetSlotPacket) packet;

            Player.SetSlot(SetSlot.WindowId, SetSlot.Slot, SetSlot.SlotData);
        }

        private void OnWindowItems(IPacket packet)
        {
            var WindowItems = (WindowItemsPacket) packet;

            Player.SetWindowItems(WindowItems.WindowId, WindowItems.SlotData);
        }

        private void OnWindowProperty(IPacket packet)
        {
            var WindowProperty = (WindowPropertyPacket) packet;
        }

        private void OnConfirmTransaction(IPacket packet)
        {
            var ConfirmTransaction = (ConfirmTransactionPacket) packet;

            Player.ConfirmTransaction(ConfirmTransaction.WindowId, ConfirmTransaction.ActionNumber,
                ConfirmTransaction.Accepted);
        }

        private void OnUpdateSign(IPacket packet)
        {
        }

        private void OnMaps(IPacket packet)
        {
        }

        private void OnUpdateBlockEntity(IPacket packet)
        {
        }

        private void OnSignEditorOpen(IPacket packet)
        {
            var SignEditorOpen = (SignEditorOpenPacket) packet;

            EditSign(SignEditorOpen.Coordinates);
        }

        private void OnStatistics(IPacket packet)
        {
            var Statistics = (StatisticsPacket) packet;

            Player.Statistics.Count = Statistics.Count;
            Player.Statistics.StatisticsName = Statistics.StatisticsName;
            Player.Statistics.Value = Statistics.Value;
        }

        private void OnPlayerListItem(IPacket packet)
        {
            var PlayerListItem = (PlayerListItemPacket) packet;

            // Maybe Clear PlayerList?
            if (!PlayersList.ContainsKey(PlayerListItem.PlayerName))
                PlayersList.Add(PlayerListItem.PlayerName, PlayerListItem.Ping);
        }

        private void OnPlayerAbilities(IPacket packet)
        {
            var PlayerAbilities = (PlayerAbilitiesPacket) packet;
            Player.Abilities.Flags = PlayerAbilities.Flags;
            Player.Abilities.FlyingSpeed = PlayerAbilities.FlyingSpeed;
            Player.Abilities.WalkingSpeed = PlayerAbilities.WalkingSpeed;
        }

        private void OnTabComplete(IPacket packet)
        {
        }

        private void OnScoreboardObjective(IPacket packet)
        {
        }

        private void OnUpdateScore(IPacket packet)
        {
        }

        private void OnDisplayScoreboard(IPacket packet)
        {
        }

        private void OnTeams(IPacket packet)
        {
        }

        private void OnPluginMessage(IPacket packet)
        {
            var PluginMessage = (PluginMessagePacket) packet;

            switch (PluginMessage.Channel)
            {
                case "MC|Brand":
                    ServerBrand = Encoding.UTF8.GetString(PluginMessage.Data);
                    break;

                default:
                    PluginMessageUnhandled.Add(PluginMessage.Channel + " : " + Encoding.UTF8.GetString(PluginMessage.Data));
                    break;
            }
        }

        private void OnDisconnect(IPacket packet)
        {
        }

        #endregion
    }
}