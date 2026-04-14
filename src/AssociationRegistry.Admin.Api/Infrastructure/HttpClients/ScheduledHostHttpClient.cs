namespace AssociationRegistry.Admin.Api.Infrastructure.HttpClients;

public class ScheduledHostHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;

    public ScheduledHostHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> TriggerBewaartermijnScheduledHost(CancellationToken cancellationToken) =>
        await _httpClient.PostAsync(requestUri: "/v1/trigger/bewaartermijn", content: null, cancellationToken);

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
