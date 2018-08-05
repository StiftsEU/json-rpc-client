﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Community.JsonRpc.ServiceClient.Benchmarks.Resources;
using Community.JsonRpc.ServiceClient.Benchmarks.TestStubs;

namespace Community.JsonRpc.ServiceClient.Benchmarks.TestSuites
{
    public sealed class JsonRpcClientBenchmarks
    {
        private static readonly IReadOnlyDictionary<string, byte[]> _resources = CreateResourceDictionary();
        private static readonly IReadOnlyList<object> _parametersByPosition = CreateParametersByPosition();
        private static readonly IReadOnlyDictionary<string, object> _parametersByName = CreateParametersByName();

        private readonly JsonRpcClient _clientNotification =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler()));
        private readonly JsonRpcClient _clientResponseResult =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler(_resources["response_result"])));
        private readonly JsonRpcClient _clientResponseError =
            new JsonRpcClient("https://localhost", new HttpClient(new JsonRpcClientBenchmarkHandler(_resources["response_error"])));

        private static IReadOnlyDictionary<string, byte[]> CreateResourceDictionary()
        {
            var resources = new Dictionary<string, byte[]>(StringComparer.Ordinal);

            foreach (var code in GetResponseCodes())
            {
                resources[code] = Encoding.UTF8.GetBytes(EmbeddedResourceManager.GetString($"Assets.{code}.json"));
            }

            return resources;
        }

        private static IReadOnlyList<object> CreateParametersByPosition()
        {
            return new object[] { 1L, 2L };
        }

        private static IReadOnlyDictionary<string, object> CreateParametersByName()
        {
            return new Dictionary<string, object>(2, StringComparer.Ordinal) { ["p1"] = 1L, ["p2"] = 2L };
        }

        private static IEnumerable<string> GetResponseCodes()
        {
            return new[] { "response_error", "response_result" };
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=U-ERROR=N")]
        public async Task InvokeAsyncWithNotificationAndNoParams()
        {
            await _clientNotification.InvokeAsync("m");
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=P-ERROR=N")]
        public async Task InvokeAsyncWithNotificationAndParamsByPosition()
        {
            await _clientNotification.InvokeAsync("m", _parametersByPosition);
        }

        [Benchmark(Description = "InvokeAsync-ID=N-PARAMS=N-ERROR=N")]
        public async Task InvokeAsyncWithNotificationAndParamsByName()
        {
            await _clientNotification.InvokeAsync("m", _parametersByName);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=U-ERROR=N")]
        public async Task<long> InvokeAsyncWithResponseResultAndNoParams()
        {
            return await _clientResponseResult.InvokeAsync<long>("m", 0L);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=P-ERROR=N")]
        public async Task<long> InvokeAsyncWithResponseResultAndParamsByPosition()
        {
            return await _clientResponseResult.InvokeAsync<long>("m", 0L, _parametersByPosition);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=N-ERROR=N")]
        public async Task<long> InvokeAsyncWithResponseResultAndParamsByName()
        {
            return await _clientResponseResult.InvokeAsync<long>("m", 0L, _parametersByName);
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=U-ERROR=Y")]
        public async Task<long> InvokeAsyncWithResponseErrorAndNoParams()
        {
            try
            {
                return await _clientResponseError.InvokeAsync<long>("m", 0L);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=P-ERROR=Y")]
        public async Task<long> InvokeAsyncWithResponseErrorAndParamsByPosition()
        {
            try
            {
                return await _clientResponseError.InvokeAsync<long>("m", 0L, _parametersByPosition);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }

        [Benchmark(Description = "InvokeAsync-ID=Y-PARAMS=N-ERROR=Y")]
        public async Task<long> InvokeAsyncWithResponseErrorAndParamsByName()
        {
            try
            {
                return await _clientResponseError.InvokeAsync<long>("m", 0L, _parametersByName);
            }
            catch (JsonRpcServiceException)
            {
                return default;
            }
        }
    }
}