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
        public Vector3 SpawnPosition;

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

    }

    public struct GameStateChanged
    {
        public GameStateReason Reason;
        public float Value;
    }

}
