namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Contactgegeven;

using System.Net;
using AutoFixture;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Marten;
using Xunit;
using Xunit.Categories;

public class Patch_A_New_Contactgegeven: IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public string WaardeVolgensType { get; }
    public FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven AanTePassenContactGegeven { get; }
    public V008_FeitelijkeVerenigingWerdGeregistreerd_WithContactgegeven Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Patch_A_New_Contactgegeven(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();

        _fixture = fixture;

        Scenario = fixture.V008FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven;
        DocumentStore = _fixture.DocumentStore;

        var contactgegeven = Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens.First();
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
        Response = await _fixture.AdminApiClient.PatchContactgegevens(Scenario.VCode, AanTePassenContactGegeven.ContactgegevenId, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_An_Existing_Contactgegeven : IClassFixture<Patch_A_New_Contactgegeven>
{
    private readonly Patch_A_New_Contactgegeven _classFixture;

    public Given_An_Existing_Contactgegeven(Patch_A_New_Contactgegeven classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();
        var contactgegevenWerdAangepast = (await session.Events
                .FetchStreamAsync(_classFixture.Scenario.VCode))
            .Single(e => e.Data.GetType() == typeof(ContactgegevenWerdGewijzigd));

        contactgegevenWerdAangepast.Data.Should()
            .BeEquivalentTo(new ContactgegevenWerdGewijzigd(_classFixture.AanTePassenContactGegeven.ContactgegevenId, _classFixture.AanTePassenContactGegeven.Type, _classFixture.WaardeVolgensType, "algemeen", false));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
