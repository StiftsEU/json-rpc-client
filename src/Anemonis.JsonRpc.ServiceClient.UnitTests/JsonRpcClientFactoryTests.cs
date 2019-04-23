using System;
using System.Threading;
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

        #region Types

        public interface IServiceInvalidReturnType
        {
            [JsonRpcMethod("m")]
            long InvokeAsync();
        }

        public interface IServiceParameterRef
        {
            [JsonRpcMethod("m")]
            Task InvokeAsync(ref long parameter);
        }

        public interface IServiceParameterOut
        {
            [JsonRpcMethod("m")]
            Task InvokeAsync(out long parameter);
        }

        public interface IServiceParametersNoneCountInvalid
        {
            [JsonRpcMethod("m")]
            Task InvokeAsync(long parameter);
        }

        public interface IServiceParametersByPositionCountInvalid
        {
            [JsonRpcMethod("m", 0)]
            Task InvokeAsync(long parameter1, long paramerer2);
        }

        public interface IServiceParametersByNameCountInvalid
        {
            [JsonRpcMethod("m", "p")]
            Task InvokeAsync(long parameter1, long paramerer2);
        }

        public interface IServiceParametersByPositionPositionsInvalid
        {
            [JsonRpcMethod("m", 0, 0)]
            Task InvokeAsync(long parameter1, long paramerer2);
        }

        public interface IServiceParametersByNameNamesInvalid
        {
            [JsonRpcMethod("m", "p", "p")]
            Task InvokeAsync(long parameter1, long paramerer2);
        }

        public interface IServiceProperty
        {
            long Status
            {
                get;
            }
        }

        public interface IServiceEvent
        {
            event EventHandler<EventArgs> Completed;
        }

        public interface IServiceSharedMethodName
        {
            [JsonRpcMethod("m0")]
            Task InvokeAsync();

            [JsonRpcMethod("m1", 0)]
            Task InvokeAsync(long parameter);

            [JsonRpcMethod("m2", 0)]
            Task InvokeAsync(string parameter);
        }

        public interface IService : IDisposable
        {
            [JsonRpcMethod("m000")]
            Task InvokeT000Async();

            [JsonRpcMethod("m001")]
            Task InvokeT001Async(CancellationToken cancellationToken);

            [JsonRpcMethod("m010", 0)]
            Task InvokeT010Async(long parameter);

            [JsonRpcMethod("m011", 0)]
            Task InvokeT011Async(long parameter, CancellationToken cancellationToken);

            [JsonRpcMethod("m020", "p")]
            Task InvokeT020Async(long parameter);

            [JsonRpcMethod("m021", "p")]
            Task InvokeT021Async(long parameter, CancellationToken cancellationToken);

            [JsonRpcMethod("m100")]
            Task<long> InvokeT100Async();

            [JsonRpcMethod("m101")]
            Task<long> InvokeT101Async(CancellationToken cancellationToken);

            [JsonRpcMethod("m110", 0)]
            Task<long> InvokeT110Async(long parameter);

            [JsonRpcMethod("m111", 0)]
            Task<long> InvokeT111Async(long parameter, CancellationToken cancellationToken);

            [JsonRpcMethod("m120", "p")]
            Task<long> InvokeT120Async(long parameter);

            [JsonRpcMethod("m121", "p")]
            Task<long> InvokeT121Async(long parameter, CancellationToken cancellationToken);

            [JsonRpcMethod("m200", typeof(long))]
            Task<long> InvokeT200Async();

            [JsonRpcMethod("m201", typeof(long))]
            Task<long> InvokeT201Async(CancellationToken cancellationToken);

            [JsonRpcMethod("m210", typeof(long), 0)]
            Task<long> InvokeT210Async(long parameter);

            [JsonRpcMethod("m211", typeof(long), 0)]
            Task<long> InvokeT211Async(long parameter, CancellationToken cancellationToken);

            [JsonRpcMethod("m220", typeof(long), "p")]
            Task<long> InvokeT220Async(long parameter);

            [JsonRpcMethod("m221", typeof(long), "p")]
            Task<long> InvokeT221Async(long parameter, CancellationToken cancellationToken);
        }

        #endregion
    }
}
