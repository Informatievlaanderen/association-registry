namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Removing_A_Vereniging;

using Events;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Remove_An_Existing_Vereniging_With_Insufficient_Rights : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Remove_An_Existing_Vereniging_With_Insufficient_Rights(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields;
        DocumentStore = _fixture.DocumentStore;
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.DeleteVereniging(Scenario.VCode, reason: "Omdat");
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_An_Client_With_Insufficient_Rights : IClassFixture<Remove_An_Existing_Vereniging_With_Insufficient_Rights>
{
    private readonly Remove_An_Existing_Vereniging_With_Insufficient_Rights _classFixture;

    public Given_An_Client_With_Insufficient_Rights(Remove_An_Existing_Vereniging_With_Insufficient_Rights classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_no_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var verenigingWerdVerwijderd = (await session.Events
                                                     .FetchStreamAsync(_classFixture.Scenario.VCode))
           .SingleOrDefault(e => e.Data.GetType() == typeof(VerenigingWerdVerwijderd));

        verenigingWerdVerwijderd.Should().BeNull();
    }

    [Fact]
    public void Then_it_returns_an_forbidden_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
