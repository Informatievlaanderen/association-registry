namespace AssociationRegistry.Test.Admin.Api.Migrate_To_Projections.V1.When_Searching;

using AssociationRegistry.Formats;
using AssociationRegistry.JsonLdContext;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Extensions;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_LocatieWerdVerwijderd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V025_LocatieWerdVerwijderd _scenario;

    public Given_LocatieWerdVerwijderd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V025LocatieWerdVerwijderd;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _adminApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_without_the_first_Locatie()
    {
        var response = await _adminApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v =>
                               {
                                   var l2 = _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[1];
                                   var l3 = _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[2];

                                   v.WithVCode(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                                    .WithJsonLdType(JsonLdType.FeitelijkeVereniging)
                                    .WithType(Verenigingstype.FeitelijkeVereniging)
                                    .WithNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam)
                                    .WithKorteNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.KorteNaam)
                                    .WithStartdatum(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Startdatum)
                                    .WithDoelgroep(_scenario.VCode)
                                    .WithLocatie(l2.Locatietype, l2.Naam, l2.Adres.ToAdresString(), l2.Adres?.Postcode, l2.Adres?.Gemeente,
                                                 _scenario.VCode,
                                                 l2.LocatieId,
                                                 l2.IsPrimair)
                                    .WithLocatie(l3.Locatietype, l3.Naam, l3.Adres.ToAdresString(), l3.Adres?.Postcode, l3.Adres?.Gemeente,
                                                 _scenario.VCode,
                                                 l3.LocatieId,
                                                 l3.IsPrimair)
                                    .WithHoofdactiviteit(
                                         _scenario.FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket[0].Code,
                                         _scenario.FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket[0].Naam);

                                   return v;
                               });

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
