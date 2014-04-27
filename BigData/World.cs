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
        public List<NewChunk> Chunks;

        public World()
        {
            Objects = new List<Object>();
            Chunks = new List<NewChunk>();
            Entities = new List<Entity>();
        }

        /// <summary>
        /// Returns the index where the chunk resides in worldChunks.
        /// </summary>
        /// <param name="x">X location of chunk</param>
        /// <param name="z">Z location of chunk</param>
        /// <returns>Index of Chunk in worldChunks</returns>
        public int GetChunkIndex(Coordinates2D coordinates)
        {
            int chunkIndex = -1;

            try
            {
                foreach (var c in Chunks)
                {
                    if (c.Coordinates == coordinates)
                    {
                        chunkIndex = Chunks.IndexOf(c);
                    }
                }
            }
            catch { return chunkIndex; }

            return chunkIndex;
        }

        public NewChunk GetChunk(Coordinates2D coordinates)
        {
            try
            {
                foreach (NewChunk c in Chunks)
                {
                    if (c.Coordinates == coordinates)
                        return c;
                }
            }
            catch { }

            return null;
        }

    }

    public struct GameStateChanged
    {
        public GameStateReason Reason;
        public float Value;
    }

}
