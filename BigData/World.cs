using System;
using System.Collections.Generic;
using MineLib.Network;
using MineLib.Network.Main.Data;
using MineLib.Network.Main.Data.Anvil;
using MineLib.Network.Main.Enums;

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
        public static List<IPacket> DamagedChunks = new List<IPacket>();
        // -- Debugging

        public GameMode GameMode;
        public Dimension Dimension;
        public Difficulty Difficulty;
        public GameStateChanged StateChanged;
        public Position SpawnPosition;

        public byte MaxPlayers;

        public string LevelType;

        public long AgeOfTheWorld;
        public long TimeOfDay;

        private const float RealTimeDivisor = 24 * 60 * 60;
        private const float GameHourInRealMinutes = (float)2 / 60;
        private const float GameHourInRealSeconds = GameHourInRealMinutes * 60;
        public float GetGameTimeOfDay()
        {
            return ((TimeOfDay / GameHourInRealSeconds) % 24); // quick demonstration of day & night cycles.
            //return 12; // this disables the day & night cycle.
        }

        public List<Chunk> Chunks;

        public TimeSpan AgeOfTheWorldTimeSpan
        {
            get
            {
                return new TimeSpan(
                    (int) TimeSpanUtil.ConvertSecondsToDays(AgeOfTheWorld / 20),
                    (int) TimeSpanUtil.ConvertSecondsToHours(AgeOfTheWorld / 20),
                    (int) TimeSpanUtil.ConvertSecondsToMinutes(AgeOfTheWorld / 20));
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

        public Chunk GetChunkByBlockCoordinates(Position coordinates)
        {
            if (coordinates.Y < 0 || coordinates.Y >= Chunk.Height)
                throw new ArgumentOutOfRangeException("coordinates", "Coordinates.Y is out of range");

            var chunkX = coordinates.X >> 4;
            var chunkZ = coordinates.Z >> 4;

            return GetChunk(new Coordinates2D(chunkX, chunkZ));
        }

        public static Coordinates2D ChunkCoordinatesToWorld(Coordinates2D coordinates)
        {
            var chunkX = coordinates.X * Chunk.Width;
            var chunkZ = coordinates.Z * Chunk.Depth;

            return new Coordinates2D(chunkX, chunkZ);
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

        public void RemoveChunk(Coordinates2D coordinates)
        {
            for (int i = 0; i < Chunks.Count; i++)
            {
                var chunk = Chunks[i];
                if (chunk.Coordinates == coordinates)
                    Chunks[i] = null;
            }
        }


        public Block GetBlock(Position coordinates)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            return chunk.GetBlock(coordinates);
        }

        public void SetBlock(Position coordinates, Block block)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            chunk.SetBlock(coordinates, block);
        }

        public void SetBlock(Position coordinates, Coordinates2D chunkCoordinates, Block block)
        {
            var chunk = GetChunk(chunkCoordinates);

            chunk.SetBlockMultiBlock(coordinates, block);
        }

        public byte GetBlockLight(Position coordinates)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            return chunk.GetBlockLight(coordinates);
        }
        
        public void SetBlockLight(Position coordinates, byte light)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            chunk.SetBlockLight(coordinates, light);
        }

        public byte GetBlockSkylight(Position coordinates)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            return chunk.GetBlockSkylight(coordinates);
        }

        public void SetBlockSkylight(Position coordinates, byte light)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            chunk.SetBlockSkylight(coordinates, light);
        }

        public byte GetBlockBiome(Position coordinates)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            return chunk.GetBlockBiome(coordinates);
        }

        public void SetBlockBiome(Position coordinates, byte biome)
        {
            var chunk = GetChunkByBlockCoordinates(coordinates);

            chunk.SetBlockBiome(coordinates, biome);
        }

        #endregion
    }

    public struct GameStateChanged
    {
        public GameStateReason Reason;
        public float Value;
    }

}
