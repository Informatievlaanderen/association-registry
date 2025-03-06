namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Some_FeitelijkeVerenigingenGeregistreerd_Met_Gemeente
{
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V047_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForDuplicateDetection_WithAnalyzer _scenario;

    public Given_Some_FeitelijkeVerenigingenGeregistreerd_Met_Gemeente(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = fixture.AdminApiClient;
        _scenario = fixture.V047FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForDuplicateDetectionWithAnalyzer;
    }

    [Theory]
    [InlineData("V9999047", "NEDER-OVER-OPPER-ONDER-HEEMBEEK")]
    [InlineData("V9999047", "NeDeR-OvEr-oPpEr-OnDeR-HeEmBeEk")]
    public async Task? Then_A_DuplicateIsDetected_WithDifferentCapitalization(
        string duplicatesShouldContainThisVCode,
        string verbasterdeGemeente)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeGemeente);
    }

    [Theory]
    [InlineData("V9999047", "Neder over opper onder heembeek")]
    [InlineData("V9999047", "Neder over opper-onder-heembeek")]
    [InlineData("V9999047", "Neder over opper. onder heembeek")]
    [InlineData("V9999047", "Neder over opper-onder. heembeek")]
    public async Task Then_A_DuplicateIsDetected_WithDifferentPunctuation(
        string duplicatesShouldContainThisVCode,
        string verbasterdeGemeente)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeGemeente);
    }

    [Theory]
    [InlineData("V9999047", "Neders-met-opper-onder-heembeek")]
    [InlineData("V9999047", "Neder-over-van-onder-heembeek")]
    [InlineData("V9999048", "Neder-met-opper-en-het-onder-heembeek")]
    public async Task? Then_A_DuplicateIsDetected_WithStopwoorden(string duplicatesShouldContainThisVCode, string verbasterdeGemeente)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeGemeente);
    }

    [Theory]
    [InlineData("V9999047", "Neders-over-opper-onder-heembeek")]
    [InlineData("V9999047", "Neder-over-oper-onder-heembeek")]
    [InlineData("V9999048", "Neder-over-opper-onder-humbeek")]
    public async Task? Then_A_DuplicateIsDetected_WithFoezieSearch(
        string duplicatesShouldContainThisVCode,
        string verbasterdeGemeente)
    {
        await VerifyThatDuplicateIsFound(duplicatesShouldContainThisVCode, verbasterdeGemeente);
    }

    private async Task VerifyThatDuplicateIsFound(string duplicatesShouldContainThisVCode, string verbasterdeGemeente)
    {
        var request = CreateRegistreerFeitelijkeVerenigingRequest(duplicatesShouldContainThisVCode, verbasterdeGemeente);

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseContent = await response.Content.ReadAsStringAsync();
        var duplicates = ExtractDuplicateVCode(responseContent);
        duplicates.Should().Contain(duplicatesShouldContainThisVCode);
    }

    private RegistreerFeitelijkeVerenigingRequest CreateRegistreerFeitelijkeVerenigingRequest(
        string vCode,
        string gemeeente)
    {
        var @event = _scenario.EventsPerVCode
                              .Single(t => t.Item1.Value == vCode)
                              .Item2
                              .First() as FeitelijkeVerenigingWerdGeregistreerd;

        return new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = @event.Naam,
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
                        Gemeente = gemeeente,
                        Land = _fixture.Create<string>(),
                    },
                },
            },
        };
    }

    private static IEnumerable<string> ExtractDuplicateVCode(string responseContent)
    {
        var duplicates = JObject.Parse(responseContent)
                                .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].vCode")
                                .Select(x => x.ToString());

        return duplicates;
    }
}
