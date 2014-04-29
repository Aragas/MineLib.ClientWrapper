using System;
using MineLib.Network.Data;

namespace MineLib.ClientWrapper.Data.Anvil
{
    public class Section
    {
        public const byte Width = 16, Height = 16, Depth = 16;

        public byte[] RawBlocks;
        public byte[] RawMetadata;
        public byte[] RawBlockLight;
        public byte[] RawSkylight;
        public byte Y;

        public Block[] Blocks;

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

        public Block GetBlock(Coordinates3D coordinates)
        {
            var index = coordinates.X + (coordinates.Z * 16) + (coordinates.Y * 16 * 16);

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
            var index = coordinates.X + (coordinates.Z * 16) + (coordinates.Y * 16 * 16);

            Blocks[index] = block;
        }

        public byte GetBlockLighting(int x, int y, int z)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            return RawBlockLight[index];
        }

        public void SetBlockLighting(int x, int y, int z, byte data)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            RawBlockLight[index] = data;
        }

        public byte GetBlockSkylight(int x, int y, int z)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            return RawSkylight[index];
        }

        public void SetBlockSkylight(int x, int y, int z, byte data)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            RawSkylight[index] = data;
        }
    }
}