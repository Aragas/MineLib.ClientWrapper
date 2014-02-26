using System;

namespace MineLib.ClientWrapper.Data.Anvil
{
    public class Section
    {
        public byte[] BlocksType;
        public byte[] BlocksMetadata;
        public byte[] BlocksLight;
        public byte[] SkyLight;
        public byte Y;

        public Section(byte y)
        {
            Y = y;
            BlocksType = new byte[4096];
            BlocksMetadata = new byte[4096];
            BlocksLight = new byte[4096];
            SkyLight = new byte[4096];
        }

        public void SetBlock(int x, int y, int z, int id)
        {
            int index = x + (z * 16) + (y * 16 * 16);
            BlocksType[index] = (byte)id;
        }

        public Block GetBlock(int x, int y, int z)
        {
            int index = x + (z*16) + (y*16*16);
            var thisBlock = new Block((int) BlocksType[index], x, y, z, (int) Math.Floor(decimal.Divide(x, 16)),
                (int) Math.Floor(decimal.Divide(z, 16)));

            return thisBlock;
        }

        public int GetBlockMetadata(int x, int y, int z)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            byte value = BlocksMetadata[index];

            return value;
        }

        public void SetBlockMetadata(int x, int y, int z, byte data)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            BlocksMetadata[index] = data;
        }

        public byte GetBlockLighting(int x, int y, int z)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            return BlocksLight[index];
        }

        public void SetBlockLighting(int x, int y, int z, byte data)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            BlocksLight[index] = data;
        }

        public byte GetBlockSkylight(int x, int y, int z)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            return SkyLight[index];
        }

        public void SetBlockSkylight(int x, int y, int z, byte data)
        {
            int index = (x + (z * 16) + (y * 16 * 16));
            SkyLight[index] = data;
        }
    }
}
