namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using System.Net;
using Xunit;

public class Delete_An_Existing_Vertegenwoordiger : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V011_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForRemovingVertegenwoordiger Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Delete_An_Existing_Vertegenwoordiger(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V011FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForRemovingVertegenwoordiger;
        DocumentStore = _fixture.DocumentStore;
    }

    public async ValueTask InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.DeleteVertegenwoordiger(Scenario.VCode,
                                                                         Scenario.FeitelijkeVerenigingWerdGeregistreerd
                                                                                 .Vertegenwoordigers[0].VertegenwoordigerId,
                                                                         jsonBody: @"{""initiator"":""OVO000001""}");
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

[Collection(nameof(AdminApiCollection))]
public class Given_An_Existing_Vertegenwoordiger : IClassFixture<Delete_An_Existing_Vertegenwoordiger>
{
    private readonly Delete_An_Existing_Vertegenwoordiger _classFixture;

    public Given_An_Existing_Vertegenwoordiger(Delete_An_Existing_Vertegenwoordiger classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var vertegenwoordigerWerdVerwijderd = (await session.Events
                                                            .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(VertegenwoordigerWerdVerwijderd));

        var vertegenwoordiger = _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0];

        vertegenwoordigerWerdVerwijderd.Data.Should()
                                       .BeEquivalentTo(
                                            new VertegenwoordigerWerdVerwijderd(
                                                vertegenwoordiger.VertegenwoordigerId,
                                                vertegenwoordiger.Insz,
                                                vertegenwoordiger.Voornaam,
                                                vertegenwoordiger.Achternaam));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
