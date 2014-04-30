using System;
using MineLib.Network.Data;

namespace MineLib.ClientWrapper.Data.Anvil
{
    // This class don't use RawData in any way. Only Blocks. I mean hash and other stuff.
    public class Section
    {
        public const byte Width = 16, Height = 16, Depth = 16;

        public byte[] RawBlocks;
        public byte[] RawMetadata;
        public byte[] RawBlockLight;
        public byte[] RawSkylight;
        public byte Y;

        public Block[] Blocks;

        public bool IsFilled;
        public void FillBlocks()
        {
            for (var i = 0; i < Blocks.Length; i++)
            {
                var id = RawBlocks[i];
                var meta = RawMetadata[i];

                if (id == 0)
                {
                    Blocks[i] = new Block(0, 0, "Air");
                    continue;
                }

                Blocks[i] = Block.GetBlock(id, meta);
            }
            IsFilled = true;
        }

        public Section(byte y)
        {
            Y = y;

            Blocks = new Block[4096];

            RawBlocks = new byte[4096];
            RawMetadata = new byte[4096];
            RawBlockLight = new byte[4096];
            RawSkylight = new byte[4096];
        }

        public override string ToString()
        {
            return IsFilled ? "Filled" : "Empty";
        }

        public Block GetBlock(Coordinates3D coordinates)
        {
            var index = GetIndex(coordinates);

            var block = Blocks[index];
            Blocks[index].Coordinates = coordinates;
            Blocks[index].Chunk = new Coordinates2D
            {
                X = (int) Math.Floor(decimal.Divide(coordinates.X, 16)),
                Z = (int) Math.Floor(decimal.Divide(coordinates.Z, 16))
            };

            return block;
        }

        public void SetBlock(Coordinates3D coordinates, Block block)
        {
            var index = GetIndex(coordinates);

            Blocks[index] = block;
        }

        public byte GetBlockLighting(Coordinates3D coordinates)
        {
            var index = GetIndex(coordinates);
            return RawBlockLight[index];
        }

        public void SetBlockLighting(Coordinates3D coordinates, byte data)
        {
            var index = GetIndex(coordinates);
            RawBlockLight[index] = data;
        }

        public byte GetBlockSkylight(Coordinates3D coordinates)
        {
            var index = GetIndex(coordinates);
            return RawSkylight[index];
        }

        public void SetBlockSkylight(Coordinates3D coordinates, byte data)
        {
            var index = GetIndex(coordinates);
            RawSkylight[index] = data;
        }

        private static int GetIndex(Coordinates3D coordinates)
        {
            return (coordinates.X + (coordinates.Z * 16) + (coordinates.Y * 16 * 16));
        }

        public bool Equals(Block block)
        {
            return Blocks.Equals(block);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (obj.GetType() != typeof(Block)) return false;
            return Equals((Block)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Blocks.GetHashCode();
            }
        }
    }
}