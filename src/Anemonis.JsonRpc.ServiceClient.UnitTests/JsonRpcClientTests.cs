using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Anemonis.JsonRpc.ServiceClient.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Anemonis.JsonRpc.ServiceClient.UnitTests
{
    [TestClass]
    public sealed partial class JsonRpcClientTests
    {
        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((string)null));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("/localhost"));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsStringAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("ftp://localhost"));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("/localhost", UriKind.Relative)));
        }

        [TestMethod]
        public void ConstructorWhenServiceUriIsUriAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("ftp://localhost")));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsStringAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((string)null, new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsStringAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("/localhost", new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsStringAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("ftp://localhost", new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsStringAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient("https://localhost", default(HttpMessageInvoker)));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null, new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("/localhost", UriKind.Relative), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("ftp://localhost"), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost"), default(HttpMessageInvoker)));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsStringAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((string)null, new JsonSerializer()));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsStringAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("/localhost", new JsonSerializer()));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsStringAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("ftp://localhost", new JsonSerializer()));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsStringAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient("https://localhost", default(JsonSerializer)));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsUriAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null, new JsonSerializer()));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsUriAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("/localhost", UriKind.Relative), new JsonSerializer()));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsUriAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("ftp://localhost"), new JsonSerializer()));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost"), default(JsonSerializer)));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsStringAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((string)null, new JsonSerializer(), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsStringAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("/localhost", new JsonSerializer(), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsStringAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient("ftp://localhost", new JsonSerializer(), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsStringAndSerializerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient("https://localhost", default(JsonSerializer), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsStringAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient("https://localhost", new JsonSerializer(), default(HttpMessageInvoker)));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsUriAndIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient((Uri)null, new JsonSerializer(), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsUriAndIsRelative()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("/localhost", UriKind.Relative), new JsonSerializer(), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsUriAndHasInvalidScheme()
        {
            Assert.ThrowsException<FormatException>(() =>
                new JsonRpcClient(new Uri("ftp://localhost"), new JsonSerializer(), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsUriAndSerializerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost"), default(JsonSerializer), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost"), new JsonSerializer(), default(HttpMessageInvoker)));
        }

        [TestMethod]
        public void GetUniqueRequestId()
        {
            using (var client = new TestJsonRpcClient())
            {
                var requestId = client.PublicGetUniqueRequestId();

                Assert.AreNotEqual(JsonRpcIdType.None, requestId.Type);
            }
        }

        [TestMethod]
        public void GetServiceUri()
        {
            using (var client = new TestJsonRpcClient())
            {
                Assert.IsNotNull(client.PublicServiceUri);
                Assert.AreEqual("https://localhost", client.PublicServiceUri.OriginalString);
            }
        }

        [TestMethod]
        public void GetContractResolver()
        {
            using (var client = new TestJsonRpcClient())
            {
                Assert.IsNotNull(client.PublicContractResolver);
            }
        }

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