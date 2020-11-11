using System;
using System.Threading.Tasks;

using Anemonis.JsonRpc.ServiceClient.UnitTests.TestStubs;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anemonis.JsonRpc.ServiceClient.UnitTests
{
    [TestClass]
    public sealed class JsonRpcClientFactoryTests
    {
        [TestMethod]
        public void CreateWithExeceutorWhenExecutorIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                JsonRpcClientFactory.Create<IService>((JsonRpcClient)null));
        }

        [TestMethod]
        public void CreateWithServiceUriStringWhenServiceUriIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                JsonRpcClientFactory.Create<IService>((string)null));
        }

        [TestMethod]
        public void CreateWithServiceUriUriWhenServiceUriIsNull()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                JsonRpcClientFactory.Create<IService>((Uri)null));
        }

        [TestMethod]
        public void CreateWhenTypeIsNotInterface()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<object>(executor));
        }

        [TestMethod]
        public void CreateWhenParameterIsRef()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceParameterRef>(executor));
        }

        [TestMethod]
        public void CreateWhenParameterIsOut()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceParameterOut>(executor));
        }

        [TestMethod]
        public void CreateWhenParametersNoneAndCountIsInvalid()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceParametersNoneCountInvalid>(executor));
        }

        [TestMethod]
        public void CreateWhenParametersByPositionAndCountIsInvalid()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceParametersByPositionCountInvalid>(executor));
        }

        [TestMethod]
        public void CreateWhenParametersByNameAndCountIsInvalid()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceParametersByNameCountInvalid>(executor));
        }

        [TestMethod]
        public void CreateWhenParametersByPositionAndPositionsAreInvalid()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceParametersByPositionPositionsInvalid>(executor));
        }

        [TestMethod]
        public void CreateWhenParametersByNameAndNamesAreInvalid()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceParametersByNameNamesInvalid>(executor));
        }

        [TestMethod]
        public void CreateWhenInterfaceDefinesProperty()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceProperty>(executor));
        }

        [TestMethod]
        public void CreateWhenInterfaceDefinesEvent()
        {
            var executor = new TestJsonRpcClient();

            Assert.ThrowsException<InvalidOperationException>(() =>
                JsonRpcClientFactory.Create<IServiceEvent>(executor));
        }

        [TestMethod]
        public async Task CreateWhenInterfaceDefinesMethodWithBody()
        {
            var executor = new TestJsonRpcClient();
            var service = JsonRpcClientFactory.Create<IServiceMethodWithBody>(executor);

            Assert.IsNotNull(service);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(() =>
                service.InvokeAsync());
        }

        [TestMethod]
        public void CreateWithSharedMethodName()
        {
            var executor = new TestJsonRpcClient();
            var service = JsonRpcClientFactory.Create<IServiceSharedMethodName>(executor);

            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void Create()
        {
            var executor = new TestJsonRpcClient();
            var service = JsonRpcClientFactory.Create<IService>(executor);

            Assert.IsNotNull(service);

            service.Dispose();
        }
    }
}
