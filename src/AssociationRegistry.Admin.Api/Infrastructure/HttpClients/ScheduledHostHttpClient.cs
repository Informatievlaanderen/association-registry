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

    public async Task<HttpResponseMessage> TriggerPowerBiExport(CancellationToken cancellationToken) =>
        await _httpClient.PostAsync(requestUri: "/v1/trigger/powerbi-export", content: null, cancellationToken);

    public async Task TriggerActiveerErkenningen(CancellationToken cancellationToken) =>
        await _httpClient.PostAsync(requestUri: "/v1/trigger/activeer-erkenningen", content: null, cancellationToken);

    public async Task TriggerVerwerkDubbelen(CancellationToken cancellationToken) =>
        await _httpClient.PostAsync(requestUri: "/v1/trigger/verwerk-dubbelen", content: null, cancellationToken);

    /// <summary>
    ///     Throws when the scheduled host refuses the trigger (e.g. an older version without this job): a caller must not
    ///     get a success for a job that will not run.
    /// </summary>
    public async Task TriggerVervalRelatieaanvragen(CancellationToken cancellationToken)
    {
        using var response = await _httpClient.PostAsync(
            requestUri: "/v1/trigger/verval-relatieaanvragen",
            content: null,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();
    }
}
