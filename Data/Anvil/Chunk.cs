using System;

namespace MineLib.ClientWrapper.Data.Anvil
{
    public class Chunk
    {
        public int X, Z, BlocksCount, aBlocks;
        public short PrimaryBitMap, AddBitMap;
        public byte[] BlocksType;
        public byte[] BlocksMetadata;
        public byte[] BlocksLight;
        public byte[] SkyLight;
        public byte[] AddArray;
        public byte[] BiomeArray;

        public bool SkyLightSent, GroundUpContinuous;
        public Section[] Sections;

        public Chunk(int x, int z, short primaryBitmap, short addBitmap, bool skyLightSent, bool groundUpContinuous)
        {
            X = x;
            Z = z;
            PrimaryBitMap = primaryBitmap;
            AddBitMap = addBitmap;
            SkyLightSent = skyLightSent;
            GroundUpContinuous = groundUpContinuous;

            Sections = new Section[16];

            BlocksCount = 0;
            aBlocks = 0;

            CreateSections();
        }

        /// <summary>
        ///     Creates the chunk sections for this column based on the primary and add bitmasks.
        /// </summary>
        private void CreateSections()
        {
            for (int i = 0; i < 16; i++)
            {
                if ((PrimaryBitMap & (1 << i)) != 0)
                {
                    BlocksCount++;
                    Sections[i] = new Section((byte) i);
                }
            }

            for (int i = 0; i < 16; i++)
            {
                if ((AddBitMap & (1 << i)) != 0)
                    aBlocks++;
            }

            // -- Number of sections * Blocks per section = Blocks in this "Chunk"
            BlocksCount = BlocksCount*4096;
        }

        /// <summary>
        ///     Populates the chunk sections contained in this chunk column with their information.
        /// </summary>
        private void Populate()
        {
            int offset = 0, current = 0, metaOff = 0, lightOff = 0, skyLightOff = 0;

            for (int i = 0; i < 16; i++)
            {
                if ((PrimaryBitMap & (1 << i)) != 0)
                {
                    var temp = new byte[4096];
                    var temp2 = new byte[2048];
                    var temp3 = new byte[2048];
                    var temp4 = new byte[2048];

                    Buffer.BlockCopy(BlocksType, offset, temp, 0, 4096); // -- Block IDs
                    Buffer.BlockCopy(BlocksMetadata, metaOff, temp2, 0, 2048); // -- Metadata.
                    Buffer.BlockCopy(BlocksLight, lightOff, temp3, 0, 2048); // -- Block lighting.
                    Buffer.BlockCopy(SkyLight, skyLightOff, temp4, 0, 2048);

                    // Strange. Some Sections can be null.
                    Section mySection = Sections[current];

                    mySection.BlocksType = temp;
                    mySection.BlocksMetadata = CreateMetadataBytes(temp2);
                    mySection.BlocksLight = CreateMetadataBytes(temp3);
                    mySection.SkyLight = CreateMetadataBytes(temp4);

                    offset += 4096;
                    metaOff += 2048;
                    lightOff += 2048;
                    skyLightOff += 2048;

                    current += 1;
                }
            }

            // -- Free the memory, everything is now stored in sections.
            BlocksType = null;
            BlocksMetadata = null;
        }

        /// <summary>
        ///     Expand the compressed Metadata (half-byte per block) into single-byte per block for easier reading.
        /// </summary>
        /// <param name="oldMeta">Old (2048-byte) Metadata</param>
        /// <returns>4096 uncompressed Metadata</returns>
        public byte[] CreateMetadataBytes(byte[] oldMeta)
        {
            var newMeta = new byte[4096];

            for (int i = 0; i < oldMeta.Length; i++)
            {
                var block2 = (byte) ((oldMeta[i] >> 4) & 15);
                var block1 = (byte) (oldMeta[i] & 15);

                newMeta[(i*2)] = block1;
                newMeta[(i*2) + 1] = block2;
            }

            return newMeta;
        }

        /// <summary>
        ///     Takes this chunk's portion of data from a byte array.
        /// </summary>
        /// <param name="deCompressed">The byte array containing this chunk's data at the front.</param>
        /// <returns>The byte array with this chunk's bytes removed.</returns>
        public byte[] GetData(byte[] deCompressed)
        {
            // -- Loading chunks, network handler hands off the decompressed bytes
            // -- This function takes its portion, and returns what's left.

            byte[] temp;
            int offset = 0;

            BlocksType = new byte[BlocksCount];
            BlocksMetadata = new byte[BlocksCount/2]; // -- Contains block Metadata.
            BlocksLight = new byte[BlocksCount/2];

            if (SkyLightSent)
                SkyLight = new byte[BlocksCount/2];

            AddArray = new byte[BlocksCount/2];

            if (GroundUpContinuous)
                BiomeArray = new byte[256];

            Buffer.BlockCopy(deCompressed, 0, BlocksType, 0, BlocksCount);
            offset += BlocksCount;

            Buffer.BlockCopy(deCompressed, offset, BlocksMetadata, 0, BlocksCount/2); // -- Copy in Metadata
            offset += BlocksCount/2;

            Buffer.BlockCopy(deCompressed, offset, BlocksLight, 0, BlocksCount/2);
            offset += BlocksCount/2;

            if (SkyLightSent)
            {
                Buffer.BlockCopy(deCompressed, offset, SkyLight, 0, BlocksCount/2);
                offset += BlocksCount/2;
            }

            Buffer.BlockCopy(deCompressed, offset, AddArray, 0, aBlocks/2);
            offset += aBlocks/2;

            if (GroundUpContinuous)
            {
                Buffer.BlockCopy(deCompressed, offset, BiomeArray, 0, 256);
                offset += 256;
            }

            temp = new byte[deCompressed.Length - offset];
            Buffer.BlockCopy(deCompressed, offset, temp, 0, temp.Length);

            Populate(); // -- Populate all of our sections with the bytes we just aquired.

            return temp;
        }

        public void UpdateBlock(int Bx, int By, int Bz, int id)
        {
            // -- Updates the block in this chunk.

            decimal ChunkX = decimal.Divide(Bx, 16);
            decimal ChunkZ = decimal.Divide(By, 16);

            ChunkX = Math.Floor(ChunkX);
            ChunkZ = Math.Floor(ChunkZ);

            if (ChunkX != X || ChunkZ != Z)
                return; // -- Block is not in this chunk, user-error somewhere.

            Section thisSection = GetSectionByNumber(By);
            thisSection.SetBlock(GetXinSection(Bx), GetPositionInSection(By), GetZinSection(Bz), id);
        }

        public int GetBlockId(int Bx, int By, int Bz)
        {
            Section thisSection = GetSectionByNumber(By);
            return thisSection.GetBlock(GetXinSection(Bx), GetPositionInSection(By), GetZinSection(Bz)).ID;
        }

        public Block GetBlock(int Bx, int By, int Bz)
        {
            Section thisSection = GetSectionByNumber(By);
            return thisSection.GetBlock(GetXinSection(Bx), GetPositionInSection(By), GetZinSection(Bz));
        }

        public int GetBlockMetadata(int Bx, int By, int Bz)
        {
            Section thisSection = GetSectionByNumber(By);
            return thisSection.GetBlockMetadata(GetXinSection(Bx), GetPositionInSection(By), GetZinSection(Bz));
        }

        public void SetBlockData(int Bx, int By, int Bz, byte data)
        {
            // -- Update the Skylight and Metadata on this block.
            Section thisSection = GetSectionByNumber(By);
            thisSection.SetBlockMetadata(GetXinSection(Bx), GetPositionInSection(By), GetZinSection(Bz), data);
        }

        public byte GetBlockLight(int x, int y, int z)
        {
            Section thisSection = GetSectionByNumber(y);
            return thisSection.GetBlockLighting(GetXinSection(x), GetPositionInSection(y), GetZinSection(z));
        }

        public void SetBlockLight(int x, int y, int z, byte light)
        {
            Section thisSection = GetSectionByNumber(y);
            thisSection.SetBlockLighting(GetXinSection(x), GetPositionInSection(y), GetZinSection(z), light);
        }

        public byte GetBlockSkylight(int x, int y, int z)
        {
            Section thisSection = GetSectionByNumber(y);
            return thisSection.GetBlockSkylight(GetXinSection(x), GetPositionInSection(y), GetZinSection(z));
        }

        public void SetBlockSkylight(int x, int y, int z, byte light)
        {
            Section thisSection = GetSectionByNumber(y);
            thisSection.SetBlockSkylight(GetXinSection(x), GetPositionInSection(y), GetZinSection(z), light);
        }

        public byte GetBlockBiome(int x, int z)
        {
            return BiomeArray[(z*16) + x];
        }

        public void SetBlockBiome(int x, int z, byte biome)
        {
            BiomeArray[(z*16) + x] = biome;
        }

        #region Helping Methods

        private Section GetSectionByNumber(int blockY)
        {
            return Sections[(byte) (blockY/16)];
        }

        private int GetXinSection(int BlockX)
        {
            return Math.Abs(BlockX - (X*16));
        }

        private int GetPositionInSection(int blockY)
        {
            return blockY%16; // Credits: SirCmpwn Craft.net
        }

        private int GetZinSection(int BlockZ)
        {
            if (Z == 0)
                return BlockZ;

            return BlockZ%Z;
        }

        #endregion
    }
}