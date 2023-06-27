namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Locatie;

using System.Net;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using Xunit;
using Xunit.Categories;

public class Post_A_New_Locatie : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public V022_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForAddingLocatie Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Post_A_New_Locatie(EventsInDbScenariosFixture fixture)
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
public class Given_A_New_Locatie : IClassFixture<Post_A_New_Locatie>
{
    private readonly Post_A_New_Locatie _classFixture;

    public Given_A_New_Locatie(Post_A_New_Locatie classFixture)
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
                        1,
                        "Correspondentie",
                        true,
                        "nieuwe locatie",
                        new Registratiedata.Adres(
                            "Stationsstraat",
                            "1",
                            "B",
                            "1790",
                            "Affligem",
                            "België"),
                        new Registratiedata.AdresId("AR", "https://data.vlaanderen.be/id/adres/0"))));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
