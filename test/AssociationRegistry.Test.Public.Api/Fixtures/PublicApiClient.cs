namespace AssociationRegistry.Test.Public.Api.Fixtures;

using System;
using System.Net.Http;
using System.Threading.Tasks;

public class PublicApiClient : IDisposable
{
    public readonly HttpClient HttpClient;

    public PublicApiClient(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetDetail(string vCode)
        => await HttpClient.GetAsync($"/v1/verenigingen/{vCode}");

    public async Task<HttpResponseMessage> Search(string q)
        => await HttpClient.GetAsync($"/v1/verenigingen/zoeken?q={q}");

    public async Task<HttpResponseMessage> Search(string q, string sort)
        => await HttpClient.GetAsync($"/v1/verenigingen/zoeken?q={q}&sort={sort}");

    public async Task<HttpResponseMessage> GetDocs()
        => await HttpClient.GetAsync("/docs/v1/docs.json?culture=en-GB");

    public void Dispose()
    {
        HttpClient.Dispose();
    }

    public async Task<HttpResponseMessage> GetHoofdactiviteiten()
        => await HttpClient.GetAsync("/v1/hoofdactiviteitenVerenigingsloket");
}
