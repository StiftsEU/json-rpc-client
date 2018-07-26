using System;
using System.Collections.Generic;
using System.Data.JsonRpc;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Community.JsonRpc.ServiceClient.UnitTests.Internal;
using Community.JsonRpc.ServiceClient.UnitTests.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Community.JsonRpc.ServiceClient.UnitTests
{
    [TestClass]
    public sealed class JsonRpcClientTests
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
        public async Task InvokeWhenMethodIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsNullAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsNullAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsNullAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsNullAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsNullAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsNullAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>((string)null, default(JsonRpcId), parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenMethodIsSystem()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsSystem()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsSystem()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsSystem()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m"));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsSystem()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsSystemAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenMethodIsSystemAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("rpc.m", parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWithParametersByPositionWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithParametersByNameWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndParametersByPositionWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndParametersByNameWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierAndParametersByPositionWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierAndParametersByNameWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierAndParametersByPositionWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (object[])null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierAndParametersByNameWhenParametersIsNull()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = (Dictionary<string, object>)null;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWithResultWhenIdentifierIsEmpty()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenIdentifierIsEmptyAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenIdentifierIsEmptyAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenIdentifierIsEmpty()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId)));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenIdentifierIsEmptyAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId), parameters));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenIdentifierIsEmptyAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };

            await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
                client.InvokeAsync<long, long>("m", default(JsonRpcId), parameters));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelled()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenCancellationTokenIsCancelled()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenCancellationTokenIsCancelled()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenCancellationTokenIsCancelled()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelled()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByPosition()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new object[] { 1L, 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        [TestMethod]
        public async Task InvokeWithResultAndErrorDataAndIdentifierWhenCancellationTokenIsCancelledAndParametersAreByName()
        {
            var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler()));
            var parameters = new Dictionary<string, object> { ["p1"] = 1L, ["p2"] = 2L };
            var cancellationTokenSource = new CancellationTokenSource();

            cancellationTokenSource.Cancel();

            await Assert.ThrowsExceptionAsync<OperationCanceledException>(() =>
                client.InvokeAsync<long, long>("m", 0L, parameters, cancellationTokenSource.Token));
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenHttpStatusCodeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync("m"));

                Assert.AreEqual(HttpStatusCode.BadRequest, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task InvokeWhenHttpContentTypeIsEmpty()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = null;
                content.Headers.ContentLength = 0L;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task InvokeWhenHttpContentTypeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var content = new ByteArrayContent(Array.Empty<byte>());

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentLength = 0L;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcRequestException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreEqual(HttpStatusCode.OK, exception.StatusCode);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIsBatch()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString("Assets.batch_true.json"));
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIdIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var contentBytes = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString("Assets.result_true_id_invalid.json"));
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResultTypeIsInvalid()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_type_invalid.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseContractIsInvalidAndResponseIsNotExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_true_unexpected.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync("m"));

                Assert.AreEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseContractIsInvalidAndResponseIsExpected()
        {
            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcContractException>(() =>
                    client.InvokeAsync<long>("m"));

                Assert.AreNotEqual(string.Empty, exception.RequestId);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIsNotExpected()
        {
            var requestAcceptHeader = default(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                requestAcceptHeader = request.Headers.Accept;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                await client.InvokeAsync("m");

                Assert.IsNotNull(requestAcceptHeader);
                CollectionAssert.Contains(requestAcceptHeader.ToArray(), new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseIsExpected()
        {
            var requestAcceptHeader = default(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                requestAcceptHeader = request.Headers.Accept;

                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_true_success_true.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var result = await client.InvokeAsync<long>("m");

                Assert.IsNotNull(requestAcceptHeader);
                CollectionAssert.Contains(requestAcceptHeader.ToArray(), new MediaTypeWithQualityHeaderValue("application/json"));
                Assert.AreEqual(1L, result);
            }
        }

        [TestMethod]
        public async Task InvokeWhenResponseWithErrorIsExpected()
        {
            var requestAcceptHeader = default(HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue>);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)(async (request) =>
            {
                requestAcceptHeader = request.Headers.Accept;

                var requestString = await request.Content.ReadAsStringAsync();
                var requestObject = JObject.Parse(requestString);
                var responseObject = JObject.Parse(EmbeddedResourceManager.GetString("Assets.result_true_success_false.json"));

                responseObject["id"] = requestObject["id"];

                var contentBytes = Encoding.UTF8.GetBytes(responseObject.ToString());
                var content = new ByteArrayContent(contentBytes);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                content.Headers.ContentLength = contentBytes.Length;

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = content
                };

                return message;
            });

            using (var client = new JsonRpcClient("https://localhost", new HttpClient(new TestHttpHandler(handler))))
            {
                var exception = await Assert.ThrowsExceptionAsync<JsonRpcServiceException>(() =>
                    client.InvokeAsync<long, long>("m"));

                Assert.IsNotNull(requestAcceptHeader);
                CollectionAssert.Contains(requestAcceptHeader.ToArray(), new MediaTypeWithQualityHeaderValue("application/json"));
                Assert.AreEqual(1L, exception.Code);
                Assert.AreEqual("e", exception.Message);
                Assert.IsTrue(exception.HasErrorData);
                Assert.AreEqual(exception.ErrorData, 1L);
            }
        }

        //###################################################################################################

        [TestMethod]
        public async Task InvokeWhenHttpVersionIsSpecified()
        {
            var httpVersion = new Version(2, 0);

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                Assert.AreEqual(httpVersion, request.Version);

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            var httpClient = new HttpClient(new TestHttpHandler(handler));

            using (var client = new TestJsonRpcClientWithHttpVersion("https://localhost", httpClient, httpVersion))
            {
                await client.InvokeAsync("m");
            }
        }

        [TestMethod]
        public async Task InvokeWithAuthorizationHeader()
        {
            var authorizationHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes("PASSWORD")));

            var handler = (Func<HttpRequestMessage, Task<HttpResponseMessage>>)((request) =>
            {
                Assert.IsNotNull(request.Headers.Authorization);
                Assert.AreEqual(authorizationHeader.Scheme, request.Headers.Authorization.Scheme);
                Assert.AreEqual(authorizationHeader.Parameter, request.Headers.Authorization.Parameter);

                var message = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NoContent
                };

                return Task.FromResult(message);
            });

            var httpClient = new HttpClient(new TestHttpHandler(handler));

            using (var client = new TestJsonRpcClientWithAuthorizationHeader("https://localhost", httpClient, authorizationHeader))
            {
                await client.InvokeAsync("m");
            }
        }
    }
}