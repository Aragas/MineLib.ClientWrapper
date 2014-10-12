using System.Collections.Generic;
using System.Text;
using MineLib.ClientWrapper.Modern.BigData;
using MineLib.Network;
using MineLib.Network.Events;
using MineLib.Network.Modern.Data.Anvil;
using MineLib.Network.Modern.Packets.Server;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        // -- Debugging
        public readonly List<string> ChatHistory = new List<string>();
        public readonly List<string> PluginMessageUnhandled = new List<string>();
        // -- Debugging

        public event PacketHandler FirePacketHandled;

        #region Voids

        private void OnKeepAlive(IPacket packet) // -- Works
        {
            var keepAlive = (KeepAlivePacket) packet;

            SendPacket(keepAlive);
        }

        private void OnJoinGame(IPacket packet) // -- Works
        {
            var joinGame = (JoinGamePacket) packet;

            ModernPlayer.EntityID = joinGame.EntityID;

            ModernWorld.Difficulty = joinGame.Difficulty;
            ModernWorld.Dimension = joinGame.Dimension;
            ModernWorld.GameMode = joinGame.GameMode;
            ModernWorld.LevelType = joinGame.LevelType;
            ModernWorld.MaxPlayers = joinGame.MaxPlayers;

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

            ModernWorld.AgeOfTheWorld = timeUpdate.AgeOfTheWorld;
            ModernWorld.TimeOfDay = timeUpdate.TimeOfDay;
        }

        private void OnEntityEquipment(IPacket packet)
        {
            var entityEquipment = (EntityEquipmentPacket) packet;

            if (!ModernEntities.ContainsKey(entityEquipment.EntityID))
                ModernEntities.Add(entityEquipment.EntityID, new Entity { EntityID = entityEquipment.EntityID });

            ModernEntities[entityEquipment.EntityID].Equipment.Item = entityEquipment.Item;
            ModernEntities[entityEquipment.EntityID].Equipment.Slot = entityEquipment.Slot;
        }

        private void OnSpawnPosition(IPacket packet) // -- Works
        {
            var spawnPosition = (SpawnPositionPacket) packet;

            ModernWorld.SpawnPosition = spawnPosition.Location;
        }

        private void OnUpdateHealth(IPacket packet) // -- Works
        {
            var updateHealth = (UpdateHealthPacket) packet;

            ModernPlayer.Health.Food = updateHealth.Food;
            ModernPlayer.Health.FoodSaturation = updateHealth.FoodSaturation;
            ModernPlayer.Health.Health = updateHealth.Health;
        }

        private void OnRespawn(IPacket packet) // -- Works
        {
            var respawn = (RespawnPacket) packet;

            ModernWorld.Dimension = respawn.Dimension;
            ModernWorld.Difficulty = respawn.Difficulty;
            ModernWorld.GameMode = respawn.GameMode;
            ModernWorld.LevelType = respawn.LevelType;

            ModernWorld.Chunks.Clear(); // And unload all chunks.
        }

        private bool playerstarted = false;
        private void OnPlayerPositionAndLook(IPacket packet) // -- Works
        {
            var playerPositionAndLook = (PlayerPositionAndLookPacket) packet;

            // Force to new position.
            ModernPlayer.Position.Vector3 = playerPositionAndLook.Vector3;
            ModernPlayer.Look.Yaw = playerPositionAndLook.Yaw;
            ModernPlayer.Look.Pitch = playerPositionAndLook.Pitch;
            ModernPlayer.Position.OnGround = true;

            SendPacket(new Network.Modern.Packets.Client.PlayerPositionAndLookPacket
            {
                X = ModernPlayer.Position.Vector3.X,
                FeetY = ModernPlayer.Position.Vector3.Y,
                Z = ModernPlayer.Position.Vector3.Z,
                Yaw = ModernPlayer.Look.Yaw,
                Pitch = ModernPlayer.Look.Pitch,
                OnGround = ModernPlayer.Position.OnGround
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

            ModernPlayer.HeldItem = heldItemChange.Slot;
        }

        private void OnUseBed(IPacket packet) // -- Works
        {
            var useBed = (UseBedPacket) packet;

            if (!ModernEntities.ContainsKey(useBed.EntityID))
                ModernEntities.Add(useBed.EntityID, new Entity {EntityID = useBed.EntityID});

            ModernEntities[useBed.EntityID].Bed = useBed.Location;
        }

        private void OnAnimation(IPacket packet) // -- Works
        {
            var animation = (AnimationPacket) packet;

            if (!ModernEntities.ContainsKey(animation.EntityID))
                ModernEntities.Add(animation.EntityID, new Entity {EntityID = animation.EntityID});

            ModernEntities[animation.EntityID].Animation = animation.Animation;
        }

        private void OnSpawnPlayer(IPacket packet)
        {
            var spawnPlayer = (SpawnPlayerPacket) packet;

            if (!ModernEntities.ContainsKey(spawnPlayer.EntityID))
                ModernEntities.Add(spawnPlayer.EntityID, new Entity {EntityID = spawnPlayer.EntityID});

            ModernEntities[spawnPlayer.EntityID].Player.UUID = spawnPlayer.PlayerUUID;
            ModernEntities[spawnPlayer.EntityID].Position = spawnPlayer.Vector3;

            ModernEntities[spawnPlayer.EntityID].Look.Yaw = spawnPlayer.Yaw;
            ModernEntities[spawnPlayer.EntityID].Look.Pitch = spawnPlayer.Pitch;

            ModernEntities[spawnPlayer.EntityID].Equipment.CurrentItem = spawnPlayer.CurrentItem;

            ModernEntities[spawnPlayer.EntityID].Metadata = spawnPlayer.EntityMetadata;
        }

        private void OnCollectItem(IPacket packet)
        {
            var collectItem = (CollectItemPacket) packet;

            if (!ModernEntities.ContainsKey(collectItem.CollectorEntityID))
                ModernEntities.Add(collectItem.CollectorEntityID, new Entity {EntityID = collectItem.CollectorEntityID});

            //Entities[collectItem.CollectorEntityID] = collectItem.CollectedEntityID; TODO: CollectedID
        }

        private void OnSpawnObject(IPacket packet)
        {
            var spawnObject = (SpawnObjectPacket) packet;
        }

        private void OnSpawnMob(IPacket packet)
        {
            var spawnMob = (SpawnMobPacket) packet;

            if (!ModernEntities.ContainsKey(spawnMob.EntityID))
                ModernEntities.Add(spawnMob.EntityID, new Entity {EntityID = spawnMob.EntityID});

            ModernEntities[spawnMob.EntityID].Type = spawnMob.Type;

            ModernEntities[spawnMob.EntityID].Position = spawnMob.Vector3;

            ModernEntities[spawnMob.EntityID].Look.Yaw = spawnMob.Yaw;
            ModernEntities[spawnMob.EntityID].Look.Pitch = spawnMob.Pitch;
            ModernEntities[spawnMob.EntityID].Look.HeadPitch = spawnMob.HeadPitch;

            ModernEntities[spawnMob.EntityID].Velocity.VelocityX = spawnMob.VelocityX;
            ModernEntities[spawnMob.EntityID].Velocity.VelocityY = spawnMob.VelocityY;
            ModernEntities[spawnMob.EntityID].Velocity.VelocityZ = spawnMob.VelocityZ;

            ModernEntities[spawnMob.EntityID].Metadata = spawnMob.EntityMetadata;
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

            if (!ModernEntities.ContainsKey(entityVelocity.EntityID))
                ModernEntities.Add(entityVelocity.EntityID, new Entity {EntityID = entityVelocity.EntityID});

            ModernEntities[entityVelocity.EntityID].Velocity.VelocityX = entityVelocity.VelocityX;
            ModernEntities[entityVelocity.EntityID].Velocity.VelocityY = entityVelocity.VelocityY;
            ModernEntities[entityVelocity.EntityID].Velocity.VelocityZ = entityVelocity.VelocityZ;
        }

        private void OnDestroyEntities(IPacket packet) // -- Works
        {
            var destroyEntities = (DestroyEntitiesPacket) packet;

            foreach (int t in destroyEntities.EntityIDs)
            {
                ModernEntities.Remove(t);
            }
        }

        private void OnEntity(IPacket packet)
        {
            var entity = (EntityPacket) packet;

            if (!ModernEntities.ContainsKey(entity.EntityID))
                ModernEntities.Add(entity.EntityID, new Entity {EntityID = entity.EntityID});
        }

        private void OnEntityRelativeMove(IPacket packet)
        {
            var entityRelativeMove = (EntityRelativeMovePacket) packet;

            if (!ModernEntities.ContainsKey(entityRelativeMove.EntityID))
                ModernEntities.Add(entityRelativeMove.EntityID, new Entity {EntityID = entityRelativeMove.EntityID});

            ModernEntities[entityRelativeMove.EntityID].Position = entityRelativeMove.DeltaVector3; //Nope

            ModernEntities[entityRelativeMove.EntityID].OnGround = entityRelativeMove.OnGround;
        }

        private void OnEntityLook(IPacket packet)
        {
            var entityLook = (EntityLookPacket) packet;

            if (!ModernEntities.ContainsKey(entityLook.EntityID))
                ModernEntities.Add(entityLook.EntityID, new Entity {EntityID = entityLook.EntityID});

            ModernEntities[entityLook.EntityID].Look.Yaw = entityLook.Yaw;
            ModernEntities[entityLook.EntityID].Look.Pitch = entityLook.Pitch;

            ModernEntities[entityLook.EntityID].OnGround = entityLook.OnGround;
        }

        private void OnEntityLookAndRelativeMove(IPacket packet)
        {
            var entityLookAndRelativeMove = (EntityLookAndRelativeMovePacket) packet;

            if (!ModernEntities.ContainsKey(entityLookAndRelativeMove.EntityID))
                ModernEntities.Add(entityLookAndRelativeMove.EntityID,
                    new Entity {EntityID = entityLookAndRelativeMove.EntityID});

            ModernEntities[entityLookAndRelativeMove.EntityID].Position = entityLookAndRelativeMove.DeltaVector3; //Nope

            ModernEntities[entityLookAndRelativeMove.EntityID].Look.Yaw = entityLookAndRelativeMove.Yaw;
            ModernEntities[entityLookAndRelativeMove.EntityID].Look.Pitch = entityLookAndRelativeMove.Pitch;

            ModernEntities[entityLookAndRelativeMove.EntityID].OnGround = entityLookAndRelativeMove.OnGround;
        }

        private void OnEntityTeleport(IPacket packet)
        {
            var entityTeleport = (EntityTeleportPacket) packet;

            if (!ModernEntities.ContainsKey(entityTeleport.EntityID))
                ModernEntities.Add(entityTeleport.EntityID, new Entity {EntityID = entityTeleport.EntityID});

            ModernEntities[entityTeleport.EntityID].Position = entityTeleport.Vector3;

            ModernEntities[entityTeleport.EntityID].Look.Yaw = entityTeleport.Yaw;
            ModernEntities[entityTeleport.EntityID].Look.Pitch = entityTeleport.Pitch;

            ModernEntities[entityTeleport.EntityID].OnGround = entityTeleport.OnGround;
        }

        private void OnEntityHeadLook(IPacket packet)
        {
            var entityHeadLook = (EntityHeadLookPacket) packet;

            if (!ModernEntities.ContainsKey(entityHeadLook.EntityID))
                ModernEntities.Add(entityHeadLook.EntityID, new Entity {EntityID = entityHeadLook.EntityID});

            ModernEntities[entityHeadLook.EntityID].Look.HeadYaw = entityHeadLook.HeadYaw;
        }

        private void OnEntityStatus(IPacket packet)
        {
            var entityStatus = (EntityStatusPacket) packet;

            if (!ModernEntities.ContainsKey(entityStatus.EntityID))
                ModernEntities.Add(entityStatus.EntityID, new Entity {EntityID = entityStatus.EntityID});

            ModernEntities[entityStatus.EntityID].Status = entityStatus.Status;
        }

        private void OnAttachEntity(IPacket packet)
        {
            var attachEntity = (AttachEntityPacket) packet;

            if (!ModernEntities.ContainsKey(attachEntity.EntityID))
                ModernEntities.Add(attachEntity.EntityID, new Entity {EntityID = attachEntity.EntityID});

            ModernEntities[attachEntity.EntityID].Vehicle.VehicleID = attachEntity.VehicleID;

            ModernEntities[attachEntity.EntityID].Leash = attachEntity.Leash;
        }

        private void OnEntityMetadata(IPacket packet)
        {
            var entityMetadata = (EntityMetadataPacket) packet;

            if (!ModernEntities.ContainsKey(entityMetadata.EntityID))
                ModernEntities.Add(entityMetadata.EntityID, new Entity {EntityID = entityMetadata.EntityID});

            ModernEntities[entityMetadata.EntityID].Metadata = entityMetadata.Metadata;
        }

        private void OnEntityEffect(IPacket packet)
        {
            var entityEffect = (EntityEffectPacket) packet;

            if (ModernPlayer.EntityID == entityEffect.EntityID)
            {
                ModernPlayer.Effects.Add(new PlayerEffect
                {
                    EffectID = entityEffect.EffectID,
                    Amplifier = entityEffect.Amplifier,
                    Duration = entityEffect.Duration,
                    HideParticle = entityEffect.HideParticles
                });
            }
            else
            {
                if (ModernEntities.ContainsKey(entityEffect.EntityID))
                    return;

                ModernEntities.Add(entityEffect.EntityID, new Entity {EntityID = entityEffect.EntityID});
                ModernEntities[entityEffect.EntityID].Effects.Add(new EntityEffect
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

            if (ModernPlayer.EntityID == removeEntityEffect.EntityID)
            {
                foreach (var effect in ModernPlayer.Effects.ToArray())
                {
                    if (effect.EffectID == removeEntityEffect.EffectID)
                        ModernPlayer.Effects.Remove(effect);
                }
            }
            else
            {
                if (!ModernEntities.ContainsKey(removeEntityEffect.EntityID))
                    ModernEntities.Add(removeEntityEffect.EntityID, new Entity {EntityID = removeEntityEffect.EntityID});

                foreach (var effect in ModernEntities[removeEntityEffect.EntityID].Effects.ToArray())
                {
                    if (effect.EffectID == removeEntityEffect.EffectID)
                        ModernEntities[removeEntityEffect.EntityID].Effects.Remove(effect);
                }
            }
        }

        private void OnSetExperience(IPacket packet)
        {
            var setExperience = (SetExperiencePacket) packet;

            ModernPlayer.Experience.ExperienceBar = setExperience.ExperienceBar;
            ModernPlayer.Experience.Level = setExperience.Level;
            ModernPlayer.Experience.TotalExperience = setExperience.TotalExperience;
        }

        private void OnEntityProperties(IPacket packet)
        {
            var entityProperties = (EntityPropertiesPacket) packet;

            if (!ModernEntities.ContainsKey(entityProperties.EntityID))
                ModernEntities.Add(entityProperties.EntityID, new Entity {EntityID = entityProperties.EntityID});

            ModernEntities[entityProperties.EntityID].Properties = entityProperties.EntityProperties;
        }

        private void OnChunkData(IPacket packet) // -- Works
        {
            var chunkData = (ChunkDataPacket) packet;

            if (ModernWorld == null)
                ModernWorld = new World();

            if (chunkData.Chunk.PrimaryBitMap == 0)
            {
                ModernWorld.RemoveChunk(chunkData.Chunk.Coordinates);
                return;
            }

            // -- Add the chunk to the world
            ModernWorld.SetChunk(chunkData.Chunk);     
        }

        private void OnMultiBlockChange(IPacket packet) // -- Works
        {
            var multiBlockChange = (MultiBlockChangePacket) packet;

            foreach (var record in multiBlockChange.RecordList.GetRecords())
            {
                var id = (short) (record.BlockIDMeta >> 4);
                var meta = (byte)(record.BlockIDMeta & 15);
                var block = new Block(id, meta);

                ModernWorld.SetBlock(record.Coordinates, multiBlockChange.Coordinates, block);
            }      
        }

        private void OnBlockChange(IPacket packet) // -- Works
        {
            var blockChange = (BlockChangePacket) packet;

            var id = (short) (blockChange.BlockIDMeta >> 4);
            var meta = (byte)(blockChange.BlockIDMeta & 15);

            var block = new Block(id, meta);
            
            ModernWorld.SetBlock(blockChange.Location, block);
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

            if (ModernWorld == null)
                ModernWorld = new World();

            foreach (var chunk in mapChunkBulk.ChunkList.GetChunk())
                ModernWorld.SetChunk(chunk);
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

            ModernWorld.StateChanged.Reason = changeGameState.Reason;
            ModernWorld.StateChanged.Value = changeGameState.Value;
        }

        private void OnSpawnGlobalEntity(IPacket packet)
        {
            var spawnGlobalEntity = (SpawnGlobalEntityPacket)packet;
        }

        private void OnOpenWindow(IPacket packet)
        {
            var openWindow = (OpenWindowPacket) packet;

            ModernPlayer.OpenWindow(openWindow.WindowID, openWindow.InventoryType, openWindow.WindowTitle, openWindow.NumberOfSlots, openWindow.EntityID);
        }

        private void OnCloseWindow(IPacket packet)
        {
            var closeWindow = (CloseWindowPacket) packet;

            ModernPlayer.CloseWindow(closeWindow.WindowID);
        }

        private void OnSetSlot(IPacket packet)
        {
            var setSlot = (SetSlotPacket) packet;

            //Player.SetSlot(setSlot.WindowId, setSlot.Slot, setSlot.SlotData);
        }

        private void OnWindowItems(IPacket packet)
        {
            var windowItems = (WindowItemsPacket) packet;

            ModernPlayer.SetWindowItems(windowItems.WindowID, windowItems.ItemStackList);
        }

        private void OnWindowProperty(IPacket packet)
        {
            var windowProperty = (WindowPropertyPacket) packet;
        }

        private void OnConfirmTransaction(IPacket packet)
        {
            var confirmTransaction = (ConfirmTransactionPacket) packet;

            ModernPlayer.ConfirmTransaction(confirmTransaction.WindowId, confirmTransaction.ActionNumber, confirmTransaction.Accepted);
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

            ModernPlayer.Statistics = statistics.StatisticsEntryList;
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

            ModernPlayer.Abilities.Flags = playerAbilities.Flags;
            ModernPlayer.Abilities.FlyingSpeed = playerAbilities.FlyingSpeed;
            ModernPlayer.Abilities.WalkingSpeed = playerAbilities.WalkingSpeed;
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