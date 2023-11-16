﻿namespace AssociationRegistry.Test.Admin.Api.WhenDetectingDuplicates;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
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
public class Given_Some_Updates
{
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V047_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForDuplicateDetection_WithAnalyzer _scenario;

    public Given_Some_Updates(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = fixture.AdminApiClient;
        _scenario = fixture.V047FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForDuplicateDetectionWithAnalyzer;
    }

    [Theory]
    [InlineData("V9999047", "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling")]
    public async Task? Then_A_DuplicateIsDetected_WithDifferentCapitalization(
        string duplicatesShouldContainThisVCode,
        string naam)
    {
        var request = CreateRegistreerFeitelijkeVerenigingRequest(duplicatesShouldContainThisVCode, naam);

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseContent = await response.Content.ReadAsStringAsync();

        var duplicate = JObject.Parse(responseContent)["mogelijkeDuplicateVerenigingen"]
                               .Single(x => x["vCode"].Value<string>() == duplicatesShouldContainThisVCode);

        duplicate["korteNaam"].Value<string>().Should().Be("Korte Naam Test");
    }

    private RegistreerFeitelijkeVerenigingRequest CreateRegistreerFeitelijkeVerenigingRequest(string vCode, string naam)
    {
        var @event = _scenario.EventsPerVCode
                              .Single(t => t.Item1.Value == vCode)
                              .Item2
                              .First() as FeitelijkeVerenigingWerdGeregistreerd;

        return new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = naam,
            Startdatum = DateOnly.FromDateTime(DateTime.Now),
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
    }

    private static IEnumerable<string> ExtractDuplicateVCode(string responseContent)
        => JObject.Parse(responseContent)
                  .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].vCode")
                  .Select(x => x.ToString());

    private static IEnumerable<string> ExtractDuplicateKorteNaam(string responseContent)
        => JObject.Parse(responseContent)
                  .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].vCode")
                  .Select(x => x.ToString());

    private static IEnumerable<string> ExtractDuplicateNaam(string responseContent)
        => JObject.Parse(responseContent)
                  .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].vCode")
                  .Select(x => x.ToString());

    private static IEnumerable<string> ExtractDuplicateGemeentes(string responseContent)
        => JObject.Parse(responseContent)
                  .SelectTokens("$.mogelijkeDuplicateVerenigingen[*].vCode")
                  .Select(x => x.ToString());
}
