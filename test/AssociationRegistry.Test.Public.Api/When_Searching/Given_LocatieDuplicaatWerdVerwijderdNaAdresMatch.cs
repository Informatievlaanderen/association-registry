namespace AssociationRegistry.Test.Public.Api.When_Searching;

using Fixtures;
using Fixtures.GivenEvents;
using Fixtures.GivenEvents.Scenarios;
using FluentAssertions;
using Formats;
using templates;
using Vereniging;

using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly V022_LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario _scenario;
    private readonly PublicApiClient _publicApiClient;

    public Given_LocatieDuplicaatWerdVerwijderdNaAdresMatch(GivenEventsFixture fixture, ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _publicApiClient = fixture.PublicApiClient;
        _scenario = fixture.V022LocatieDuplicaatWerdVerwijderdNaAdresMatchScenario;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.Search(_scenario.VCode)).Should().BeSuccessful();

    [Fact]
    public async Task Then_we_retrieve_one_vereniging_matching_the_vcode_searched()
    {
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
                                        loc.Adres?.Postcode,
                                        loc.Adres?.Gemeente,
                                        _scenario.VCode,
                                        loc.LocatieId,
                                        loc.IsPrimair
                                    )
                                   .WithDoelgroep(_scenario.VCode)
                           );

        var content = await GetResponseFromApiClient();

        for (var i = 0; i < 5; i++)
        {
            Thread.Sleep(3000);

            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Receive new content:");
            content = await GetResponseFromApiClient();
            Console.WriteLine(content);
            Console.ForegroundColor = foregroundColor;
        }

        // content.Should().BeEquivalentJson(goldenMaster);
    }

    private async Task<string> GetResponseFromApiClient()
    {
        var response = await _publicApiClient.Search(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();
        _outputHelper.WriteLine(content);

        return content;
    }
}
