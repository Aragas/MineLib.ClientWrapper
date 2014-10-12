using System.Collections.Generic;
using System.Text;
using MineLib.ClientWrapper.Main.BigData;
using MineLib.Network;
using MineLib.Network.Events;
using MineLib.Network.Main.Data.Anvil;
using MineLib.Network.Main.Packets.Server;

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

            MainPlayer.EntityID = joinGame.EntityID;

            MainWorld.Difficulty = joinGame.Difficulty;
            MainWorld.Dimension = joinGame.Dimension;
            MainWorld.GameMode = joinGame.GameMode;
            MainWorld.LevelType = joinGame.LevelType;
            MainWorld.MaxPlayers = joinGame.MaxPlayers;

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

            MainWorld.AgeOfTheWorld = timeUpdate.AgeOfTheWorld;
            MainWorld.TimeOfDay = timeUpdate.TimeOfDay;
        }

        private void OnEntityEquipment(IPacket packet)
        {
            var entityEquipment = (EntityEquipmentPacket) packet;

            if (!MainEntities.ContainsKey(entityEquipment.EntityID))
                MainEntities.Add(entityEquipment.EntityID, new Entity { EntityID = entityEquipment.EntityID });

            MainEntities[entityEquipment.EntityID].Equipment.Item = entityEquipment.Item;
            MainEntities[entityEquipment.EntityID].Equipment.Slot = entityEquipment.Slot;
        }

        private void OnSpawnPosition(IPacket packet) // -- Works
        {
            var spawnPosition = (SpawnPositionPacket) packet;

            MainWorld.SpawnPosition = spawnPosition.Location;
        }

        private void OnUpdateHealth(IPacket packet) // -- Works
        {
            var updateHealth = (UpdateHealthPacket) packet;

            MainPlayer.Health.Food = updateHealth.Food;
            MainPlayer.Health.FoodSaturation = updateHealth.FoodSaturation;
            MainPlayer.Health.Health = updateHealth.Health;
        }

        private void OnRespawn(IPacket packet) // -- Works
        {
            var respawn = (RespawnPacket) packet;

            MainWorld.Dimension = respawn.Dimension;
            MainWorld.Difficulty = respawn.Difficulty;
            MainWorld.GameMode = respawn.GameMode;
            MainWorld.LevelType = respawn.LevelType;

            MainWorld.Chunks.Clear(); // And unload all chunks.
        }

        private bool playerstarted = false;
        private void OnPlayerPositionAndLook(IPacket packet) // -- Works
        {
            var playerPositionAndLook = (PlayerPositionAndLookPacket) packet;

            // Force to new position.
            MainPlayer.Position.Vector3 = playerPositionAndLook.Vector3;
            MainPlayer.Look.Yaw = playerPositionAndLook.Yaw;
            MainPlayer.Look.Pitch = playerPositionAndLook.Pitch;
            MainPlayer.Position.OnGround = true;

            SendPacket(new Network.Main.Packets.Client.PlayerPositionAndLookPacket
            {
                X = MainPlayer.Position.Vector3.X,
                FeetY = MainPlayer.Position.Vector3.Y,
                Z = MainPlayer.Position.Vector3.Z,
                Yaw = MainPlayer.Look.Yaw,
                Pitch = MainPlayer.Look.Pitch,
                OnGround = MainPlayer.Position.OnGround
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

            MainPlayer.HeldItem = heldItemChange.Slot;
        }

        private void OnUseBed(IPacket packet) // -- Works
        {
            var useBed = (UseBedPacket) packet;

            if (!MainEntities.ContainsKey(useBed.EntityID))
                MainEntities.Add(useBed.EntityID, new Entity {EntityID = useBed.EntityID});

            MainEntities[useBed.EntityID].Bed = useBed.Location;
        }

        private void OnAnimation(IPacket packet) // -- Works
        {
            var animation = (AnimationPacket) packet;

            if (!MainEntities.ContainsKey(animation.EntityID))
                MainEntities.Add(animation.EntityID, new Entity {EntityID = animation.EntityID});

            MainEntities[animation.EntityID].Animation = animation.Animation;
        }

        private void OnSpawnPlayer(IPacket packet)
        {
            var spawnPlayer = (SpawnPlayerPacket) packet;

            if (!MainEntities.ContainsKey(spawnPlayer.EntityID))
                MainEntities.Add(spawnPlayer.EntityID, new Entity {EntityID = spawnPlayer.EntityID});

            MainEntities[spawnPlayer.EntityID].Player.UUID = spawnPlayer.PlayerUUID;
            MainEntities[spawnPlayer.EntityID].Position = spawnPlayer.Vector3;

            MainEntities[spawnPlayer.EntityID].Look.Yaw = spawnPlayer.Yaw;
            MainEntities[spawnPlayer.EntityID].Look.Pitch = spawnPlayer.Pitch;

            MainEntities[spawnPlayer.EntityID].Equipment.CurrentItem = spawnPlayer.CurrentItem;

            MainEntities[spawnPlayer.EntityID].Metadata = spawnPlayer.EntityMetadata;
        }

        private void OnCollectItem(IPacket packet)
        {
            var collectItem = (CollectItemPacket) packet;

            if (!MainEntities.ContainsKey(collectItem.CollectorEntityID))
                MainEntities.Add(collectItem.CollectorEntityID, new Entity {EntityID = collectItem.CollectorEntityID});

            //Entities[collectItem.CollectorEntityID] = collectItem.CollectedEntityID; TODO: CollectedID
        }

        private void OnSpawnObject(IPacket packet)
        {
            var spawnObject = (SpawnObjectPacket) packet;
        }

        private void OnSpawnMob(IPacket packet)
        {
            var spawnMob = (SpawnMobPacket) packet;

            if (!MainEntities.ContainsKey(spawnMob.EntityID))
                MainEntities.Add(spawnMob.EntityID, new Entity {EntityID = spawnMob.EntityID});

            MainEntities[spawnMob.EntityID].Type = spawnMob.Type;

            MainEntities[spawnMob.EntityID].Position = spawnMob.Vector3;

            MainEntities[spawnMob.EntityID].Look.Yaw = spawnMob.Yaw;
            MainEntities[spawnMob.EntityID].Look.Pitch = spawnMob.Pitch;
            MainEntities[spawnMob.EntityID].Look.HeadPitch = spawnMob.HeadPitch;

            MainEntities[spawnMob.EntityID].Velocity.VelocityX = spawnMob.VelocityX;
            MainEntities[spawnMob.EntityID].Velocity.VelocityY = spawnMob.VelocityY;
            MainEntities[spawnMob.EntityID].Velocity.VelocityZ = spawnMob.VelocityZ;

            MainEntities[spawnMob.EntityID].Metadata = spawnMob.EntityMetadata;
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

            if (!MainEntities.ContainsKey(entityVelocity.EntityID))
                MainEntities.Add(entityVelocity.EntityID, new Entity {EntityID = entityVelocity.EntityID});

            MainEntities[entityVelocity.EntityID].Velocity.VelocityX = entityVelocity.VelocityX;
            MainEntities[entityVelocity.EntityID].Velocity.VelocityY = entityVelocity.VelocityY;
            MainEntities[entityVelocity.EntityID].Velocity.VelocityZ = entityVelocity.VelocityZ;
        }

        private void OnDestroyEntities(IPacket packet) // -- Works
        {
            var destroyEntities = (DestroyEntitiesPacket) packet;

            foreach (int t in destroyEntities.EntityIDs)
            {
                MainEntities.Remove(t);
            }
        }

        private void OnEntity(IPacket packet)
        {
            var entity = (EntityPacket) packet;

            if (!MainEntities.ContainsKey(entity.EntityID))
                MainEntities.Add(entity.EntityID, new Entity {EntityID = entity.EntityID});
        }

        private void OnEntityRelativeMove(IPacket packet)
        {
            var entityRelativeMove = (EntityRelativeMovePacket) packet;

            if (!MainEntities.ContainsKey(entityRelativeMove.EntityID))
                MainEntities.Add(entityRelativeMove.EntityID, new Entity {EntityID = entityRelativeMove.EntityID});

            MainEntities[entityRelativeMove.EntityID].Position = entityRelativeMove.DeltaVector3; //Nope

            MainEntities[entityRelativeMove.EntityID].OnGround = entityRelativeMove.OnGround;
        }

        private void OnEntityLook(IPacket packet)
        {
            var entityLook = (EntityLookPacket) packet;

            if (!MainEntities.ContainsKey(entityLook.EntityID))
                MainEntities.Add(entityLook.EntityID, new Entity {EntityID = entityLook.EntityID});

            MainEntities[entityLook.EntityID].Look.Yaw = entityLook.Yaw;
            MainEntities[entityLook.EntityID].Look.Pitch = entityLook.Pitch;

            MainEntities[entityLook.EntityID].OnGround = entityLook.OnGround;
        }

        private void OnEntityLookAndRelativeMove(IPacket packet)
        {
            var entityLookAndRelativeMove = (EntityLookAndRelativeMovePacket) packet;

            if (!MainEntities.ContainsKey(entityLookAndRelativeMove.EntityID))
                MainEntities.Add(entityLookAndRelativeMove.EntityID,
                    new Entity {EntityID = entityLookAndRelativeMove.EntityID});

            MainEntities[entityLookAndRelativeMove.EntityID].Position = entityLookAndRelativeMove.DeltaVector3; //Nope

            MainEntities[entityLookAndRelativeMove.EntityID].Look.Yaw = entityLookAndRelativeMove.Yaw;
            MainEntities[entityLookAndRelativeMove.EntityID].Look.Pitch = entityLookAndRelativeMove.Pitch;

            MainEntities[entityLookAndRelativeMove.EntityID].OnGround = entityLookAndRelativeMove.OnGround;
        }

        private void OnEntityTeleport(IPacket packet)
        {
            var entityTeleport = (EntityTeleportPacket) packet;

            if (!MainEntities.ContainsKey(entityTeleport.EntityID))
                MainEntities.Add(entityTeleport.EntityID, new Entity {EntityID = entityTeleport.EntityID});

            MainEntities[entityTeleport.EntityID].Position = entityTeleport.Vector3;

            MainEntities[entityTeleport.EntityID].Look.Yaw = entityTeleport.Yaw;
            MainEntities[entityTeleport.EntityID].Look.Pitch = entityTeleport.Pitch;

            MainEntities[entityTeleport.EntityID].OnGround = entityTeleport.OnGround;
        }

        private void OnEntityHeadLook(IPacket packet)
        {
            var entityHeadLook = (EntityHeadLookPacket) packet;

            if (!MainEntities.ContainsKey(entityHeadLook.EntityID))
                MainEntities.Add(entityHeadLook.EntityID, new Entity {EntityID = entityHeadLook.EntityID});

            MainEntities[entityHeadLook.EntityID].Look.HeadYaw = entityHeadLook.HeadYaw;
        }

        private void OnEntityStatus(IPacket packet)
        {
            var entityStatus = (EntityStatusPacket) packet;

            if (!MainEntities.ContainsKey(entityStatus.EntityID))
                MainEntities.Add(entityStatus.EntityID, new Entity {EntityID = entityStatus.EntityID});

            MainEntities[entityStatus.EntityID].Status = entityStatus.Status;
        }

        private void OnAttachEntity(IPacket packet)
        {
            var attachEntity = (AttachEntityPacket) packet;

            if (!MainEntities.ContainsKey(attachEntity.EntityID))
                MainEntities.Add(attachEntity.EntityID, new Entity {EntityID = attachEntity.EntityID});

            MainEntities[attachEntity.EntityID].Vehicle.VehicleID = attachEntity.VehicleID;

            MainEntities[attachEntity.EntityID].Leash = attachEntity.Leash;
        }

        private void OnEntityMetadata(IPacket packet)
        {
            var entityMetadata = (EntityMetadataPacket) packet;

            if (!MainEntities.ContainsKey(entityMetadata.EntityID))
                MainEntities.Add(entityMetadata.EntityID, new Entity {EntityID = entityMetadata.EntityID});

            MainEntities[entityMetadata.EntityID].Metadata = entityMetadata.Metadata;
        }

        private void OnEntityEffect(IPacket packet)
        {
            var entityEffect = (EntityEffectPacket) packet;

            if (MainPlayer.EntityID == entityEffect.EntityID)
            {
                MainPlayer.Effects.Add(new PlayerEffect
                {
                    EffectID = entityEffect.EffectID,
                    Amplifier = entityEffect.Amplifier,
                    Duration = entityEffect.Duration,
                    HideParticle = entityEffect.HideParticles
                });
            }
            else
            {
                if (MainEntities.ContainsKey(entityEffect.EntityID))
                    return;

                MainEntities.Add(entityEffect.EntityID, new Entity {EntityID = entityEffect.EntityID});
                MainEntities[entityEffect.EntityID].Effects.Add(new EntityEffect
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

            if (MainPlayer.EntityID == removeEntityEffect.EntityID)
            {
                foreach (var effect in MainPlayer.Effects.ToArray())
                {
                    if (effect.EffectID == removeEntityEffect.EffectID)
                        MainPlayer.Effects.Remove(effect);
                }
            }
            else
            {
                if (!MainEntities.ContainsKey(removeEntityEffect.EntityID))
                    MainEntities.Add(removeEntityEffect.EntityID, new Entity {EntityID = removeEntityEffect.EntityID});

                foreach (var effect in MainEntities[removeEntityEffect.EntityID].Effects.ToArray())
                {
                    if (effect.EffectID == removeEntityEffect.EffectID)
                        MainEntities[removeEntityEffect.EntityID].Effects.Remove(effect);
                }
            }
        }

        private void OnSetExperience(IPacket packet)
        {
            var setExperience = (SetExperiencePacket) packet;

            MainPlayer.Experience.ExperienceBar = setExperience.ExperienceBar;
            MainPlayer.Experience.Level = setExperience.Level;
            MainPlayer.Experience.TotalExperience = setExperience.TotalExperience;
        }

        private void OnEntityProperties(IPacket packet)
        {
            var entityProperties = (EntityPropertiesPacket) packet;

            if (!MainEntities.ContainsKey(entityProperties.EntityID))
                MainEntities.Add(entityProperties.EntityID, new Entity {EntityID = entityProperties.EntityID});

            MainEntities[entityProperties.EntityID].Properties = entityProperties.EntityProperties;
        }

        private void OnChunkData(IPacket packet) // -- Works
        {
            var chunkData = (ChunkDataPacket) packet;

            if (MainWorld == null)
                MainWorld = new World();

            if (chunkData.Chunk.PrimaryBitMap == 0)
            {
                MainWorld.RemoveChunk(chunkData.Chunk.Coordinates);
                return;
            }

            // -- Add the chunk to the world
            MainWorld.SetChunk(chunkData.Chunk);     
        }

        private void OnMultiBlockChange(IPacket packet) // -- Works
        {
            var multiBlockChange = (MultiBlockChangePacket) packet;

            foreach (var record in multiBlockChange.RecordList.GetRecords())
            {
                var id = (short) (record.BlockIDMeta >> 4);
                var meta = (byte)(record.BlockIDMeta & 15);
                var block = new Block(id, meta);

                MainWorld.SetBlock(record.Coordinates, multiBlockChange.Coordinates, block);
            }      
        }

        private void OnBlockChange(IPacket packet) // -- Works
        {
            var blockChange = (BlockChangePacket) packet;

            var id = (short) (blockChange.BlockIDMeta >> 4);
            var meta = (byte)(blockChange.BlockIDMeta & 15);

            var block = new Block(id, meta);
            
            MainWorld.SetBlock(blockChange.Location, block);
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

            if (MainWorld == null)
                MainWorld = new World();

            foreach (var chunk in mapChunkBulk.ChunkList.GetChunk())
                MainWorld.SetChunk(chunk);
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

            MainWorld.StateChanged.Reason = changeGameState.Reason;
            MainWorld.StateChanged.Value = changeGameState.Value;
        }

        private void OnSpawnGlobalEntity(IPacket packet)
        {
            var spawnGlobalEntity = (SpawnGlobalEntityPacket)packet;
        }

        private void OnOpenWindow(IPacket packet)
        {
            var openWindow = (OpenWindowPacket) packet;

            MainPlayer.OpenWindow(openWindow.WindowID, openWindow.InventoryType, openWindow.WindowTitle, openWindow.NumberOfSlots, openWindow.EntityID);
        }

        private void OnCloseWindow(IPacket packet)
        {
            var closeWindow = (CloseWindowPacket) packet;

            MainPlayer.CloseWindow(closeWindow.WindowID);
        }

        private void OnSetSlot(IPacket packet)
        {
            var setSlot = (SetSlotPacket) packet;

            //Player.SetSlot(setSlot.WindowId, setSlot.Slot, setSlot.SlotData);
        }

        private void OnWindowItems(IPacket packet)
        {
            var windowItems = (WindowItemsPacket) packet;

            MainPlayer.SetWindowItems(windowItems.WindowID, windowItems.ItemStackList);
        }

        private void OnWindowProperty(IPacket packet)
        {
            var windowProperty = (WindowPropertyPacket) packet;
        }

        private void OnConfirmTransaction(IPacket packet)
        {
            var confirmTransaction = (ConfirmTransactionPacket) packet;

            MainPlayer.ConfirmTransaction(confirmTransaction.WindowId, confirmTransaction.ActionNumber, confirmTransaction.Accepted);
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

            MainPlayer.Statistics = statistics.StatisticsEntryList;
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

            MainPlayer.Abilities.Flags = playerAbilities.Flags;
            MainPlayer.Abilities.FlyingSpeed = playerAbilities.FlyingSpeed;
            MainPlayer.Abilities.WalkingSpeed = playerAbilities.WalkingSpeed;
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