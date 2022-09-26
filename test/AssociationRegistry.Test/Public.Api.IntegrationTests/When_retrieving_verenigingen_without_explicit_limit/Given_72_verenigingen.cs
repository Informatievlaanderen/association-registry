using AssociationRegistry.Test.Public.Api.IntegrationTests.Fixtures;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AssociationRegistry.Test.Public.Api.IntegrationTests.When_retrieving_verenigingen_without_explicit_limit;

[Collection(VerenigingPublicApiWith72VerenigingenCollection.Name)]
public class Given_72_verenigingen : IClassFixture<VerenigingPublicApiFixtureWith72Verenigingen>
{
    private readonly HttpClient _httpClient;

    public Given_72_verenigingen(VerenigingPublicApiFixtureWith72Verenigingen fixture)
    {
        _httpClient = fixture.HttpClient;
    }

    [Fact]
    //TODO To implement when DB is Connected
    public async Task Then_verenigingen_0_49_are_returned()
    {
        var responseMessage = await _httpClient.GetAsync("/v1/verenigingen");
        var content = await responseMessage.Content.ReadAsStringAsync();
        // var goldenMaster = GetType().GetAssociatedResourceJson(
        //     $"{nameof(Given_72_verenigingen)}_{nameof(Then_verenigingen_0_49_are_returned)}");

        var deserializedContent = JToken.Parse(content);
        // var deserializedGoldenMaster = JToken.Parse(goldenMaster);

        //deserializedContent.Should().BeEquivalentTo(deserializedGoldenMaster);
    }

    [Fact]
    public async Task With_offset_50_Then_verenigingen_50_72_are_returned()
    {
    }
}


