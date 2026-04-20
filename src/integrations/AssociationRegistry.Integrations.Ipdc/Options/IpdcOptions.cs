namespace AssociationRegistry.Integrations.Ipdc.Options;

using Framework;

public class IpdcOptions
{
    public readonly record struct HttpClientOptions(string BaseUrl, string ApiKey);

    public HttpClientOptions HttpClient { get; init; }
    public void ThrowIfInValid()
    {
        Throw<ArgumentNullException>.IfNullOrWhiteSpace(HttpClient.BaseUrl, nameof(HttpClient.BaseUrl));
        Throw<ArgumentNullException>.IfNullOrWhiteSpace(HttpClient.ApiKey, nameof(HttpClient.ApiKey));
    }
}

