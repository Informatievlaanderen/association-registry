namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Adding_Contactgegeven;

using System.Net;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using Vereniging;
using FluentAssertions;
using Marten;
using Xunit;
using Xunit.Categories;

public class Post_A_New_Contactgegeven : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public V002_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Post_A_New_Contactgegeven(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields;
        DocumentStore = _fixture.ApiDocumentStore;

        _jsonBody = $@"{{
            ""contactgegeven"":
                {{
                    ""type"":""email"",
                    ""waarde"": ""test@example.org"",
                    ""beschrijving"": ""algemeen"",
                    ""isPrimair"": false
                }},
            ""initiator"": ""OVO000001""
        }}";
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PostContactgegevens(Scenario.VCode, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_New_Contactgegeven : IClassFixture<Post_A_New_Contactgegeven>
{
    private readonly Post_A_New_Contactgegeven _classFixture;

    public Given_A_New_Contactgegeven(Post_A_New_Contactgegeven classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();
        var contactgegevenWerdToegevoegd = (await session.Events
                .FetchStreamAsync(_classFixture.Scenario.VCode))
            .Single(e => e.Data.GetType() == typeof(ContactgegevenWerdToegevoegd));

        contactgegevenWerdToegevoegd.Data.Should()
            .BeEquivalentTo(
                new ContactgegevenWerdToegevoegd(
                    1,
                    ContactgegevenType.Email,
                    "test@example.org",
                    "algemeen",
                    false));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
