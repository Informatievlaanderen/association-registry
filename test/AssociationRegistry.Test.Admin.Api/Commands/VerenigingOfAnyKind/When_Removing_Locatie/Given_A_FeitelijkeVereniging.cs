namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Removing_Locatie;

using Events;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Delete_An_Existing_Locatie_Given_A_FeitelijkeVereniging : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V024_FeitelijkeVerenigingWerdGeregistreerd_WithLocatie_ForRemovingLocatie Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Delete_An_Existing_Locatie_Given_A_FeitelijkeVereniging(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V024FeitelijkeVerenigingWerdGeregistreerdWithLocatieForRemovingLocatie;
        DocumentStore = _fixture.DocumentStore;
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.DeleteLocatie(Scenario.VCode,
                                                               Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0].LocatieId,
                                                               jsonBody: @"{""initiator"":""OVO000001""}");
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_FeitelijkeVereniging : IClassFixture<Delete_An_Existing_Locatie_Given_A_FeitelijkeVereniging>
{
    private readonly Delete_An_Existing_Locatie_Given_A_FeitelijkeVereniging _classFixture;

    public Given_A_FeitelijkeVereniging(Delete_An_Existing_Locatie_Given_A_FeitelijkeVereniging classFixture)
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

        var locatie = _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties[0];

        locatieWerdVerwijderd.Data.Should()
                             .BeEquivalentTo(
                                  new LocatieWerdVerwijderd(_classFixture.Scenario.VCode, locatie));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
