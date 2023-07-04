namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie;

using System.Net;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using Framework;
using AutoFixture;
using FluentAssertions;
using Marten;
using Xunit;
using Xunit.Categories;

public class Patch_A_Locatie: IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public string WaardeVolgensType { get; }
    public Registratiedata.Locatie AanTePassenContactGegeven { get; }
    public V026_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Patch_A_Locatie(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();

        _fixture = fixture;

        Scenario = fixture.V026FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigen;
        DocumentStore = _fixture.DocumentStore;

        var contactgegeven = Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First();
        WaardeVolgensType = autoFixture.CreateContactgegevenVolgensType(contactgegeven.Type).Waarde;
        _jsonBody = $@"{{
            ""contactgegeven"":
                {{
                    ""waarde"": ""{WaardeVolgensType}"",
                    ""beschrijving"": ""algemeen"",
                    ""isPrimair"": false
                }},
            ""initiator"": ""OVO000001""
        }}";
        AanTePassenContactGegeven = contactgegeven;
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PatchLocaties(Scenario.VCode, AanTePassenContactGegeven.LocatieId, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_An_Existing_Locatie : IClassFixture<Patch_A_Locatie>
{
    private readonly Patch_A_Locatie _classFixture;

    public Given_An_Existing_Locatie(Patch_A_Locatie classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();
        var contactgegevenWerdAangepast = (await session.Events
                .FetchStreamAsync(_classFixture.Scenario.VCode))
            .Single(e => e.Data.GetType() == typeof(LocatieWerdGewijzigd));

        contactgegevenWerdAangepast.Data.Should()
            .BeEquivalentTo(new LocatieWerdGewijzigd(_classFixture.AanTePassenContactGegeven.LocatieId, _classFixture.AanTePassenContactGegeven.Type, _classFixture.WaardeVolgensType, "algemeen", false));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
