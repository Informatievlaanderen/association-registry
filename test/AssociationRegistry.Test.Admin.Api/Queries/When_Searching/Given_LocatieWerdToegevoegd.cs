namespace AssociationRegistry.Test.Admin.Api.Queries.When_Searching;

using AssociationRegistry.Formats;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Framework.templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_LocatieWerdToegevoegd
{
    private readonly V023_LocatieWerdToegevoegd _scenario;
    private readonly AdminApiClient _adminApiClient;

    public Given_LocatieWerdToegevoegd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V023LocatieWerdToegevoegd;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_with_the_added_Locatie()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                                   .WithLocatie(_scenario.LocatieWerdToegevoegd.Locatie.Locatietype,
                                                _scenario.LocatieWerdToegevoegd.Locatie.Naam,
                                                _scenario.LocatieWerdToegevoegd.Locatie.Adres.ToAdresString(),
                                                _scenario.LocatieWerdToegevoegd.Locatie.Adres?.Postcode,
                                                _scenario.LocatieWerdToegevoegd.Locatie.Adres?.Gemeente,
                                                _scenario.VCode,
                                                _scenario.LocatieWerdToegevoegd.Locatie.LocatieId,
                                                _scenario.LocatieWerdToegevoegd.Locatie.IsPrimair
                                    )
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
