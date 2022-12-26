namespace AssociationRegistry.Test.Admin.Api.IntegrationTests.Fixtures;

using AssociationRegistry.Admin.Api.Infrastructure;

public class AdminApiClient
{
    private readonly HttpClient _httpClient;

    public AdminApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<HttpResponseMessage> GetDetail(string vCode, long? expectedSequence = null)
        => expectedSequence == null ?
            _httpClient.GetAsync($"/v1/verenigingen/{vCode}") :
            _httpClient.GetAsync($"/v1/verenigingen/{vCode}?{WellknownParameters.ExpectedSequence}={expectedSequence}");
}
