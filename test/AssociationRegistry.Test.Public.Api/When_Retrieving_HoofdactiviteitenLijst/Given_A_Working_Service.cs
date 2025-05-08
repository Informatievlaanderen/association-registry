namespace AssociationRegistry.Test.Public.Api.When_Retrieving_HoofdactiviteitenLijst;

using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Xunit;

[Collection(nameof(PublicApiCollection))]
public class Given_A_Working_Service
{
    private readonly PublicApiClient _publicApiClient;

    public Given_A_Working_Service(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
    }

    [Fact]
    public async ValueTask Then_we_get_a_successful_response()
    {
        var response = await _publicApiClient.GetHoofdactiviteiten();
        response.Should().BeSuccessful();
    }

    [Fact]
    public async ValueTask Then_It_Returns_All_Possible_Values()
    {
        var response = await _publicApiClient.GetHoofdactiviteiten();
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Working_Service)}_{nameof(Then_It_Returns_All_Possible_Values)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
