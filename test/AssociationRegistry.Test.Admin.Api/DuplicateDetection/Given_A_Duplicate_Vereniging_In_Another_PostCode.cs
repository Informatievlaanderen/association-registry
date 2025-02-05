namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
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
public class Given_A_Duplicate_Vereniging_In_Another_PostCode
{
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V047_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForDuplicateDetection_WithAnalyzer _scenario;

    public Given_A_Duplicate_Vereniging_In_Another_PostCode(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = fixture.AdminApiClient;
        _scenario = fixture.V047FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForDuplicateDetectionWithAnalyzer;
    }

    [Fact]
    public async Task Then_It_Does_Not_Return_The_Vereniging_As_Duplicate()
    {
        var request = CreateRegistreerFeitelijkeVerenigingRequest("V9999048", _fixture.Create<string>(), _fixture.Create<string>());

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseContent = await response.Content.ReadAsStringAsync();
        var duplicates = ExtractDuplicateVCode(responseContent);
        duplicates.Should().NotContain("V9999048");
    }

    private RegistreerFeitelijkeVerenigingRequest CreateRegistreerFeitelijkeVerenigingRequest(
        string vCode,
        string gemeeente,
        string postcode)
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
                        Postcode = postcode,
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
