namespace AssociationRegistry.Test.Admin.Api.When_Removing_Contactgegeven;

using System.Net;
using Events;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Marten;
using Xunit;
using Xunit.Categories;

public class Delete_An_Existing_Contactgegeven : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    public V007_VerenigingWerdGeregistreerd_WithContactgegeven Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Delete_An_Existing_Contactgegeven(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V007VerenigingWerdGeregistreerdWithContactgegeven;
        DocumentStore = _fixture.DocumentStore;
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.DeleteContactgegeven(Scenario.VCode, 1);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_An_Existing_Contactgegeven : IClassFixture<Delete_An_Existing_Contactgegeven>
{
    private readonly Delete_An_Existing_Contactgegeven _classFixture;

    public Given_An_Existing_Contactgegeven(Delete_An_Existing_Contactgegeven classFixture)
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
            .BeEquivalentTo(new ContactgegevenWerdVerwijderd(1, _classFixture.Scenario.VerenigingWerdGeregistreerd.Contactgegevens[0].Type, _classFixture.Scenario.VerenigingWerdGeregistreerd.Contactgegevens[0].Waarde, _classFixture.Scenario.VerenigingWerdGeregistreerd.Contactgegevens[0].Omschrijving, _classFixture.Scenario.VerenigingWerdGeregistreerd.Contactgegevens[0].IsPrimair));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
