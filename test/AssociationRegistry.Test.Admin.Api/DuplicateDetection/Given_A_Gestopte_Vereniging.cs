namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Framework;
using Newtonsoft.Json;
using System.Net;
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
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
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
