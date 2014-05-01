using System;
using System.Collections.Generic;
using MineLib.Network.Data;

namespace MineLib.ClientWrapper.Data.Anvil
{
    public class Chunk
    {

        public const int Width = 16, Height = 256, Depth = 16;

        private const int BlockDataLength = Section.Width * Section.Depth * Section.Height;
        private const int NibbleDataLength = BlockDataLength / 2;

        public Coordinates2D Coordinates;
        public ushort PrimaryBitMap;
        public ushort AddBitMap;
        public bool SkyLightSent;
        public bool GroundUp;

        public byte[] Biomes;

        public Section[] Sections;
    
        public Chunk()
        {
            Biomes = new byte[Width * Depth];

            Sections = new Section[16];
            for (var i = 0; i < Sections.Length; i++)
                Sections[i] = new Section((byte)i);
        }

        public override string ToString()
        {
            return string.Format("Filled Sections: {0}", GetFilledSectionsCount());
        }

        private static int GetSectionCount(ushort bitMap)
        {
            // Get the total sections included in the bitMap
            var sectionCount = 0;
            for (var y = 0; y < 16; y++)
            {
                if ((bitMap & (1 << y)) > 0)
                    sectionCount++;
            }

            return sectionCount;
        }

        #region MapChunkBulk

        private void HandleChunkData(byte[] data)
        {
            var sectionCount = GetSectionCount(PrimaryBitMap);

            // Run through the sections
            // TODO: Support block IDs >255
            for (var y = 0; y < 16; y++)
            {
                if ((PrimaryBitMap & (1 << y)) > 0)
                {
                    // Blocks
                    Array.Copy(data, y * BlockDataLength, Sections[y].RawBlocks, 0, BlockDataLength);

                    // Metadata
                    var nibbleMetadata = new byte[2048];
                    Array.Copy(data, (BlockDataLength * sectionCount) + (y * NibbleDataLength),
                        nibbleMetadata, 0, NibbleDataLength);
                    Sections[y].RawMetadata = CreateMetadataBytes(nibbleMetadata); // Convert half-byte to one-byte.

                    // BlocksLight
                    var nibbleBlocksLight = new byte[2048];
                    Array.Copy(data, ((BlockDataLength + NibbleDataLength) * sectionCount) + (y * NibbleDataLength),
                        nibbleBlocksLight, 0, NibbleDataLength);
                    Sections[y].RawBlockLight = CreateMetadataBytes(nibbleBlocksLight); // Convert half-byte to one-byte.

                    // SkyLight
                    if (SkyLightSent)
                    {
                        var nibbleSkyLight = new byte[2048];
                        Array.Copy(data,
                            ((BlockDataLength + NibbleDataLength + NibbleDataLength) * sectionCount) +
                            (y * NibbleDataLength), nibbleSkyLight, 0, NibbleDataLength);
                        Sections[y].RawSkylight = CreateMetadataBytes(nibbleSkyLight); // Convert half-byte to one-byte.
                    }

                    Sections[y].FillBlocks();
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

        private static byte[] CreateMetadataBytes(IList<byte> oldMeta)
        {
            var newMeta = new byte[4096];

            for (var i = 0; i < oldMeta.Count; i++)
            {
                var block2 = (byte)((oldMeta[i] >> 4) & 15);
                var block1 = (byte)(oldMeta[i] & 15);

                newMeta[(i * 2)] = block1;
                newMeta[(i * 2) + 1] = block2;
            }

            return newMeta;
        }

        public void SetBlock(Coordinates3D coordinates, Block block)
        {
            // -- Updates the block in this chunk.

            var chunkX = decimal.Divide(coordinates.X, 16);
            var chunkZ = decimal.Divide(coordinates.Z, 16);

            chunkX = Math.Floor(chunkX);
            chunkZ = Math.Floor(chunkZ);

            // -- https://github.com/Azzi777/Umbra-Voxel-Engine/blob/master/Umbra%20Voxel%20Engine/Implementations/ChunkManager.cs#L172
            if (chunkX != Coordinates.X || chunkZ != Coordinates.Z)
                throw new Exception("You stupid asshole!"); 

            var thisSection = GetSectionByNumber(coordinates.Y);
            thisSection.SetBlock(GetSectionCoordinates(coordinates), block);

        }

        public Block GetBlock(Coordinates3D coordinates)
        {
            var thisSection = GetSectionByNumber(coordinates.Y);
            return thisSection.GetBlock(GetSectionCoordinates(coordinates));
        }

        public byte GetBlockLight(Coordinates3D coordinates)
        {
            var thisSection = GetSectionByNumber(coordinates.Y);
            return thisSection.GetBlockLighting(GetSectionCoordinates(coordinates));
        }

        public void SetBlockLight(Coordinates3D coordinates, byte light)
        {
            var thisSection = GetSectionByNumber(coordinates.Y);
            thisSection.SetBlockLighting(GetSectionCoordinates(coordinates), light);
        }

        public byte GetBlockSkylight(Coordinates3D coordinates)
        {
            var thisSection = GetSectionByNumber(coordinates.Y);
            return thisSection.GetBlockSkylight(GetSectionCoordinates(coordinates));
        }

        public void SetBlockSkylight(Coordinates3D coordinates, byte light)
        {
            var thisSection = GetSectionByNumber(coordinates.Y);
            thisSection.SetBlockSkylight(GetSectionCoordinates(coordinates), light);
        }

        public byte GetBlockBiome(Coordinates2D coordinates)
        {
            return Biomes[(coordinates.Z * 16) + coordinates.X];
        }

        public void SetBlockBiome(Coordinates2D coordinates, byte biome)
        {
            Biomes[(coordinates.Z * 16) + coordinates.X] = biome;
        }

        #region Helping Methods

        private Section GetSectionByNumber(int blockY)
        {
            return Sections[(byte)(blockY / 16)];
        }
        private Coordinates3D GetSectionCoordinates(Coordinates3D coordinates)
        {
            return new Coordinates3D
            {
                X = GetXinSection(coordinates.X),
                Y = GetPositionInSection(coordinates.Y),
                Z = GetZinSection(coordinates.Z)
            };
        }
        private int GetXinSection(int blockX)
        {
            return Math.Abs(blockX - (Coordinates.X * 16));
        }
        private static int GetPositionInSection(int blockY)
        {
            return blockY % 16;
        }
        private int GetZinSection(int blockZ)
        {
            if (Coordinates.Z == 0)
                return blockZ;

            return blockZ % Coordinates.Z;
        }

        private int GetFilledSectionsCount()
        {
            var count = 0;
            foreach (var section in Sections)
            {
                if (section.IsFilled)
                    count++;
            }
            return count;
        }

        #endregion

    }
}
