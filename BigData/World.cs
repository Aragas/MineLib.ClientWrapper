using System;
using System.Collections.Generic;
using MineLib.ClientWrapper.Data.Anvil;
using MineLib.Network.Enums;

namespace MineLib.ClientWrapper.BigData
{
    public class World
    {
        public GameMode GameMode;
        public Dimension Dimension;
        public Difficulty Difficulty;
        public GameStateChanged StateChanged;
        public WorldSpawnPosition SpawnPosition;

        public byte MaxPlayers;

        public string LevelType;

        public long AgeOfTheWorld;
        public long TimeOfDay;


        //public List<Entity> Entities;
        //public List<Object> worldObjects;
        public List<Chunk> WorldChunks;

        public World()
        {
            //worldObjects = new List<Object>();
            WorldChunks = new List<Chunk>();
            //Entities = new List<Entity>();
        }


        /// <summary>
        /// Returns the index of which the entity resides in for the Entities list
        /// </summary>
        /// <param name="Entity_ID"></param>
        /// <returns></returns>
        //public int GetEntityById(int Entity_ID)
        //{
        //    int thisEntity = -1;
        //
        //    try
        //    {
        //        foreach (Entity b in Entities)
        //        {
        //            if (b.EntityID == Entity_ID)
        //            {
        //                thisEntity = Entities.IndexOf(b);
        //                break;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        return thisEntity;
        //    }
        //
        //    return thisEntity;
        //}

        /// <summary>
        /// Returns the index where the chunk resides in worldChunks.
        /// </summary>
        /// <param name="x">X location of chunk</param>
        /// <param name="z">Z location of chunk</param>
        /// <returns>Index of Chunk in worldChunks</returns>
        public int GetChunkIndex(int x, int z)
        {
            int chunkIndex = -1;

            try
            {
                foreach (Chunk c in WorldChunks)
                {
                    if (c.X == x && c.Z == z)
                    {
                        chunkIndex = WorldChunks.IndexOf(c);
                    }
                }
            }
            catch { return chunkIndex; }

            return chunkIndex;
        }

        public Chunk GetChunk(int x, int z)
        {
            try
            {
                foreach (Chunk c in WorldChunks)
                {
                    if (c.X == x && c.Z == z)
                        return c; 
                }
            }
            catch(Exception e) {}

            return null;
        }
    }

    public struct GameStateChanged
    {
        public GameStateReason Reason;
        public float Value;
    }

    public struct WorldSpawnPosition
    {
        public int X, Y, Z;
    }
}
