namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Removing_Contactgegeven;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Delete_An_Existing_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V037_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForVerwijderContactgegeven Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Delete_An_Existing_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V037VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForVerwijderContactgegeven;
        DocumentStore = _fixture.DocumentStore;
    }

    public async ValueTask InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.DeleteContactgegeven(Scenario.VCode,
                                                                      Scenario.ContactgegevenWerdToegevoegd.ContactgegevenId);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_VerenigingMetRechtspersoonlijkheid : IClassFixture<
    Delete_An_Existing_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid>
{
    private readonly Delete_An_Existing_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid _classFixture;

    public Given_A_VerenigingMetRechtspersoonlijkheid(
        Delete_An_Existing_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var contactgegevenWerdVerwijderd = (await session.Events
                                                         .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(ContactgegevenWerdVerwijderd));

        contactgegevenWerdVerwijderd.Data.Should()
                                    .BeEquivalentTo(
                                         new ContactgegevenWerdVerwijderd(
                                             _classFixture.Scenario.ContactgegevenWerdToegevoegd
                                                          .ContactgegevenId,
                                             _classFixture.Scenario.ContactgegevenWerdToegevoegd
                                                          .Contactgegeventype,
                                             _classFixture.Scenario.ContactgegevenWerdToegevoegd
                                                          .Waarde,
                                             _classFixture.Scenario.ContactgegevenWerdToegevoegd
                                                          .Beschrijving,
                                             _classFixture.Scenario.ContactgegevenWerdToegevoegd
                                                          .IsPrimair));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
