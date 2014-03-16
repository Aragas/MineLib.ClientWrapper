using System;
using System.Text;
using MineLib.ClientWrapper.BigData;
using MineLib.ClientWrapper.Data.Anvil;
using MineLib.Network.Data;
using MineLib.Network.Events;
using MineLib.Network.Packets;
using MineLib.Network.Packets.Server;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
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

            DisplayChatMessage(ChatMessage.Message);
        }

        private void OnTimeUpdate(IPacket packet)
        {
            var TimeUpdate = (TimeUpdatePacket) packet;

            if (Ready)
                SendPacket(Player.Packet());

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

            World.SpawnPosition.X = SpawnPosition.X;
            World.SpawnPosition.Y = SpawnPosition.Y;
            World.SpawnPosition.Z = SpawnPosition.Z;
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

            // And unload all chunks.
        }

        private void OnPlayerPositionAndLook(IPacket packet)
        {
            var PlayerPositionAndLook = (PlayerPositionAndLookPacket) packet;

            if (!Player.Position.Initialized)
            {
                Player.Position.Vector3.X = (int) PlayerPositionAndLook.X;
                Player.Position.Vector3.Y = (int) PlayerPositionAndLook.Y;
                Player.Position.Vector3.Z = (int) PlayerPositionAndLook.Z;
                Player.Look.Yaw = PlayerPositionAndLook.Yaw;
                Player.Look.Pitch = PlayerPositionAndLook.Pitch;
                Player.Position.OnGround = PlayerPositionAndLook.OnGround;

                Player.Position.Initialized = true;
            }
            else
            {
                Player.NewPosition.X = PlayerPositionAndLook.X;
                Player.NewPosition.Y = PlayerPositionAndLook.Y;
                Player.NewPosition.Z = PlayerPositionAndLook.Z;
                Player.NewPosition.Yaw = PlayerPositionAndLook.Yaw;
                Player.NewPosition.Pitch = PlayerPositionAndLook.Pitch;
                Player.NewPosition.OnGround = PlayerPositionAndLook.OnGround;
            }
        }

        private void OnHeldItemChange(IPacket packet)
        {
            var HeldItemChange = (HeldItemChangePacket) packet;

            Player.HeldItem.Slot = HeldItemChange.Slot;
        }

        private void OnUseBed(IPacket packet)
        {
            var UseBed = (UseBedPacket) packet;

            if (!Entities.ContainsKey(UseBed.EntityID))
                Entities.Add(UseBed.EntityID, new Entity {EntityID = UseBed.EntityID});

            Entities[UseBed.EntityID].Bed.X = UseBed.X;
            Entities[UseBed.EntityID].Bed.Y = UseBed.Y;
            Entities[UseBed.EntityID].Bed.Z = UseBed.Z;
        }

        private void OnAnimation(IPacket packet)
        {
            var Animation = (AnimationPacket) packet;

            if (!Entities.ContainsKey(Animation.EntityID))
                Entities.Add(Animation.EntityID, new Entity {EntityID = Animation.EntityID});

            Entities[Animation.EntityID].Animation.Animation = Animation.Animation;
        }

        private void OnSpawnPlayer(IPacket packet)
        {
            var SpawnPlayer = (SpawnPlayerPacket) packet;

            if (!Entities.ContainsKey(SpawnPlayer.EntityID))
                Entities.Add(SpawnPlayer.EntityID, new Entity {EntityID = SpawnPlayer.EntityID});

            Entities[SpawnPlayer.EntityID].Player.PlayerUUID = SpawnPlayer.PlayerUUID;
            Entities[SpawnPlayer.EntityID].Player.PlayerName = SpawnPlayer.PlayerName;
            Entities[SpawnPlayer.EntityID].Position.X = SpawnPlayer.X;
            Entities[SpawnPlayer.EntityID].Position.Y = SpawnPlayer.Y;
            Entities[SpawnPlayer.EntityID].Position.Z = SpawnPlayer.Z;
            Entities[SpawnPlayer.EntityID].Look.Yaw = SpawnPlayer.Yaw;
            Entities[SpawnPlayer.EntityID].Look.Pitch = SpawnPlayer.Pitch;
            Entities[SpawnPlayer.EntityID].Metadata = SpawnPlayer.Metadata;
        }

        private void OnCollectItem(IPacket packet)
        {
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

            Entities[SpawnMob.EntityID].Position.X = SpawnMob.X;
            Entities[SpawnMob.EntityID].Position.Y = SpawnMob.Y;
            Entities[SpawnMob.EntityID].Position.Z = SpawnMob.Z;
            Entities[SpawnMob.EntityID].Look.Yaw = SpawnMob.Yaw;
            Entities[SpawnMob.EntityID].Look.HeadPitch = SpawnMob.HeadPitch;
            Entities[SpawnMob.EntityID].Look.Pitch = SpawnMob.Pitch;
            Entities[SpawnMob.EntityID].Velocity.VelocityX = SpawnMob.VelocityX;
            Entities[SpawnMob.EntityID].Velocity.VelocityY = SpawnMob.VelocityY;
            Entities[SpawnMob.EntityID].Velocity.VelocityZ = SpawnMob.VelocityZ;
            Entities[SpawnMob.EntityID].Metadata = SpawnMob.Metadata;
        }

        private void OnSpawnPainting(IPacket packet)
        {
        }

        private void OnSpawnExperienceOrb(IPacket packet)
        {
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

            Entities[EntityRelativeMove.EntityID].NewPosition.X = EntityRelativeMove.DeltaX;
            Entities[EntityRelativeMove.EntityID].NewPosition.Y = EntityRelativeMove.DeltaY;
            Entities[EntityRelativeMove.EntityID].NewPosition.Z = EntityRelativeMove.DeltaZ;
        }

        private void OnEntityLook(IPacket packet)
        {
            var EntityLook = (EntityLookPacket) packet;

            if (!Entities.ContainsKey(EntityLook.EntityID))
                Entities.Add(EntityLook.EntityID, new Entity {EntityID = EntityLook.EntityID});

            Entities[EntityLook.EntityID].NewPosition.Yaw = EntityLook.Yaw;
            Entities[EntityLook.EntityID].NewPosition.Pitch = EntityLook.Pitch;
        }

        private void OnEntityLookAndRelativeMove(IPacket packet)
        {
            var EntityLookAndRelativeMove = (EntityLookAndRelativeMovePacket) packet;

            if (!Entities.ContainsKey(EntityLookAndRelativeMove.EntityID))
                Entities.Add(EntityLookAndRelativeMove.EntityID,
                    new Entity {EntityID = EntityLookAndRelativeMove.EntityID});

            Entities[EntityLookAndRelativeMove.EntityID].NewPosition.X = EntityLookAndRelativeMove.DeltaX;
            Entities[EntityLookAndRelativeMove.EntityID].NewPosition.Y = EntityLookAndRelativeMove.DeltaY;
            Entities[EntityLookAndRelativeMove.EntityID].NewPosition.Z = EntityLookAndRelativeMove.DeltaZ;
            Entities[EntityLookAndRelativeMove.EntityID].NewPosition.Yaw = EntityLookAndRelativeMove.Yaw;
            Entities[EntityLookAndRelativeMove.EntityID].NewPosition.Pitch = EntityLookAndRelativeMove.Pitch;
        }

        private void OnEntityTeleport(IPacket packet)
        {
            var EntityTeleport = (EntityTeleportPacket) packet;

            if (!Entities.ContainsKey(EntityTeleport.EntityID))
                Entities.Add(EntityTeleport.EntityID, new Entity {EntityID = EntityTeleport.EntityID});

            Entities[EntityTeleport.EntityID].NewPosition.X = EntityTeleport.X;
            Entities[EntityTeleport.EntityID].NewPosition.Y = EntityTeleport.Y;
            Entities[EntityTeleport.EntityID].NewPosition.Z = EntityTeleport.Z;
            Entities[EntityTeleport.EntityID].NewPosition.Yaw = EntityTeleport.Yaw;
            Entities[EntityTeleport.EntityID].NewPosition.Pitch = EntityTeleport.Pitch;
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

            Entities[AttachEntity.EntityID].Vehile.VehileID = AttachEntity.VehicleID;
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
                if (!Player.Effects.ContainsKey(EntityEffect.EffectID))
                    Player.Effects.Add(EntityEffect.EffectID, new PlayerEffect
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
                Entities[EntityEffect.EntityID].Effects.Add(EntityEffect.EntityID, new EntityEffect
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
                Player.Effects.Remove(RemoveEntityEffect.EntityID);
            }
            else
            {
                if (!Entities.ContainsKey(RemoveEntityEffect.EntityID))
                    Entities.Add(RemoveEntityEffect.EntityID, new Entity {EntityID = RemoveEntityEffect.EntityID});

                Entities[RemoveEntityEffect.EntityID].Effects.Remove(RemoveEntityEffect.EntityID);
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

        private void OnChunkData(IPacket packet)
        {
            var ChunkData = (ChunkDataPacket) packet;

            if (ChunkData.PrimaryBitMap == 0)
            {
                // -- Unload chunk.
                int cIndex = -1;

                if (World != null)
                    cIndex = World.GetChunkIndex((int) ChunkData.Coordinates.X, (int) ChunkData.Coordinates.Z);

                if (cIndex != -1)
                    World.WorldChunks.RemoveAt(cIndex);

                //mc.RaiseChunkUnload(X, Z);
                return;
            }

            // -- Remove GZip Header
            Array.Copy(ChunkData.Data, 2, ChunkData.Trim, 0, ChunkData.Trim.Length);

            // -- Decompress the data
            byte[] decompressedData = Decompressor.Decompress(ChunkData.Trim);

            // -- Create new chunk
            var newChunk = new Chunk((int) ChunkData.Coordinates.X, (int) ChunkData.Coordinates.Z,
                ChunkData.PrimaryBitMap, ChunkData.AddBitMap, true, ChunkData.GroundUpContinuous);
            // -- Skylight assumed true
            newChunk.GetData(decompressedData);

            if (World == null)
                World = new World();

            // -- Add the chunk to the world
            World.WorldChunks.Add(newChunk);

            //mc.RaiseChunkLoad(X, Z);
        }

        private void OnMultiBlockChange(IPacket packet)
        {
        }

        private void OnBlockChange(IPacket packet)
        {
        }

        private void OnBlockAction(IPacket packet)
        {
        }

        private void OnBlockBreakAnimation(IPacket packet)
        {
        }

        private void OnMapChunkBulk(IPacket packet)
        {
            var MapChunkBulk = (MapChunkBulkPacket) packet;

            var chunks = new Chunk[MapChunkBulk.ChunkColumnCount];

            Array.Copy(MapChunkBulk.ChunkData, 2, MapChunkBulk.Trim, 0, MapChunkBulk.Trim.Length);

            byte[] DecompressedData = Decompressor.Decompress(MapChunkBulk.Trim);

            MapChunkBulkMetadata[] Data = MapChunkBulk.MetaInformation;

            int i = 0;
            foreach (MapChunkBulkMetadata d in MapChunkBulk.MetaInformation)
            {
                chunks[i] = new Chunk(Data[i].ChunkX, Data[i].ChunkZ, Data[i].PrimaryBitMap, Data[i].AddBitMap,
                    MapChunkBulk.SkyLightSent, true); // -- Assume true for Ground Up Continuous

                DecompressedData = chunks[i].GetData(DecompressedData);
                // -- Calls the chunk class to take all of the bytes it needs, and return whats left.

                if (World == null)
                    World = new World();

                World.WorldChunks.Add(chunks[i]);
                i++;
            }
        }

        private void OnExplosion(IPacket packet)
        {
        }

        private void OnEffect(IPacket packet)
        {
            var Effect = (EffectPacket) packet;

            PlayEffect(Effect.EffectID, Effect.X, (byte) Effect.Y,
                Effect.Z, Effect.Data, Effect.DisableRelativeVolume);
        }

        private void OnSoundEffect(IPacket packet)
        {
            var SoundEffect = (SoundEffectPacket) packet;

            PlaySound(SoundEffect.SoundName, SoundEffect.X, SoundEffect.Y,
                SoundEffect.Z, SoundEffect.Volume, SoundEffect.Pitch);
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

            EditSign(SignEditorOpen.X, SignEditorOpen.Y, SignEditorOpen.Z);
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
            if (!PlayerList.ContainsKey(PlayerListItem.PlayerName))
                PlayerList.Add(PlayerListItem.PlayerName, PlayerListItem.Ping);
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
                    break;
            }
        }

        private void OnDisconnect(IPacket packet)
        {
        }

        #endregion
    }
}