namespace AssociationRegistry.Test.Admin.Api.When_Searching;

using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_MaatschappelijkeZetelWerdovergenomenUitKbo
{
    private readonly V029_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_All_Data _scenario;
    private readonly string _goldenMaster;
    private readonly AdminApiClient _adminApiClient;

    public Given_MaatschappelijkeZetelWerdovergenomenUitKbo(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V029VerenigingeMetRechtspersoonlijkheidWerdGeregistreerdWithAllData;
        _adminApiClient = fixture.AdminApiClient;

        _goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_MaatschappelijkeZetelWerdovergenomenUitKbo)}_{nameof(Then_we_retrieve_one_vereniging_with_A_Maatschappelijke_Zetel)}");
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_A_Maatschappelijke_Zetel()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = _goldenMaster
           .Replace(oldValue: "{{originalQuery}}", _scenario.VCode);

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
