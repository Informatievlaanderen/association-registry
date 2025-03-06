namespace AssociationRegistry.Test.Public.Api.When_Retrieving_HoofdactiviteitenLijst;

using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using System.Threading.Tasks;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_A_Working_Service
{
    private readonly PublicApiClient _publicApiClient;

    public Given_A_Working_Service(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetHoofdactiviteiten();
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_It_Returns_All_Possible_Values()
    {
        var response = await _publicApiClient.GetHoofdactiviteiten();
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Working_Service)}_{nameof(Then_It_Returns_All_Possible_Values)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
