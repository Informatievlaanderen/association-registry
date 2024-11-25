namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Some_FeitelijkeVerenigingenGeregistreerd_Met_Naam
{
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V047_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForDuplicateDetection_WithAnalyzer _scenario;

    public Given_Some_FeitelijkeVerenigingenGeregistreerd_Met_Naam(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = fixture.AdminApiClient;
        _scenario = fixture.V047FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForDuplicateDetectionWithAnalyzer;
    }

    [Theory]
    [InlineData("V9999048", "Grote vereniging")]
    [InlineData("V9999048", "GROTE VERENIGING")]
    [InlineData("V9999048", "grote vereniging")]
    [InlineData("V9999051", "De Pottestampers")]
    [InlineData("V9999051", "DE POTTESTAMPERS")]
    [InlineData("V9999051", "de pottestampers")]
    public async Task? Then_A_DuplicateIsDetected_WithDifferentCapitalization(
        string duplicatesShouldContainThisVCode,
        string verbasterdeNaam)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeNaam);
    }

    [Theory]
    [InlineData("V9999048", "Grote-Vereniging")]
    [InlineData("V9999050", "Sint Servaas")]
    [InlineData("V9999048", "Grote Vereniging!")]
    [InlineData("V9999050", "Sint-Servaas!")]
    [InlineData("V9999048", "Grote-Vereniging!")]
    [InlineData("V9999050", "Sint Servaas!")]
    public async Task Then_A_DuplicateIsDetected_WithDifferentPunctuation(string duplicatesShouldContainThisVCode, string verbasterdeNaam)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeNaam);
    }

    [Theory]
    [InlineData("V9999048", "   Grote Vereniging")]
    [InlineData("V9999048", "Grote Vereniging   ")]
    [InlineData("V9999048", "Grote    Vereniging")]
    [InlineData("V9999048", " Grote  Vereniging ")]
    public async Task? Then_A_DuplicateIsDetected_WithAdditionalSpaces(string duplicatesShouldContainThisVCode, string verbasterdeNaam)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeNaam);
    }

    [Theory]
    [InlineData("V9999047", "Vereniging van Technologieenthousiasten: Innovacie & Ontwikkeling")]
    [InlineData("V9999049", "Cafesport")]
    public async Task? Then_A_DuplicateIsDetected_WithNoAccents(string duplicatesShouldContainThisVCode, string verbasterdeNaam)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeNaam);
    }

    [Theory]
    [InlineData("V9999048", "Grote Veréniging")]
    [InlineData("V9999051", "Dé pottestampers")]
    public async Task? Then_A_DuplicateIsDetected_WithMoreAccents(string duplicatesShouldContainThisVCode, string verbasterdeNaam)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeNaam);
    }

    [Theory]
    [InlineData("V9999048", "De Grote van de Vereniging")]
    [InlineData("V9999051", "pottestampers met het")]
    public async Task? Then_A_DuplicateIsDetected_WithStopwoorden(string duplicatesShouldContainThisVCode, string verbasterdeNaam)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeNaam);
    }

    [Theory]
    [InlineData("V9999048", "Gorte Veregigning")]
    [InlineData("V9999048", "Gorte Vereeegigning")]
    [InlineData("V9999051", "De potenstampers")]
    public async Task? Then_A_DuplicateIsDetected_WithFoezieSearch(string duplicatesShouldContainThisVCode, string verbasterdeNaam)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeNaam);
    }

    private async Task VerifyThatDuplicateIsFound(string duplicatesShouldContainThisVCode, string verbasterdeNaam)
    {
        var request = FeitelijkeVerenigingWerdGeregistreerd(duplicatesShouldContainThisVCode, verbasterdeNaam);

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseContent = await response.Content.ReadAsStringAsync();
        var duplicates = ExtractDuplicateVCode(responseContent);
        duplicates.Should().Contain(duplicatesShouldContainThisVCode);
    }

    private RegistreerFeitelijkeVerenigingRequest FeitelijkeVerenigingWerdGeregistreerd(
        string vCode,
        string naam)
    {
        var @event = _scenario.EventsPerVCode
                              .Single(t => t.Item1.Value == vCode)
                              .Item2
                              .First() as FeitelijkeVerenigingWerdGeregistreerd;

        var request1 = new RegistreerFeitelijkeVerenigingRequest
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
                        Postcode = @event.Locaties.First().Adres.Postcode,
                        Gemeente = @event.Locaties.First().Adres.Gemeente,
                        Land = _fixture.Create<string>(),
                    },
                },
            },
        };

        var request = request1;

        return request;
    }

    private static IEnumerable<string> ExtractDuplicateVCode(string responseContent)
    {
        var duplicates = JObject.Parse(responseContent)
                                .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].vCode")
                                .Select(x => x.ToString());

        return duplicates;
    }
}
