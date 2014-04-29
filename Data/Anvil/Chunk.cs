using System;
using MineLib.Network.Data;

namespace MineLib.ClientWrapper.Data.Anvil
{
    public class Chunk
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

        public Chunk()
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
                    Array.Copy(data, y*BlockDataLength, Sections[y].RawBlocks, 0, BlockDataLength);

                    // Metadata
                    Array.Copy(data, (BlockDataLength*sectionCount) + (y*NibbleDataLength),
                        temp1, 0, NibbleDataLength);
                    Sections[y].RawMetadata = CreateMetadataBytes(temp1); // Convert half-byte to one-byte.

                    // BlocksLight
                    Array.Copy(data, ((BlockDataLength + NibbleDataLength)*sectionCount) + (y*NibbleDataLength),
                        temp2, 0, NibbleDataLength);
                    Sections[y].RawBlockLight = CreateMetadataBytes(temp2); // Convert half-byte to one-byte.

                    // SkyLight
                    if (SkyLightSent)
                    {
                        Array.Copy(data,
                            ((BlockDataLength + NibbleDataLength + NibbleDataLength)*sectionCount) +
                            (y*NibbleDataLength), temp3, 0, NibbleDataLength);
                        Sections[y].RawSkylight = CreateMetadataBytes(temp3); // Convert half-byte to one-byte.
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

        public void SetBlock(Coordinates3D coordinates, Block block)
        {
            // -- Updates the block in this chunk.

            var ChunkX = decimal.Divide(coordinates.X, 16);
            var ChunkZ = decimal.Divide(coordinates.Z, 16);

            ChunkX = Math.Floor(ChunkX);
            ChunkZ = Math.Floor(ChunkZ);

            // -- https://github.com/Azzi777/Umbra-Voxel-Engine/blob/master/Umbra%20Voxel%20Engine/Implementations/ChunkManager.cs#L172
            if (ChunkX != Coordinates.X || ChunkZ != Coordinates.Z)
                throw new Exception("You stupid asshole!"); 

            var thisSection = GetSectionByNumber(coordinates.Y);
            thisSection.SetBlock(GetSectionCoordinates(coordinates), block);

        }

        public Block GetBlock(Coordinates3D coordinates)
        {
            var thisSection = GetSectionByNumber(coordinates.Y);
            return thisSection.GetBlock(GetSectionCoordinates(coordinates));
        }

        public byte GetBlockLight(int x, int y, int z)
        {
            var thisSection = GetSectionByNumber(y);
            return thisSection.GetBlockLighting(GetXinSection(x), GetPositionInSection(y), GetZinSection(z));
        }

        public void SetBlockLight(int x, int y, int z, byte light)
        {
            var thisSection = GetSectionByNumber(y);
            thisSection.SetBlockLighting(GetXinSection(x), GetPositionInSection(y), GetZinSection(z), light);
        }

        public byte GetBlockSkylight(int x, int y, int z)
        {
            var thisSection = GetSectionByNumber(y);
            return thisSection.GetBlockSkylight(GetXinSection(x), GetPositionInSection(y), GetZinSection(z));
        }

        public void SetBlockSkylight(int x, int y, int z, byte light)
        {
            var thisSection = GetSectionByNumber(y);
            thisSection.SetBlockSkylight(GetXinSection(x), GetPositionInSection(y), GetZinSection(z), light);
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
        #endregion
    }
}
