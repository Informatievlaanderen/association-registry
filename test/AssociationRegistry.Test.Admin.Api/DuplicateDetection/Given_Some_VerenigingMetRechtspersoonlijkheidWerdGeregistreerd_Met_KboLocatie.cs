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
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_Some_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_Met_KboLocatie
{
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V057_VerenigingWerdGeregistreerd_With_KboLocatie_For_DuplicateDetection _scenario;

    public Given_Some_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_Met_KboLocatie(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = fixture.AdminApiClient;
        _scenario = fixture.V057VerenigingWerdGeregistreerdWithKboLocatieForDuplicateDetection;
    }

    [Fact]
    public async Task Then_A_DuplicateIsDetected()
    {
        var request = CreateRegistreerFeitelijkeVerenigingRequest(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Naam,
                                                                  _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres
                                                                           .Gemeente,
                                                                  _scenario.MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.Adres
                                                                           .Postcode);

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseContent = await response.Content.ReadAsStringAsync();

        var duplicate = JObject.Parse(responseContent)["mogelijkeDuplicateVerenigingen"]
                               .Single(x => x["vCode"].Value<string>() == _scenario.VCode);
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
