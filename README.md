## Community.JsonRpc.ServiceClient

A lightweight [JSON-RPC 2.0](http://www.jsonrpc.org/specification) service client based on the [JSON-RPC 2.0 Transport: HTTP](https://www.simple-is-better.org/json-rpc/transport_http.html) specification..

[![NuGet package](https://img.shields.io/nuget/v/Community.JsonRpc.ServiceClient.svg?style=flat-square)](https://www.nuget.org/packages/Community.JsonRpc.ServiceClient)

### Features

- The client supports operation cancellation via the `CancellationToken`.
- The client supports `gzip` and `DEFLATE` response encodings.
- The client supports usage of a custom `HttpMessageInvoker` instance to execute HTTP requests.

### Specifics

- The client uses string representation of an UUID as a message identifier.

### Limitations

- Working with batch requests is not supported.
- Working with optional error data value is not supported.

### Examples

Retrieving RANDOM.ORG service API key usage information:

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
\+
```cs
using (var client = new JsonRpcClient("https://api.random.org/json-rpc/2/invoke"))
{
    var parameters = new Dictionary<string, object>
    {
        ["apiKey"] = Guid.Empty.ToString()
    };

    var result = await client.InvokeAsync<KeyUsage>("getUsage", parameters);

    Console.WriteLine(result.BitsLeft);
}
```