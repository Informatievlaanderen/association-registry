﻿namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Adding_Locatie;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Fixtures;
using AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Given_A_FeitelijkeVereniging_Setup : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public V022_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Given_A_FeitelijkeVereniging_Setup(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V022FeitelijkeVerenigingWerdGeregistreerdWithMinimalFieldsForAddingLocatie;
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
                },
                ""adresId"": {
                    ""broncode"": ""AR"",
                    ""bronwaarde"": ""https://data.vlaanderen.be/id/adres/0"",
                }
            }
        }";
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PostLocatie(Scenario.VCode, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_FeitelijkeVereniging : IClassFixture<Given_A_FeitelijkeVereniging_Setup>
{
    private readonly Given_A_FeitelijkeVereniging_Setup _classFixture;

    public Given_A_FeitelijkeVereniging(Given_A_FeitelijkeVereniging_Setup classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var contactgegevenWerdToegevoegd = (await session.Events
                                                         .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(LocatieWerdToegevoegd));

        contactgegevenWerdToegevoegd.Data.Should()
                                    .BeEquivalentTo(
                                         new LocatieWerdToegevoegd(
                                             new Registratiedata.Locatie(
                                                 LocatieId: 1,
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
                                                 new Registratiedata.AdresId(Broncode: "AR",
                                                                             Bronwaarde: "https://data.vlaanderen.be/id/adres/0"))));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
