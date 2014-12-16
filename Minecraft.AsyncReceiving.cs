using System;
using System.Collections.Generic;
using System.Linq;
using MineLib.Network;
using MineLib.Network.Data.Anvil;

namespace MineLib.ClientWrapper
{
    public partial class Minecraft
    {
        private Dictionary<Type, Action<IAsyncReceive>> AsyncReceiveHandlers { get; set; }

        public void RegisterReceiveEvent(Type asyncReceiveType, Action<IAsyncReceive> method)
        {
            var any = asyncReceiveType.GetInterfaces().Any(p => p == typeof(IAsyncReceive));
            if (!any)
                throw new InvalidOperationException("AsyncReceive type must implement MineLib.Network.IAsyncReceive");

            AsyncReceiveHandlers[asyncReceiveType] = method;
        }


        private void RegisterSupportedReceiveEvents()
        {
            RegisterReceiveEvent(typeof(OnChatMessage), OnChatMessage);

            RegisterReceiveEvent(typeof(OnPlayerPosition), OnPlayerPosition);
            RegisterReceiveEvent(typeof(OnPlayerLook), OnPlayerLook);
            RegisterReceiveEvent(typeof(OnHeldItemChange), OnHeldItemChange);
            RegisterReceiveEvent(typeof(OnSpawnPoint), OnSpawnPoint);
            RegisterReceiveEvent(typeof(OnUpdateHealth), OnUpdateHealth);
            RegisterReceiveEvent(typeof(OnRespawn), OnRespawn);
            RegisterReceiveEvent(typeof(OnAction), OnAction);
            RegisterReceiveEvent(typeof(OnSetExperience), OnSetExperience);

            RegisterReceiveEvent(typeof(OnChunk), OnChunk);
            RegisterReceiveEvent(typeof(OnChunkList), OnChunkList);
            RegisterReceiveEvent(typeof(OnBlockChange), OnBlockChange);
            RegisterReceiveEvent(typeof(OnMultiBlockChange), OnMultiBlockChange);
            RegisterReceiveEvent(typeof(OnBlockAction), OnBlockAction);

            RegisterReceiveEvent(typeof(OnBlockBreakAction), OnBlockBreakAction);

        }

        public void DoReceiveEvent(Type asyncReceiveType, IAsyncReceive data)
        {
            var any = asyncReceiveType.GetInterfaces().Any(p => p == typeof(IAsyncReceive));
            if (!any)
                throw new InvalidOperationException("AsyncReceive type must implement MineLib.Network.IAsyncReceive");

            if (!AsyncReceiveHandlers.ContainsKey(asyncReceiveType))
                return;

            AsyncReceiveHandlers[asyncReceiveType](data);
        }


        private void OnChatMessage(IAsyncReceive receiveEvent)
        {
            var data = (OnChatMessage) receiveEvent;

            ChatHistory.Add(data.Message);
        }

        #region Anvil

        private void OnChunk(IAsyncReceive receiveEvent)
        {
            var data = (OnChunk) receiveEvent;

            if (data.Chunk.PrimaryBitMap == 0)
            {
                World.RemoveChunk(data.Chunk.Coordinates);
                return;
            }

            // -- Add the chunk to the world
            World.SetChunk(data.Chunk);
        }

        private void OnChunkList(IAsyncReceive receiveEvent)
        {
            var data = (OnChunkList) receiveEvent;

            foreach (var chunk in data.Chunks.GetChunk())
                World.SetChunk(chunk);
        }

        private void OnBlockChange(IAsyncReceive receiveEvent)
        {
            var data = (OnBlockChange) receiveEvent;

            var id = (ushort) (data.Block >> 4);
            var meta = (byte) (data.Block & 0xF);

            var block = new Block(id, meta);

            World.SetBlock(data.Location, block);
        }

        private void OnMultiBlockChange(IAsyncReceive receiveEvent)
        {
            var data = (OnMultiBlockChange) receiveEvent;

            foreach (var record in data.Records)
            {
                var id = (ushort) (record.BlockIDMeta >> 4);
                var meta = (byte) (record.BlockIDMeta & 0xF);

                World.SetBlock(record.Coordinates, data.ChunkLocation, new Block(id, meta));
            }      
        }

        private void OnBlockAction(IAsyncReceive receiveEvent)
        {
            var data = (OnBlockAction) receiveEvent;
        }

        private void OnBlockBreakAction(IAsyncReceive receiveEvent)
        {
            var data = (OnBlockBreakAction) receiveEvent;
        }

        #endregion

        private void OnPlayerPosition(IAsyncReceive receiveEvent)
        {
            var data = (OnPlayerPosition) receiveEvent;
        }

        private void OnPlayerLook(IAsyncReceive receiveEvent)
        {
            var data = (OnPlayerLook) receiveEvent;
        }

        private void OnHeldItemChange(IAsyncReceive receiveEvent)
        {
            var data = (OnHeldItemChange) receiveEvent;
        }

        private void OnSpawnPoint(IAsyncReceive receiveEvent)
        {
            var data = (OnSpawnPoint) receiveEvent;
        }

        private void OnUpdateHealth(IAsyncReceive receiveEvent)
        {
            var data = (OnUpdateHealth) receiveEvent;  
        }

        private void OnRespawn(IAsyncReceive receiveEvent)
        {
            var data = (OnRespawn) receiveEvent;
        }

        private void OnAction(IAsyncReceive receiveEvent)
        {
            var data = (OnAction) receiveEvent;
        }

        private void OnSetExperience(IAsyncReceive receiveEvent)
        {
            var data = (OnSetExperience) receiveEvent;
        }
    }
}
