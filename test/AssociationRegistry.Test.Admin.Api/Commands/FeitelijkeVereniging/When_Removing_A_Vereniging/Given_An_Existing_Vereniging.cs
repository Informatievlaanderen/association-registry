namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Removing_A_Vereniging;

using Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
using Framework.Categories;
using Framework.Fixtures;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Remove_An_Existing_Vereniging : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V058_FeitelijkeVerenigingWerdGeregistreerd_ForRemoval Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Remove_An_Existing_Vereniging(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V058FeitelijkeVerenigingWerdGeregistreerdForRemoval;
        DocumentStore = _fixture.DocumentStore;
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.SuperAdminApiClient.DeleteVereniging(Scenario.VCode, reason: "Omdat");
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTestToRefactor]
public class Given_An_Existing_Vereniging : IClassFixture<Remove_An_Existing_Vereniging>
{
    private readonly Remove_An_Existing_Vereniging _classFixture;

    public Given_An_Existing_Vereniging(Remove_An_Existing_Vereniging classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var verenigingWerdVerwijderd = (await session.Events
                                                     .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(VerenigingWerdVerwijderd));

        verenigingWerdVerwijderd.Data.Should()
                                .BeEquivalentTo(new VerenigingWerdVerwijderd(_classFixture.Scenario.VCode, Reden: "Omdat"));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
