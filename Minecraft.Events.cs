using System.Collections.Generic;
using System.Text;
using MineLib.ClientWrapper.BigData;
using MineLib.Network.Data.Anvil;
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

        private void OnKeepAlive(IPacket packet) // -- Works
        {
            var keepAlive = (KeepAlivePacket) packet;

            SendPacket(keepAlive);
        }

        private void OnJoinGame(IPacket packet) // -- Works
        {
            var joinGame = (JoinGamePacket) packet;

            Player.EntityID = joinGame.EntityID;

            World.Difficulty = joinGame.Difficulty;
            World.Dimension = joinGame.Dimension;
            World.GameMode = joinGame.GameMode;
            World.LevelType = joinGame.LevelType;
            World.MaxPlayers = joinGame.MaxPlayers;

            ReducedDebugInfo = joinGame.ReducedDebugInfo;
        }

        private void OnChatMessage(IPacket packet)
        {
            var chatMessage = (ChatMessagePacket) packet;

            // -- Debugging
            ChatHistory.Add(chatMessage.Message);
            // -- Debugging

            DisplayChatMessage(chatMessage.Message);
        }

        private void OnTimeUpdate(IPacket packet) // -- Works
        {
            var timeUpdate = (TimeUpdatePacket) packet;

            World.AgeOfTheWorld = timeUpdate.AgeOfTheWorld;
            World.TimeOfDay = timeUpdate.TimeOfDay;
        }

        private void OnEntityEquipment(IPacket packet)
        {
            var entityEquipment = (EntityEquipmentPacket) packet;

            if (!Entities.ContainsKey(entityEquipment.EntityID))
                Entities.Add(entityEquipment.EntityID, new Entity { EntityID = entityEquipment.EntityID });

            Entities[entityEquipment.EntityID].Equipment.Item = entityEquipment.Item;
            Entities[entityEquipment.EntityID].Equipment.Slot = entityEquipment.Slot;
        }

        private void OnSpawnPosition(IPacket packet) // -- Works
        {
            var spawnPosition = (SpawnPositionPacket) packet;

            World.SpawnPosition = spawnPosition.Location;
        }

        private void OnUpdateHealth(IPacket packet) // -- Works
        {
            var updateHealth = (UpdateHealthPacket) packet;

            Player.Health.Food = updateHealth.Food;
            Player.Health.FoodSaturation = updateHealth.FoodSaturation;
            Player.Health.Health = updateHealth.Health;
        }

        private bool playerstarted = false;
        private void OnRespawn(IPacket packet) // -- Works
        {
            var respawn = (RespawnPacket) packet;

            World.Dimension = respawn.Dimension;
            World.Difficulty = respawn.Difficulty;
            World.GameMode = respawn.GameMode;
            World.LevelType = respawn.LevelType;

            World.Chunks.Clear(); // And unload all chunks.
        }

        private void OnPlayerPositionAndLook(IPacket packet) // -- Works
        {
            var playerPositionAndLook = (PlayerPositionAndLookPacket) packet;

            // Force to new position.
            Player.Position.Vector3 = playerPositionAndLook.Vector3;
            Player.Look.Yaw = playerPositionAndLook.Yaw;
            Player.Look.Pitch = playerPositionAndLook.Pitch;
            Player.Position.OnGround = true;

            SendPacket(new Network.Packets.Client.PlayerPositionAndLookPacket
            {
                X = Player.Position.Vector3.X,
                FeetY = Player.Position.Vector3.Y,
                Z = Player.Position.Vector3.Z,
                Yaw = Player.Look.Yaw,
                Pitch = Player.Look.Pitch,
                OnGround = Player.Position.OnGround
            });

            if (!playerstarted)
            {
                StartPlayerTickHandler();
                playerstarted = true;
            }
        }

        private void OnHeldItemChange(IPacket packet) // -- Works
        {
            var heldItemChange = (HeldItemChangePacket) packet;

            Player.HeldItem = heldItemChange.Slot;
        }

        private void OnUseBed(IPacket packet) // -- Works
        {
            var useBed = (UseBedPacket) packet;

            if (!Entities.ContainsKey(useBed.EntityID))
                Entities.Add(useBed.EntityID, new Entity {EntityID = useBed.EntityID});

            Entities[useBed.EntityID].Bed = useBed.Location;
        }

        private void OnAnimation(IPacket packet) // -- Works
        {
            var animation = (AnimationPacket) packet;

            if (!Entities.ContainsKey(animation.EntityID))
                Entities.Add(animation.EntityID, new Entity {EntityID = animation.EntityID});

            Entities[animation.EntityID].Animation = animation.Animation;
        }

        private void OnSpawnPlayer(IPacket packet)
        {
            var spawnPlayer = (SpawnPlayerPacket) packet;

            if (!Entities.ContainsKey(spawnPlayer.EntityID))
                Entities.Add(spawnPlayer.EntityID, new Entity {EntityID = spawnPlayer.EntityID});

            Entities[spawnPlayer.EntityID].Player.UUID = spawnPlayer.PlayerUUID;
            Entities[spawnPlayer.EntityID].Position = spawnPlayer.Vector3;

            Entities[spawnPlayer.EntityID].Look.Yaw = spawnPlayer.Yaw;
            Entities[spawnPlayer.EntityID].Look.Pitch = spawnPlayer.Pitch;

            Entities[spawnPlayer.EntityID].Equipment.CurrentItem = spawnPlayer.CurrentItem;

            Entities[spawnPlayer.EntityID].Metadata = spawnPlayer.EntityMetadata;
        }

        private void OnCollectItem(IPacket packet)
        {
            var collectItem = (CollectItemPacket) packet;

            if (!Entities.ContainsKey(collectItem.CollectorEntityID))
                Entities.Add(collectItem.CollectorEntityID, new Entity {EntityID = collectItem.CollectorEntityID});

            //Entities[collectItem.CollectorEntityID] = collectItem.CollectedEntityID; TODO: CollectedID
        }

        private void OnSpawnObject(IPacket packet)
        {
            var spawnObject = (SpawnObjectPacket) packet;
        }

        private void OnSpawnMob(IPacket packet)
        {
            var spawnMob = (SpawnMobPacket) packet;

            if (!Entities.ContainsKey(spawnMob.EntityID))
                Entities.Add(spawnMob.EntityID, new Entity {EntityID = spawnMob.EntityID});

            Entities[spawnMob.EntityID].Type = spawnMob.Type;

            Entities[spawnMob.EntityID].Position = spawnMob.Vector3;

            Entities[spawnMob.EntityID].Look.Yaw = spawnMob.Yaw;
            Entities[spawnMob.EntityID].Look.Pitch = spawnMob.Pitch;
            Entities[spawnMob.EntityID].Look.HeadPitch = spawnMob.HeadPitch;

            Entities[spawnMob.EntityID].Velocity.VelocityX = spawnMob.VelocityX;
            Entities[spawnMob.EntityID].Velocity.VelocityY = spawnMob.VelocityY;
            Entities[spawnMob.EntityID].Velocity.VelocityZ = spawnMob.VelocityZ;

            Entities[spawnMob.EntityID].Metadata = spawnMob.EntityMetadata;
        }

        private void OnSpawnPainting(IPacket packet)
        {
            var spawnPainting = (SpawnPaintingPacket) packet;
        }

        private void OnSpawnExperienceOrb(IPacket packet)
        {
            var spawnExperienceOrb = (SpawnExperienceOrbPacket) packet;
        }

        private void OnEntityVelocity(IPacket packet)
        {
            var entityVelocity = (EntityVelocityPacket) packet;

            if (!Entities.ContainsKey(entityVelocity.EntityID))
                Entities.Add(entityVelocity.EntityID, new Entity {EntityID = entityVelocity.EntityID});

            Entities[entityVelocity.EntityID].Velocity.VelocityX = entityVelocity.VelocityX;
            Entities[entityVelocity.EntityID].Velocity.VelocityY = entityVelocity.VelocityY;
            Entities[entityVelocity.EntityID].Velocity.VelocityZ = entityVelocity.VelocityZ;
        }

        private void OnDestroyEntities(IPacket packet) // -- Works
        {
            var destroyEntities = (DestroyEntitiesPacket) packet;

            foreach (int t in destroyEntities.EntityIDs)
            {
                Entities.Remove(t);
            }
        }

        private void OnEntity(IPacket packet)
        {
            var entity = (EntityPacket) packet;

            if (!Entities.ContainsKey(entity.EntityID))
                Entities.Add(entity.EntityID, new Entity {EntityID = entity.EntityID});
        }

        private void OnEntityRelativeMove(IPacket packet)
        {
            var entityRelativeMove = (EntityRelativeMovePacket) packet;

            if (!Entities.ContainsKey(entityRelativeMove.EntityID))
                Entities.Add(entityRelativeMove.EntityID, new Entity {EntityID = entityRelativeMove.EntityID});

            Entities[entityRelativeMove.EntityID].Position = entityRelativeMove.DeltaVector3; //Nope

            Entities[entityRelativeMove.EntityID].OnGround = entityRelativeMove.OnGround;
        }

        private void OnEntityLook(IPacket packet)
        {
            var entityLook = (EntityLookPacket) packet;

            if (!Entities.ContainsKey(entityLook.EntityID))
                Entities.Add(entityLook.EntityID, new Entity {EntityID = entityLook.EntityID});

            Entities[entityLook.EntityID].Look.Yaw = entityLook.Yaw;
            Entities[entityLook.EntityID].Look.Pitch = entityLook.Pitch;

            Entities[entityLook.EntityID].OnGround = entityLook.OnGround;
        }

        private void OnEntityLookAndRelativeMove(IPacket packet)
        {
            var entityLookAndRelativeMove = (EntityLookAndRelativeMovePacket) packet;

            if (!Entities.ContainsKey(entityLookAndRelativeMove.EntityID))
                Entities.Add(entityLookAndRelativeMove.EntityID,
                    new Entity {EntityID = entityLookAndRelativeMove.EntityID});

            Entities[entityLookAndRelativeMove.EntityID].Position = entityLookAndRelativeMove.DeltaVector3; //Nope

            Entities[entityLookAndRelativeMove.EntityID].Look.Yaw = entityLookAndRelativeMove.Yaw;
            Entities[entityLookAndRelativeMove.EntityID].Look.Pitch = entityLookAndRelativeMove.Pitch;

            Entities[entityLookAndRelativeMove.EntityID].OnGround = entityLookAndRelativeMove.OnGround;
        }

        private void OnEntityTeleport(IPacket packet)
        {
            var entityTeleport = (EntityTeleportPacket) packet;

            if (!Entities.ContainsKey(entityTeleport.EntityID))
                Entities.Add(entityTeleport.EntityID, new Entity {EntityID = entityTeleport.EntityID});

            Entities[entityTeleport.EntityID].Position = entityTeleport.Vector3;

            Entities[entityTeleport.EntityID].Look.Yaw = entityTeleport.Yaw;
            Entities[entityTeleport.EntityID].Look.Pitch = entityTeleport.Pitch;

            Entities[entityTeleport.EntityID].OnGround = entityTeleport.OnGround;
        }

        private void OnEntityHeadLook(IPacket packet)
        {
            var entityHeadLook = (EntityHeadLookPacket) packet;

            if (!Entities.ContainsKey(entityHeadLook.EntityID))
                Entities.Add(entityHeadLook.EntityID, new Entity {EntityID = entityHeadLook.EntityID});

            Entities[entityHeadLook.EntityID].Look.HeadYaw = entityHeadLook.HeadYaw;
        }

        private void OnEntityStatus(IPacket packet)
        {
            var entityStatus = (EntityStatusPacket) packet;

            if (!Entities.ContainsKey(entityStatus.EntityID))
                Entities.Add(entityStatus.EntityID, new Entity {EntityID = entityStatus.EntityID});

            Entities[entityStatus.EntityID].Status = entityStatus.Status;
        }

        private void OnAttachEntity(IPacket packet)
        {
            var attachEntity = (AttachEntityPacket) packet;

            if (!Entities.ContainsKey(attachEntity.EntityID))
                Entities.Add(attachEntity.EntityID, new Entity {EntityID = attachEntity.EntityID});

            Entities[attachEntity.EntityID].Vehicle.VehicleID = attachEntity.VehicleID;

            Entities[attachEntity.EntityID].Leash = attachEntity.Leash;
        }

        private void OnEntityMetadata(IPacket packet)
        {
            var entityMetadata = (EntityMetadataPacket) packet;

            if (!Entities.ContainsKey(entityMetadata.EntityID))
                Entities.Add(entityMetadata.EntityID, new Entity {EntityID = entityMetadata.EntityID});

            Entities[entityMetadata.EntityID].Metadata = entityMetadata.Metadata;
        }

        private void OnEntityEffect(IPacket packet)
        {
            var entityEffect = (EntityEffectPacket) packet;

            if (Player.EntityID == entityEffect.EntityID)
            {
                Player.Effects.Add(new PlayerEffect
                {
                    EffectID = entityEffect.EffectID,
                    Amplifier = entityEffect.Amplifier,
                    Duration = entityEffect.Duration,
                    HideParticle = entityEffect.HideParticles
                });
            }
            else
            {
                if (Entities.ContainsKey(entityEffect.EntityID))
                    return;

                Entities.Add(entityEffect.EntityID, new Entity {EntityID = entityEffect.EntityID});
                Entities[entityEffect.EntityID].Effects.Add(new EntityEffect
                {
                    EffectID = entityEffect.EffectID,
                    Amplifier = entityEffect.Amplifier,
                    Duration = entityEffect.Duration,
                    HideParticle = entityEffect.HideParticles
                });
            }
        }

        private void OnRemoveEntityEffect(IPacket packet)
        {
            var removeEntityEffect = (RemoveEntityEffectPacket) packet;

            if (Player.EntityID == removeEntityEffect.EntityID)
            {
                foreach (var effect in Player.Effects.ToArray())
                {
                    if (effect.EffectID == removeEntityEffect.EffectID)
                        Player.Effects.Remove(effect);
                }
            }
            else
            {
                if (!Entities.ContainsKey(removeEntityEffect.EntityID))
                    Entities.Add(removeEntityEffect.EntityID, new Entity {EntityID = removeEntityEffect.EntityID});

                foreach (var effect in Entities[removeEntityEffect.EntityID].Effects.ToArray())
                {
                    if (effect.EffectID == removeEntityEffect.EffectID)
                        Entities[removeEntityEffect.EntityID].Effects.Remove(effect);
                }
            }
        }

        private void OnSetExperience(IPacket packet)
        {
            var setExperience = (SetExperiencePacket) packet;

            Player.Experience.ExperienceBar = setExperience.ExperienceBar;
            Player.Experience.Level = setExperience.Level;
            Player.Experience.TotalExperience = setExperience.TotalExperience;
        }

        private void OnEntityProperties(IPacket packet)
        {
            var entityProperties = (EntityPropertiesPacket) packet;

            if (!Entities.ContainsKey(entityProperties.EntityID))
                Entities.Add(entityProperties.EntityID, new Entity {EntityID = entityProperties.EntityID});

            Entities[entityProperties.EntityID].Properties = entityProperties.EntityProperties;
        }

        private void OnChunkData(IPacket packet) // -- Works
        {
            var chunkData = (ChunkDataPacket) packet;

            if (World == null)
                World = new World();

            if (chunkData.Chunk.PrimaryBitMap == 0)
            {
                World.RemoveChunk(chunkData.Chunk.Coordinates);
                return;
            }

            // -- Add the chunk to the world
            World.SetChunk(chunkData.Chunk);     
        }

        private void OnMultiBlockChange(IPacket packet) // -- Works
        {
            var multiBlockChange = (MultiBlockChangePacket) packet;

            foreach (var record in multiBlockChange.RecordList.GetRecords())
            {
                var id = (short) (record.BlockIDMeta >> 4);
                var meta = (byte)(record.BlockIDMeta & 15);
                var block = new Block(id, meta);

                World.SetBlock(record.Coordinates, multiBlockChange.Coordinates, block);
            }      
        }

        private void OnBlockChange(IPacket packet) // -- Works
        {
            var blockChange = (BlockChangePacket) packet;

            var id = (short) (blockChange.BlockIDMeta >> 4);
            var meta = (byte)(blockChange.BlockIDMeta & 15);

            var block = new Block(id, meta);
            
            World.SetBlock(blockChange.Location, block);
        }

        private void OnBlockAction(IPacket packet)
        {
            var blockAction = (BlockActionPacket) packet;
        }

        private void OnBlockBreakAnimation(IPacket packet)
        {
            var blockBreakAnimation = (BlockBreakAnimationPacket)packet;
        }

        private void OnMapChunkBulk(IPacket packet) //Bug: Nope
        {
            var mapChunkBulk = (MapChunkBulkPacket) packet;

            if (World == null)
                World = new World();

            foreach (var chunk in mapChunkBulk.ChunkList.GetChunk())
                World.SetChunk(chunk);
        }

        private void OnExplosion(IPacket packet)
        {
            var explosion = (ExplosionPacket) packet;
        }

        private void OnEffect(IPacket packet)
        {
            var effect = (EffectPacket) packet;

            PlayEffect(effect.EffectID, effect.Location, effect.Data, effect.DisableRelativeVolume);
        }

        private void OnSoundEffect(IPacket packet)
        {
            var soundEffect = (SoundEffectPacket) packet;

            PlaySound(soundEffect.SoundName, soundEffect.Coordinates, soundEffect.Volume, soundEffect.Pitch);
        }

        private void OnParticle(IPacket packet)
        {
            var particle = (ParticlePacket) packet;

            //CreateParticle();
        }

        private void OnChangeGameState(IPacket packet)
        {
            var changeGameState = (ChangeGameStatePacket) packet;

            World.StateChanged.Reason = changeGameState.Reason;
            World.StateChanged.Value = changeGameState.Value;
        }

        private void OnSpawnGlobalEntity(IPacket packet)
        {
            var spawnGlobalEntity = (SpawnGlobalEntityPacket)packet;
        }

        private void OnOpenWindow(IPacket packet)
        {
            var openWindow = (OpenWindowPacket) packet;

            Player.OpenWindow(openWindow.WindowID, openWindow.InventoryType, openWindow.WindowTitle, openWindow.NumberOfSlots, openWindow.EntityID);
        }

        private void OnCloseWindow(IPacket packet)
        {
            var closeWindow = (CloseWindowPacket) packet;

            Player.CloseWindow(closeWindow.WindowID);
        }

        private void OnSetSlot(IPacket packet)
        {
            var setSlot = (SetSlotPacket) packet;

            //Player.SetSlot(setSlot.WindowId, setSlot.Slot, setSlot.SlotData);
        }

        private void OnWindowItems(IPacket packet)
        {
            var windowItems = (WindowItemsPacket) packet;

            Player.SetWindowItems(windowItems.WindowID, windowItems.ItemStackList);
        }

        private void OnWindowProperty(IPacket packet)
        {
            var windowProperty = (WindowPropertyPacket) packet;
        }

        private void OnConfirmTransaction(IPacket packet)
        {
            var confirmTransaction = (ConfirmTransactionPacket) packet;

            Player.ConfirmTransaction(confirmTransaction.WindowId, confirmTransaction.ActionNumber, confirmTransaction.Accepted);
        }

        private void OnUpdateSign(IPacket packet)
        {
            var updateSign = (UpdateSignPacket) packet;
        }

        private void OnMaps(IPacket packet)
        {
            var maps = (MapsPacket) packet;
        }

        private void OnUpdateBlockEntity(IPacket packet)
        {
            var updateBlockEntity = (UpdateBlockEntityPacket) packet;
        }

        private void OnSignEditorOpen(IPacket packet)
        {
            var signEditorOpen = (SignEditorOpenPacket) packet;

            EditSign(signEditorOpen.Location);
        }

        private void OnStatistics(IPacket packet)
        {
            var statistics = (StatisticsPacket) packet;

            Player.Statistics = statistics.StatisticsEntryList;
        }

        private void OnPlayerListItem(IPacket packet)
        {
            var playerListItem = (PlayerListItemPacket) packet;

            // Maybe Clear PlayerList?
            //if (!PlayersList.ContainsKey(playerListItem.PlayerName))
            //    PlayersList.Add(playerListItem.PlayerName, playerListItem.Ping);
        }

        private void OnPlayerAbilities(IPacket packet)
        {
            var playerAbilities = (PlayerAbilitiesPacket) packet;

            Player.Abilities.Flags = playerAbilities.Flags;
            Player.Abilities.FlyingSpeed = playerAbilities.FlyingSpeed;
            Player.Abilities.WalkingSpeed = playerAbilities.WalkingSpeed;
        }

        private void OnTabComplete(IPacket packet)
        {
            var tabComplete = (TabCompletePacket) packet;
        }

        private void OnScoreboardObjective(IPacket packet)
        {
            var scoreboardObjective = (ScoreboardObjectivePacket) packet;
        }

        private void OnUpdateScore(IPacket packet)
        {
            var updateScore = (UpdateScorePacket) packet;
        }

        private void OnDisplayScoreboard(IPacket packet)
        {
            var displayScoreboard = (DisplayScoreboardPacket) packet;
        }

        private void OnTeams(IPacket packet)
        {
            var teams = (TeamsPacket) packet;
        }

        private void OnPluginMessage(IPacket packet)
        {
            var pluginMessage = (PluginMessagePacket) packet;

            switch (pluginMessage.Channel)
            {
                case "MC|Brand":
                    ServerBrand = Encoding.UTF8.GetString(pluginMessage.Data);
                    break;

                default:
                    PluginMessageUnhandled.Add(pluginMessage.Channel + " : " + Encoding.UTF8.GetString(pluginMessage.Data));
                    break;
            }
        }

        private void OnDisconnect(IPacket packet)
        {
            var disconnect = (DisconnectPacket)packet;
        }

        #endregion
    }
}