## Community.JsonRpc.ServiceClient

A lightweight [JSON-RPC 2.0](http://www.jsonrpc.org/specification) service client based on the [JSON-RPC 2.0 Transport: HTTP](https://www.simple-is-better.org/json-rpc/transport_http.html) specification.

[![NuGet package](https://img.shields.io/nuget/v/Community.JsonRpc.ServiceClient.svg?style=flat-square)](https://www.nuget.org/packages/Community.JsonRpc.ServiceClient)

### Features

- The client supports operation cancellation via a cancellation token.
- The client supports `gzip` and `DEFLATE` response encodings.
- The client supports usage of a custom HTTP message invoker instance.
- The client supports usage of a specific HTTP message version.

### Specifics

- The client uses string representation of an UUID as a message identifier.

### Limitations

- Working with batch requests is not supported.
- Working with optional error data value is not supported.

### Examples

Retrieving API key usage information for the [RANDOM.ORG](https://api.random.org/json-rpc/2) service:
```cs
public class KeyUsage
{
    [JsonProperty("bitsLeft")]
    public long BitsLeft
    {
        get;
        set;
    }
}
```
```cs
var parameters = new Dictionary<string, object>
{
    ["apiKey"] = "00000000-0000-0000-0000-000000000000"
};

using (var client = new JsonRpcClient("https://api.random.org/json-rpc/2/invoke"))
{
    var result = await client.InvokeAsync<KeyUsage>("getUsage", parameters);

    Console.WriteLine(result.BitsLeft);
}
```
The corresponding JSON-RPC messages:
```json
{
    "jsonrpc": "2.0",
    "method": "getUsage",
    "params": {
        "apiKey": "00000000-0000-0000-0000-000000000000"
    },
    "id": "89999193-0e46-4a14-b471-69191baf2c2b"
}
```
```json
{
    "jsonrpc": "2.0",
    "result": {
        "status": "running",
        "creationTime": "2013-02-01 17:53:40Z",
        "bitsLeft": 970409,
        "requestsLeft": 198935,
        "totalBits": 208391002,
        "totalRequests": 2963905
    },
    "id": "89999193-0e46-4a14-b471-69191baf2c2b"
}
```