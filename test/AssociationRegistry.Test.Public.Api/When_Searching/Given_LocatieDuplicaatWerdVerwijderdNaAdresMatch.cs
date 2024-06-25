namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formatters;
using Framework;
using templates;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch
{
    private readonly V022_LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V022LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task? Then_we_retrieve_one_vereniging_matching_the_vcode_searched()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var loc = V022_LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario.TeBehoudenLocatie;

        var goldenMaster = new ZoekVerenigingenResponseTemplate()
                          .FromQuery(_scenario.VCode)
                          .WithVereniging(
                               v => v
                                   .WithVCode(_scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode)
                                   .WithType(Verenigingstype.FeitelijkeVereniging)
                                   .WithNaam(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam)
                                   .WithLocatie(
                                        loc.Locatietype,
                                        loc.Naam,
                                        loc.Adres.ToAdresString(),
                                        loc.Adres.Postcode,
                                        loc.Adres.Gemeente,
                                        _scenario.VCode,
                                        loc.LocatieId,
                                        loc.IsPrimair
                                    )
                                   .WithDoelgroep(_scenario.VCode)
                           );

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
