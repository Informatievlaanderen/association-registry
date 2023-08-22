namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Removing_Locatie;

using System.Net;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using Xunit;
using Xunit.Categories;

public class Delete_An_Existing_Locatie : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V033_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForVerwijderen Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Delete_An_Existing_Locatie(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V033VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithLocatiesForVerwijderen;
        DocumentStore = _fixture.DocumentStore;
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.DeleteLocatie(Scenario.VCode, Scenario.LocatieWerdToegevoegd.Locatie.LocatieId, @"{""initiator"":""OVO000001""}");
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_Vereniging_Met_Rechtspersoonlijkheid : IClassFixture<Delete_An_Existing_Locatie>
{
    private readonly Delete_An_Existing_Locatie _classFixture;

    public Given_A_Vereniging_Met_Rechtspersoonlijkheid(Delete_An_Existing_Locatie classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();
        var locatieWerdVerwijderd = (await session.Events
                .FetchStreamAsync(_classFixture.Scenario.VCode))
            .Single(e => e.Data.GetType() == typeof(LocatieWerdVerwijderd));

        var locatie = _classFixture.Scenario.LocatieWerdToegevoegd.Locatie;
        locatieWerdVerwijderd.Data.Should()
            .BeEquivalentTo(
                new LocatieWerdVerwijderd(locatie));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
