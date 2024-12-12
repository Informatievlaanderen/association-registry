namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Vereniging_Gemarkeerd_Als_Dubbel
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V080_VerenigingWerdGeregistreerd_And_Gemarkeerd_Als_Dubbel_For_DuplicateDetection _scenario;

    public Given_A_Vereniging_Gemarkeerd_Als_Dubbel(EventsInDbScenariosFixture fixture, ITestOutputHelper testOutputHelper)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = fixture.AdminApiClient;
        _scenario = fixture.V080VerenigingWerdGeregistreerdAndGemarkeerdAlsDubbelForDuplicateDetection;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task? Then_no_duplicate_is_returned_for_dubbele_vereniging()
    {
        var request = CreateRegistreerFeitelijkeVerenigingRequest(_scenario.DubbeleVerenigingWerdGeregistreerd.Naam,
                                                                  _scenario.DubbeleVerenigingWerdGeregistreerd.Locaties.First().Adres
                                                                           .Gemeente,
                                                                  _scenario.DubbeleVerenigingWerdGeregistreerd.Locaties.First().Adres
                                                                           .Postcode);

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    private static IEnumerable<string> ExtractDuplicateVCode(string responseContent)
    {
        var duplicates = JObject.Parse(responseContent)
                                .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].vCode")
                                .Select(x => x.ToString());

        return duplicates;
    }

    private RegistreerFeitelijkeVerenigingRequest CreateRegistreerFeitelijkeVerenigingRequest(string naam, string gemeente, string postcode)
    {
        return new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = naam,
            Startdatum = null,
            KorteNaam = "",
            KorteBeschrijving = "",
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Locatietype = Locatietype.Correspondentie,
                    Adres = new Adres
                    {
                        Straatnaam = _fixture.Create<string>(),
                        Huisnummer = _fixture.Create<string>(),
                        Postcode = postcode,
                        Gemeente = gemeente,
                        Land = _fixture.Create<string>(),
                    },
                },
            },
        };
    }
}
