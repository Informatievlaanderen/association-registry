namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Newtonsoft.Json;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Categories;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_A_Gestopte_Vereniging
{
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V056_VerenigingWerdGeregistreerd_And_Gestopt_For_DuplicateDetection _scenario;

    public Given_A_Gestopte_Vereniging(EventsInDbScenariosFixture fixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = fixture.AdminApiClient;
        _scenario = fixture.V056VerenigingWerdGeregistreerdAndGestoptForDuplicateDetection;
    }

    [Fact]
    public async Task? Then_no_duplicate_is_returned()
    {
        var request = CreateRegistreerFeitelijkeVerenigingRequest(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam,
                                                                  _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().Adres
                                                                           .Gemeente,
                                                                  _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().Adres
                                                                           .Postcode);

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        var content = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            var duplicateResponse = JsonConvert.DeserializeObject<PotentialDuplicatesResponse>(content);

            duplicateResponse.MogelijkeDuplicateVerenigingen.Should()
                             .NotContain(x => x.VCode == _scenario.FeitelijkeVerenigingWerdGeregistreerd.VCode);
        }
        // all good
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
