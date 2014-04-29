using System;
using System.Collections.Generic;
using MineLib.ClientWrapper.Data.Anvil;
using MineLib.Network.Data;
using MineLib.Network.Enums;

namespace MineLib.ClientWrapper.BigData
{
    public class World
    {
        public GameMode GameMode;
        public Dimension Dimension;
        public Difficulty Difficulty;
        public GameStateChanged StateChanged;
        public Coordinates3D SpawnPosition;

        public byte MaxPlayers;

        public string LevelType;

        public long AgeOfTheWorld;
        public long TimeOfDay;


        public List<Entity> Entities;
        public List<Object> Objects;
        public List<Chunk> Chunks;

        public World()
        {
            Objects = new List<Object>();
            Chunks = new List<Chunk>();
            Entities = new List<Entity>();
        }

        /// <summary>
        /// Returns the index where the chunk resides in worldChunks.
        /// </summary>
        /// <param name="coordinates">X location of chunk</param>
        /// <returns>Index of Chunk in worldChunks</returns>
        public int GetChunkIndex(Coordinates2D coordinates)
        {
            foreach (var c in Chunks)
            {
                if (c.Coordinates == coordinates)
                    return Chunks.IndexOf(c);
            }

            return -1;
        }

        public Chunk GetChunkBlockPosition(Coordinates3D coordinates)
        {
            if (coordinates.Y < 0 || coordinates.Y >= Chunk.Height)
                throw new ArgumentOutOfRangeException("coordinates", "Coordinates are out of range");

            var chunkX = (int)Math.Floor((double)coordinates.X / Chunk.Width);
            var chunkZ = (int)Math.Floor((double)coordinates.Z / Chunk.Depth);

            return GetChunk(new Coordinates2D(chunkX, chunkZ));
        }

        public Chunk GetChunk(Coordinates2D coordinates)
        {
            foreach (Chunk c in Chunks)
            {
                if (c.Coordinates == coordinates)
                    return c;
            }
            return null;
        }

        public void SetChunk(Chunk chunk)
        {
            Chunks.Add(chunk);
        }

        public Block GetBlock(Coordinates3D coordinates)
        {
            var chunk = GetChunkBlockPosition(coordinates);
            return chunk.GetBlock(coordinates);
        }

        public void SetBlock(Coordinates3D coordinates, Block block)
        {
            var chunk = GetChunkBlockPosition(coordinates);
            chunk.SetBlock(coordinates, block);

            //OnBlockChange(coordinates);
        }
    }


    public struct GameStateChanged
    {
        public GameStateReason Reason;
        public float Value;
    }

}
