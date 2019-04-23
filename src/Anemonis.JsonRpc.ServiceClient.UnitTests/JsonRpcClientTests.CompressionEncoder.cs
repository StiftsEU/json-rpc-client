using System;
using System.IO;
using System.IO.Compression;

namespace Anemonis.JsonRpc.ServiceClient.UnitTests
{
    public sealed partial class JsonRpcClientTests
    {
        private static class CompressionEncoder
        {
            public static byte[] Encode(byte[] content, string algorithm)
            {
                switch (algorithm)
                {
                    case "br":
                        {
                            using (var contentStream = new MemoryStream())
                            {
                                using (var compressionStream = new BrotliStream(contentStream, CompressionLevel.Optimal))
                                {
                                    compressionStream.Write(content, 0, content.Length);
                                }

                                return contentStream.ToArray();
                            }
                        }
                    default:
                        {
                            throw new NotSupportedException();
                        }
                }
            }
        }
    }
}
