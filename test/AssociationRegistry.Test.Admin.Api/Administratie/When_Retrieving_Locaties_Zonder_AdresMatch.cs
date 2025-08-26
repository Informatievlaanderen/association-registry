namespace AssociationRegistry.Test.Admin.Api.Administratie;

using AssociationRegistry.Admin.Schema.Detail;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;
using Wolverine.Http;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class When_Retrieving_Locaties_Zonder_AdresMatch
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_Retrieving_Locaties_Zonder_AdresMatch(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Without_SuperAdmin_Returns403Forbidden()
    {
        var testClient = _fixture.AdminApiClient.HttpClient;
        var result = await testClient.GetAsync("/v1/admin/locaties/zonder-adresmatch");
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task With_SuperAdmin_ReturnsLocatiesZonderAdresMatch()
    {
        var testClient = _fixture.SuperAdminApiClient.HttpClient;
        var result = await testClient.GetAsync("/v1/admin/locaties/zonder-adresmatch");
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Should().NotBeNull();
        var content = result.Content.ReadAsStringAsync().Result;
        var actual = JsonConvert.DeserializeObject<LocatieZonderAdresMatchDocument[]>(content);
        actual.Should().NotBeEmpty();
    }
}

[Collection(nameof(AdminApiCollection))]
public class When_Querying_Deadletters
{
    private readonly EventsInDbScenariosFixture _fixture;

    public When_Querying_Deadletters(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Without_SuperAdmin_Returns403Forbidden()
    {
        var testClient = _fixture.AdminApiClient.HttpClient;
        var result = await testClient.PostAsJsonAsync("dead-letters", new DeadLetterEnvelopeGetRequest());
        result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task With_SuperAdmin_ReturnsLocatiesZonderAdresMatch()
    {
        var testClient = _fixture.SuperAdminApiClient.HttpClient;
        var result = await testClient.PostAsJsonAsync("dead-letters", new DeadLetterEnvelopeGetRequest());
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Content.Should().NotBeNull();
        var content = result.Content.ReadAsStringAsync().Result;
        var actual = JsonConvert.DeserializeObject<DeadLetterEnvelopesFoundResponse>(content);
        actual.Should().NotBeNull();
    }
}
