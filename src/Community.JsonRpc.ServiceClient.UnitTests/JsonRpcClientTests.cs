using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using Community.JsonRpc.ServiceClient.UnitTests.TestStubs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
                new JsonRpcClient("https://localhost", null));
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
                new JsonRpcClient(new Uri("https://localhost", UriKind.Absolute), null));
        }

        //###################################################################################################

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