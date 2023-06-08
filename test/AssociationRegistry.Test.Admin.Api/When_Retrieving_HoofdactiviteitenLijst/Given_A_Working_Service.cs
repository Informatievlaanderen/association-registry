namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_HoofdactiviteitenLijst;

using Fixtures;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Working_Service
{
    private readonly AdminApiClient _adminApiClient;

    public Given_A_Working_Service(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
    {
        var response = await _adminApiClient.GetHoofdactiviteiten();
        response.Should().BeSuccessful();
    }

    [Fact]
    public async Task Then_It_Returns_All_Possible_Values()
    {
        var response = await _adminApiClient.GetHoofdactiviteiten();
        var content = await response.Content.ReadAsStringAsync();
        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"{nameof(Given_A_Working_Service)}_{nameof(Then_It_Returns_All_Possible_Values)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
