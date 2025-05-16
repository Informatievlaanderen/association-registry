namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Removing_Contactgegeven;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Delete_An_Existing_Contactgegeven_Given_A_FeitelijkeVereniging : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V007_FeitelijkeVerenigingWerdGeregistreerd_WithContactgegeven Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Delete_An_Existing_Contactgegeven_Given_A_FeitelijkeVereniging(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V007FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven;
        DocumentStore = _fixture.DocumentStore;
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.DeleteContactgegeven(Scenario.VCode,
                                                                      Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens[0]
                                                                              .ContactgegevenId);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_FeitelijkeVereniging : IClassFixture<Delete_An_Existing_Contactgegeven_Given_A_FeitelijkeVereniging>
{
    private readonly Delete_An_Existing_Contactgegeven_Given_A_FeitelijkeVereniging _classFixture;

    public Given_A_FeitelijkeVereniging(Delete_An_Existing_Contactgegeven_Given_A_FeitelijkeVereniging classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var contactgegevenWerdVerwijderd = (await session.Events
                                                         .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(ContactgegevenWerdVerwijderd));

        contactgegevenWerdVerwijderd.Data.Should()
                                    .BeEquivalentTo(
                                         new ContactgegevenWerdVerwijderd(
                                             _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens[0]
                                                          .ContactgegevenId,
                                             _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens[0]
                                                          .Contactgegeventype,
                                             _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens[0]
                                                          .Waarde,
                                             _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens[0]
                                                          .Beschrijving,
                                             _classFixture.Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens[0]
                                                          .IsPrimair));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
