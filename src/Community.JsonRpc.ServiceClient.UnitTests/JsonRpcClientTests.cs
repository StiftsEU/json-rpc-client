using System;
using System.Data.JsonRpc;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Community.JsonRpc.ServiceClient.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Community.JsonRpc.ServiceClient.UnitTests
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
            Assert.ThrowsException<UriFormatException>(() =>
                new JsonRpcClient("/api"));
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
            Assert.ThrowsException<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative)));
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
            Assert.ThrowsException<UriFormatException>(() =>
                new JsonRpcClient("/api", new HttpClient(new TestHttpHandler())));
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
            Assert.ThrowsException<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithInvokerWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost", UriKind.Absolute), default(HttpMessageInvoker)));
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
            Assert.ThrowsException<UriFormatException>(() =>
                new JsonRpcClient("/api", new JsonSerializer()));
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
            Assert.ThrowsException<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative), new JsonSerializer()));
        }

        [TestMethod]
        public void ConstructorWithSerializerWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost", UriKind.Absolute), default(JsonSerializer)));
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
            Assert.ThrowsException<UriFormatException>(() =>
                new JsonRpcClient("/api", new JsonSerializer(), new HttpClient(new TestHttpHandler())));
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
            Assert.ThrowsException<ArgumentException>(() =>
                new JsonRpcClient(new Uri("/api", UriKind.Relative), new JsonSerializer(), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsUriAndSerializerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost", UriKind.Absolute), default(JsonSerializer), new HttpClient(new TestHttpHandler())));
        }

        [TestMethod]
        public void ConstructorWithSerializerAndInvokerWhenServiceUriIsUriAndInvokerIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                new JsonRpcClient(new Uri("https://localhost", UriKind.Absolute), new JsonSerializer(), default(HttpMessageInvoker)));
        }

        //###################################################################################################

        [TestMethod]
        public void GenerateRequestId()
        {
            using (var client = new TestJsonRpcClient())
            {
                var requestId = client.PublicGenerateRequestId();

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

        //###################################################################################################

        private static byte[] CompressWithBrotli(byte[] content)
        {
            using (var outputStream = new MemoryStream())
            {
                using (var compressionStream = new BrotliStream(outputStream, CompressionLevel.Optimal))
                {
                    compressionStream.Write(content, 0, content.Length);
                }

                return outputStream.ToArray();
            }
        }
    }
}