using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MineLib.Network.Data;

namespace MineLib.ClientWrapper.Data.Anvil
{
    //*
    public struct ChunkData
    {
        public byte[] Blocks;
        public byte[] Metadata;
        public byte[] BlockLight;
        public byte[] SkyLight;
        public byte[] AddArray;
        public byte[] Biome;

    }

    public class NewChunk
    {
        public const int Width = 16, Height = 256, Depth = 16;

        private const int BlockDataLength = Section.Width * Section.Depth * Section.Height;
        private const int NibbleDataLength = BlockDataLength / 2;

        public Coordinates2D Coordinates { get; set; }
        public ushort PrimaryBitMap { get; set; }
        public ushort AddBitMap { get; set; }
        public bool SkyLightSent { get; set; }
        public bool GroundUp { get; set; }

        public byte[] Biomes { get; set; }

        public Section[] Sections { get; set; }

        private ChunkData Data;

        public NewChunk()
        {
            Biomes = new byte[Width * Depth];

            Sections = new Section[16];
            for (int i = 0; i < Sections.Length; i++)
                Sections[i] = new Section((byte)i);
        }

        private static int GetSectionCount(ushort bitMap)
        {
            // Get the total sections included in the bitMap
            var sectionCount = 0;
            for (int y = 0; y < 16; y++)
            {
                if ((bitMap & (1 << y)) > 0)
                    sectionCount++;
            }

            return sectionCount;
        }

        #region MapChunkBulk

        private void HandleChunkData2(byte[] data)
        {
            Data = new ChunkData();

            var sectionCount = GetSectionCount(PrimaryBitMap);
            var total = BlockDataLength * sectionCount;
            var nibbletotal = NibbleDataLength * sectionCount;

            byte[] temp;
            int offset = 0;

            Data.Blocks = new byte[total];
            Data.Metadata = new byte[nibbletotal]; // -- Contains block Metadata.
            Data.BlockLight = new byte[nibbletotal];
            Data.SkyLight = new byte[nibbletotal];

            Data.AddArray = new byte[nibbletotal];
            Data.Biome = new byte[256];

            Buffer.BlockCopy(data, offset, Data.Blocks, 0, total);
            offset += total;

            Buffer.BlockCopy(data, offset, Data.Metadata, 0, nibbletotal); // -- Copy in Metadata
            offset += nibbletotal;

            Buffer.BlockCopy(data, offset, Data.BlockLight, 0, nibbletotal);
            offset += nibbletotal;

            if (SkyLightSent)
            {
                Buffer.BlockCopy(data, offset, Data.SkyLight, 0, nibbletotal);
                offset += nibbletotal;
            }

            // Not used?
            //Buffer.BlockCopy(data, offset, Data.AddArray, 0, aBlocks / 2);
            //offset += aBlocks / 2;

            if (GroundUp)
            {
                Buffer.BlockCopy(data, offset, Data.Biome, 0, 256);
                offset += 256;
            }


        }

        private void HandleChunkData(byte[] data)
        {
            var sectionCount = GetSectionCount(PrimaryBitMap);

            // Run through the sections
            // TODO: Support block IDs >255
            for (var y = 0; y < 16; y++)
            {
                if ((PrimaryBitMap & (1 << y)) > 0)
                {
                    var temp1 = new byte[2048];
                    var temp2 = new byte[2048];
                    var temp3 = new byte[2048];

                    // Blocks
                    Array.Copy(data, y*BlockDataLength, Sections[y].Blocks, 0, BlockDataLength);

                    // Metadata
                    Array.Copy(data, (BlockDataLength*sectionCount) + (y*NibbleDataLength),
                        temp1, 0, NibbleDataLength);
                    Sections[y].Metadata = CreateMetadataBytes(temp1); // Convert half-byte to one-byte.

                    // BlocksLight
                    Array.Copy(data, ((BlockDataLength + NibbleDataLength)*sectionCount) + (y*NibbleDataLength),
                        temp2, 0, NibbleDataLength);
                    Sections[y].BlockLight = CreateMetadataBytes(temp2); // Convert half-byte to one-byte.

                    // SkyLight
                    if (SkyLightSent)
                    {
                        Array.Copy(data,
                            ((BlockDataLength + NibbleDataLength + NibbleDataLength)*sectionCount) +
                            (y*NibbleDataLength), temp3, 0, NibbleDataLength);
                        Sections[y].Skylight = CreateMetadataBytes(temp3); // Convert half-byte to one-byte.
                    }
                }
            }
            if (GroundUp)
                Array.Copy(data, data.Length - Biomes.Length, Biomes, 0, Biomes.Length);

        }

        public byte[] ReadChunkData(byte[] deCompressed)
        {
            var chunkLength = (BlockDataLength + (NibbleDataLength * 2) + (SkyLightSent ? NibbleDataLength : 0)) *
                    GetSectionCount(PrimaryBitMap) +
                        NibbleDataLength * GetSectionCount(AddBitMap) + (Width * Depth);

            var chunkData = new byte[chunkLength];
            Array.Copy(deCompressed, 0, chunkData, 0, chunkLength);

            var nextChunksData = new byte[deCompressed.Length - chunkLength];
            Array.Copy(deCompressed, chunkLength, nextChunksData, 0, nextChunksData.Length);

            HandleChunkData(chunkData);

            return nextChunksData;
        }

        #endregion MapChunkBulk

        byte[] CreateMetadataBytes(byte[] oldMeta)
        {
            byte[] newMeta = new byte[4096];

            for (int i = 0; i < oldMeta.Length; i++)
            {
                byte block2 = (byte)((oldMeta[i] >> 4) & 15);
                byte block1 = (byte)(oldMeta[i] & 15);

                newMeta[(i * 2)] = block1;
                newMeta[(i * 2) + 1] = block2;
            }

            return newMeta;
        }



        public void UpdateBlock(Coordinates3D coordinates, int id)
        {
            // -- Updates the block in this chunk.
            
            decimal ChunkX = decimal.Divide(coordinates.X, 16);
            decimal ChunkZ = decimal.Divide(coordinates.Y, 16);
            
            ChunkX = Math.Floor(ChunkX);
            ChunkZ = Math.Floor(ChunkZ);

            if (ChunkX != Coordinates.X || ChunkZ != Coordinates.Z)
                return; // -- Block is not in this chunk, user-error somewhere.

            Section thisSection = GetSectionByNumber(coordinates.Y);
            thisSection.SetBlock(GetXinSection(coordinates.X), GetPositionInSection(coordinates.Y),
                GetZinSection(coordinates.Z), id);

        }

        public void SetBlockMetadata(Vector3 vector3, byte data)
        {
            // -- Update the Skylight and Metadata on this block.
            Section thisSection = GetSectionByNumber((int) vector3.Y);
            thisSection.SetBlockMetadata(GetXinSection((int) vector3.Z), GetPositionInSection((int) vector3.Y),
                GetZinSection((int) vector3.Z), data);
        }

        private Section GetSectionByNumber(int blockY)
        {
            return Sections[(byte)(blockY / 16)];
        }
        private int GetXinSection(int BlockX)
        {
            return Math.Abs(BlockX - (Coordinates.X * 16));
        }
        private int GetPositionInSection(int blockY)
        {
            return blockY % 16; // Credits: SirCmpwn Craft.net
        }
        private int GetZinSection(int BlockZ)
        {
            if (Coordinates.Z == 0)
                return BlockZ;

            return BlockZ % Coordinates.Z;
        }

    }
    //*/
}
