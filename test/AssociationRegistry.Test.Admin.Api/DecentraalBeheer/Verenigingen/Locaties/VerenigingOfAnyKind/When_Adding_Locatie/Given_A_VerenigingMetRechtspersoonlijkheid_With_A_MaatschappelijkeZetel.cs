namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Locaties.VerenigingOfAnyKind.When_Adding_Locatie;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using System.Net;
using Xunit;

public class Given_A_VerenigingMetRechtspersoonlijkheid_With_A_MaatschappelijkeZetel_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public V034_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForAddingLocatie Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Given_A_VerenigingMetRechtspersoonlijkheid_With_A_MaatschappelijkeZetel_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V034VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMaatschappelijkeZetelForAddingLocatie;
        DocumentStore = _fixture.DocumentStore;

        _jsonBody = @"{
            ""locatie"": {
                ""locatietype"": ""Correspondentie"",
                ""isPrimair"": true,
                ""naam"": ""nieuwe locatie"",
                ""adres"": {
                    ""straatnaam"": ""Stationsstraat"",
                    ""huisnummer"": ""1"",
                    ""busnummer"": ""B"",
                    ""postcode"": ""1790"",
                    ""gemeente"": ""Affligem"",
                    ""land"": ""België"",
                }
            }
        }";
    }

    public async ValueTask InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PostLocatie(Scenario.VCode, _jsonBody);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

[Collection(nameof(AdminApiCollection))]
public class Given_A_VerenigingMetRechtspersoonlijkheid_With_A_MaatschappelijkeZetel : IClassFixture<
    Given_A_VerenigingMetRechtspersoonlijkheid_With_A_MaatschappelijkeZetel_Setup>
{
    private readonly Given_A_VerenigingMetRechtspersoonlijkheid_With_A_MaatschappelijkeZetel_Setup _classFixture;

    public Given_A_VerenigingMetRechtspersoonlijkheid_With_A_MaatschappelijkeZetel(
        Given_A_VerenigingMetRechtspersoonlijkheid_With_A_MaatschappelijkeZetel_Setup classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var locatieWerdToegevoegd = (await session.Events
                                                  .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(LocatieWerdToegevoegd));

        locatieWerdToegevoegd.Data.Should()
                             .BeEquivalentTo(
                                  new LocatieWerdToegevoegd(
                                      new Registratiedata.Locatie(
                                          LocatieId: 2,
                                          Locatietype: "Correspondentie",
                                          IsPrimair: true,
                                          Naam: "nieuwe locatie",
                                          new Registratiedata.Adres(
                                              Straatnaam: "Stationsstraat",
                                              Huisnummer: "1",
                                              Busnummer: "B",
                                              Postcode: "1790",
                                              Gemeente: "Affligem",
                                              Land: "België"),
                                          AdresId: null)));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
