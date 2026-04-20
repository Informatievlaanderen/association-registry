namespace AssociationRegistry.Integrations.Wegwijs.Options;

using Framework;

public class WegwijsOptions
{
    public readonly record struct HttpClientOptions(string BaseUrl);

    public HttpClientOptions HttpClient { get; init; }

    public void ThrowIfInValid()
    {
        Throw<ArgumentNullException>.IfNullOrWhiteSpace(HttpClient.BaseUrl, nameof(HttpClient.BaseUrl));
    }
}
