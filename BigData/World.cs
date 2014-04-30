using System;
using System.Collections.Generic;
using MineLib.ClientWrapper.Data.Anvil;
using MineLib.Network.Data;
using MineLib.Network.Enums;

namespace MineLib.ClientWrapper.BigData
{
    // Just ignore it. I think something is wrong here :DD
    internal static class TimeSpanUtil
    {
        public static double ConvertSecondsToDays(double seconds)
        {
            return TimeSpan.FromSeconds(seconds).TotalDays;
        }

        public static double ConvertSecondsToHours(double seconds)
        {
            return TimeSpan.FromSeconds(seconds).TotalHours;
        }

        public static double ConvertSecondsToMinutes(double seconds)
        {
            return TimeSpan.FromSeconds(seconds).TotalMinutes;
        }

    }

    public class World
    {
        // -- Debugging
        public static List<string> UnsupportedBlockList = new List<string>();
        // -- Debugging

        public GameMode GameMode;
        public Dimension Dimension;
        public Difficulty Difficulty;
        public GameStateChanged StateChanged;
        public Coordinates3D SpawnPosition;

        public byte MaxPlayers;

        public string LevelType;

        public long AgeOfTheWorld;
        public long TimeOfDay;

        public List<Chunk> Chunks;

        public TimeSpan AgeOfTheWorldTimeSpan
        {
            get
            {
                return new TimeSpan(
                    (int) TimeSpanUtil.ConvertSecondsToDays(AgeOfTheWorld/20),
                    (int) TimeSpanUtil.ConvertSecondsToHours(AgeOfTheWorld/20),
                    (int) TimeSpanUtil.ConvertSecondsToMinutes(AgeOfTheWorld/20));
            }
        }

        public World()
        {
            Chunks = new List<Chunk>();
        }

        #region Anvil

        public int GetChunkIndex(Coordinates2D coordinates)
        {
            foreach (var chunk in Chunks)
            {
                if (chunk.Coordinates == coordinates)
                    return Chunks.IndexOf(chunk);
            }

            return -1;
        }

        public Chunk GetChunkByBlockCoordinates(Coordinates3D coordinates)
        {
            if (coordinates.Y < 0 || coordinates.Y >= Chunk.Height)
                throw new ArgumentOutOfRangeException("coordinates", "Coordinates are out of range");

            var chunkX = (int)Math.Floor((double)coordinates.X / Chunk.Width);
            var chunkZ = (int)Math.Floor((double)coordinates.Z / Chunk.Depth);

            return GetChunk(new Coordinates2D(chunkX, chunkZ));
        }

        public Chunk GetChunk(Coordinates2D coordinates)
        {
            foreach (var chunk in Chunks)
            {
                if (chunk.Coordinates == coordinates)
                    return chunk;
            }
            return null;
        }

        public void SetChunk(Chunk chunk)
        {
            Chunks.Add(chunk);
        }

        public Block GetBlock(Coordinates3D coordinates)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);
            return chunk.GetBlock(coordinates);
        }

        public void SetBlock(Coordinates3D coordinates, Block block)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);
            chunk.SetBlock(coordinates, block);

            //OnBlockChange(coordinates);
        }

        #endregion
    }

    public struct GameStateChanged
    {
        public GameStateReason Reason;
        public float Value;
    }

}
