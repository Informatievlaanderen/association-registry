namespace AssociationRegistry.Test.Admin.Api.DuplicateDetection;

using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Humanizer;
using Marten;
using Marten.Events;
using Nest;
using Newtonsoft.Json;
using Vereniging;
using Xunit;
using Adres = AssociationRegistry.Admin.Api.Verenigingen.Common.Adres;

[Collection(nameof(AdminApiCollection))]
public class Given_A_Gestopte_Vereniging
{
    private readonly AdminApiClient _adminApiClient;
    private readonly Fixture _fixture;
    private readonly V056_VerenigingWerdGeregistreerd_And_Gestopt_For_DuplicateDetection _scenario;
    private IDocumentStore _store;
    private IElasticClient _elasticClient;

    public Given_A_Gestopte_Vereniging(EventsInDbScenariosFixture scenarioFixture)
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _adminApiClient = scenarioFixture.AdminApiClient;
        _elasticClient = scenarioFixture.ElasticClient;
        _store = scenarioFixture.ApiDocumentStore;
        _scenario = scenarioFixture.V056VerenigingWerdGeregistreerdAndGestoptForDuplicateDetection;
    }

    [Fact]
    public async ValueTask Then_no_duplicate_is_returned()
    {
        var request = CreateRegistreerFeitelijkeVerenigingRequest(_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam,
                                                                  _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().Adres
                                                                           .Gemeente,
                                                                  _scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().Adres
                                                                           .Postcode);

        var response = await _adminApiClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(request));
        var content = await response.Content.ReadAsStringAsync();

        await _store.WaitForNonStaleProjectionDataAsync(10.Seconds());
        await _elasticClient.Indices.RefreshAsync(Indices.AllIndices);

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
