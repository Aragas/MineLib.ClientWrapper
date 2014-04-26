using System;
using System.IO;
using System.IO.Compression;

namespace MineLib.ClientWrapper.Data.Anvil
{
    class Decompressor
    {
        // ZLib Decompressor.
        public static byte[] Decompress(byte[] data)
        {
            var trim = new byte[data.Length - 2];
            Buffer.BlockCopy(data, 2, trim, 0, trim.Length);

            using (var compressedStream = new MemoryStream(trim))
            using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress))
            using (var resultStream = new MemoryStream())
            {
                var buffer = new byte[4096];
                int read;

                while ((read = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    resultStream.Write(buffer, 0, read);
                }

                return resultStream.ToArray();
            }
        }
    }
}
