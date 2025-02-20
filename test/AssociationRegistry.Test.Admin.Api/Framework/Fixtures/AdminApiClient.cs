namespace AssociationRegistry.Test.Admin.Api.Framework.Fixtures;

using AssociationRegistry.Admin.Api.Infrastructure;
using Helpers;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

public class AdminApiClient : IDisposable
{
    public readonly HttpClient HttpClient;

    public AdminApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
        HttpClient.DefaultRequestHeaders.Add(WellknownHeaderNames.CorrelationId, Guid.NewGuid().ToString());
        HttpClient.DefaultRequestHeaders.Add(WellknownHeaderNames.Initiator, value: "OVO000001");
    }

    public async Task<HttpResponseMessage> GetRoot()
        => await HttpClient.GetAsync("");

    public async Task<HttpResponseMessage> Search(string q)
        => await HttpClient.GetAsync($"/v1/verenigingen/zoeken?q={q}");

    public async Task<HttpResponseMessage> Search(string q, string sort)
        => await HttpClient.GetAsync($"/v1/verenigingen/zoeken?q={q}&sort={sort}");

    public async Task<HttpResponseMessage> GetDetail(string vCode, long? expectedSequence = null)
        => await GetWithPossibleSequence($"/v1/verenigingen/{vCode}", expectedSequence);

    public async Task<HttpResponseMessage> GetLocatieLookup(string vCode, long? expectedSequence = null)
        => await GetWithPossibleSequence($"/v1/projections/admin/locaties/lookup/vCode/{vCode}", expectedSequence);

    public async Task<HttpResponseMessage> GetHistoriek(string vCode, long? expectedSequence = null)
        => await GetWithPossibleSequence($"/v1/verenigingen/{vCode}/historiek", expectedSequence);

    public async Task<HttpResponseMessage> GetKboSyncHistoriek()
        => await HttpClient.GetAsync("/v1/verenigingen/kbo/historiek");

    public async Task<HttpResponseMessage> RegistreerFeitelijkeVereniging(
        string content,
        string? bevestigingsToken = null,
        string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken, bevestigingsToken);
        WithHeaders(version: null, initiator);

        var httpResponseMessage =
            await HttpClient.PostAsync(requestUri: "/v1/verenigingen/feitelijkeverenigingen", content.AsJsonContent());

        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken);

        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
        string content,
        string? bevestigingsToken = null,
        string? initiator = "OVO000001")
    {
        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken, bevestigingsToken);
        WithHeaders(version: null, initiator);

        var httpResponseMessage =
            await HttpClient.PostAsync(requestUri: "/v1/verenigingen/verenigingenzondereigenrechtspersoonlijkheid", content.AsJsonContent());

        AddOrRemoveHeader(WellknownHeaderNames.BevestigingsToken);

        return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> RegistreerKboVereniging(string content, string? initiator = "OVO000001")
    {
        WithHeaders(version: null, initiator);
        var httpResponseMessage = await HttpClient.PostAsync(requestUri: "/v1/verenigingen/kbo", content.AsJsonContent());

        return httpResponseMessage;
    }

    private async Task<HttpResponseMessage> GetWithPossibleSequence(string? requestUri, long? expectedSequence)
        => expectedSequence == null
            ? await HttpClient.GetAsync(requestUri)
            : await HttpClient.GetAsync($"{requestUri}?{WellknownParameters.ExpectedSequence}={expectedSequence}");

    public async Task<HttpResponseMessage> PatchVereniging(
        string vCode,
        string content,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PatchAsync($"/v1/verenigingen/{vCode}", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> StopVereniging(
        string vCode,
        string content,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PostAsync($"/v1/verenigingen/{vCode}/stop", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchVerenigingMetRechtspersoonlijkheid(
        string vCode,
        string content,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PatchAsync($"/v1/verenigingen/{vCode}/kbo", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PostVertegenwoordiger(
        string vCode,
        string content,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PostAsync($"/v1/verenigingen/{vCode}/vertegenwoordigers", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PostContactgegevens(
        string vCode,
        string content,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PostAsync($"/v1/verenigingen/{vCode}/contactgegevens", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchContactgegevens(
        string vCode,
        int contactgegevenId,
        string jsonBody,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PatchAsync($"/v1/verenigingen/{vCode}/contactgegevens/{contactgegevenId}", jsonBody.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchContactgegevensFromKbo(
        string vCode,
        int contactgegevenId,
        string jsonBody,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PatchAsync($"/v1/verenigingen/{vCode}/kbo/contactgegevens/{contactgegevenId}", jsonBody.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchVertegenwoordiger(
        string vCode,
        int vertegenwoordigerId,
        string jsonBody,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PatchAsync($"/v1/verenigingen/{vCode}/vertegenwoordigers/{vertegenwoordigerId}", jsonBody.AsJsonContent());
    }

    public async Task<HttpResponseMessage> DeleteContactgegeven(
        string vCode,
        int contactgegevenId,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"/v1/verenigingen/{vCode}/contactgegevens/{contactgegevenId}", UriKind.Relative),
        };

        return await HttpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteVertegenwoordiger(
        string vCode,
        int vertegenwoordigerId,
        string jsonBody,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"/v1/verenigingen/{vCode}/vertegenwoordigers/{vertegenwoordigerId}", UriKind.Relative),
            Content = jsonBody.AsJsonContent(),
        };

        return await HttpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> PostLocatie(string vCode, string content, long? version = null, string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PostAsync($"/v1/verenigingen/{vCode}/locaties", content.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchLocatie(
        string vCode,
        int locatieId,
        string jsonBody,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PatchAsync($"/v1/verenigingen/{vCode}/locaties/{locatieId}", jsonBody.AsJsonContent());
    }

    public async Task<HttpResponseMessage> PatchMaatschappelijkeZetel(
        string vCode,
        int locatieId,
        string jsonBody,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        return await HttpClient.PatchAsync($"/v1/verenigingen/{vCode}/kbo/locaties/{locatieId}", jsonBody.AsJsonContent());
    }

    public async Task<HttpResponseMessage> DeleteLocatie(
        string vCode,
        int locatieId,
        string jsonBody,
        long? version = null,
        string? initiator = "OVO000001")
    {
        WithHeaders(version, initiator);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"/v1/verenigingen/{vCode}/locaties/{locatieId}", UriKind.Relative),
        };

        return await HttpClient.SendAsync(request);
    }

    private void WithHeaders(long? version, string? initiator)
    {
        AddOrRemoveHeader(HeaderNames.IfMatch, GetIfMatchHeaderValue(version));
        UpdateHeaderIfNotNull(WellknownHeaderNames.Initiator, initiator);
    }

    public async Task<HttpResponseMessage> GetDocsJson()
        => await HttpClient.GetAsync("/docs/v1/docs.json?culture=en-GB");

    private static string? GetIfMatchHeaderValue(long? version)
        => version is not null ? $"W/\"{version}\"" : null;

    private void AddOrRemoveHeader(string headerName, string? headerValue = null)
    {
        HttpClient.DefaultRequestHeaders.Remove(headerName);
        if (headerValue is not null) HttpClient.DefaultRequestHeaders.Add(headerName, headerValue);
    }

    private void UpdateHeaderIfNotNull(string headerName, string? headerValue = null)
    {
        if (headerValue is null) return;
        HttpClient.DefaultRequestHeaders.Remove(headerName);
        HttpClient.DefaultRequestHeaders.Add(headerName, headerValue);
    }

    public void Dispose()
    {
        HttpClient.Dispose();
    }

    public async Task<HttpResponseMessage> GetHoofdactiviteiten()
        => await HttpClient.GetAsync("/v1/hoofdactiviteitenVerenigingsloket");

    public async Task<HttpResponseMessage> RebuildAllAdminProjections(CancellationToken cancellationToken)
        => await HttpClient.PostAsync(requestUri: "/v1/projections/admin/all/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildAdminDetailProjection(CancellationToken cancellationToken)
        => await HttpClient.PostAsync(requestUri: "/v1/projections/admin/detail/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildAdminHistoriekProjection(CancellationToken cancellationToken)
        => await HttpClient.PostAsync(requestUri: "/v1/projections/admin/historiek/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildAdminZoekenProjection(CancellationToken cancellationToken)
        => await HttpClient.PostAsync(requestUri: "/v1/projections/admin/search/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildAdminDuplicateDetectionProjection(CancellationToken cancellationToken)
        => await HttpClient.PostAsync(requestUri: "/v1/projections/admin/duplicatedetection/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildPubliekDetailProjection(CancellationToken cancellationToken)
        => await HttpClient.PostAsync(requestUri: "/v1/projections/public/detail/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> RebuildPubliekZoekenProjection(CancellationToken cancellationToken)
        => await HttpClient.PostAsync(requestUri: "/v1/projections/public/search/rebuild", content: null, cancellationToken);

    public async Task<HttpResponseMessage> DeleteVereniging(
        string vCode,
        string reason,
        long? version = null,
        string initiator = "V0001001")
    {
        WithHeaders(version, initiator);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri($"/v1/verenigingen/{vCode}", UriKind.Relative),
            Content = JsonConvert.SerializeObject(new { Reden = reason }).AsJsonContent(),
        };

        return await HttpClient.SendAsync(request);
    }

    public async Task<HttpResponseMessage> GetJsonLdContext(string contextName)
        => await HttpClient.GetAsync($"/v1/contexten/beheer/{contextName}");

    public async Task<HttpResponseMessage> GetDocs()
        => await HttpClient.GetAsync("/docs/v1/docs.json?culture=en-GB");

}
