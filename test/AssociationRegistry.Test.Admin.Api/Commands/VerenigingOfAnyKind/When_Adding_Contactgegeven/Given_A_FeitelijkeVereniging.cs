namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Adding_Contactgegeven;

using Events;
using Common.Scenarios.EventsInDb;
using Vereniging;
using FluentAssertions;
using Framework.Fixtures;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Post_A_New_Contactgegeven : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;

    public Post_A_New_Contactgegeven(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields;
        DocumentStore = _fixture.DocumentStore;

        _jsonBody = @"{
            ""contactgegeven"":
                {
                    ""contactgegeventype"":""e-mail"",
                    ""waarde"": ""test@example.org"",
                    ""beschrijving"": ""algemeen"",
                    ""isPrimair"": false
                },
            ""initiator"": ""OVO000001""
        }";
    }

    public V002_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

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
public class Given_A_FeitelijkeVereniging : IClassFixture<Post_A_New_Contactgegeven>
{
    private readonly Post_A_New_Contactgegeven _classFixture;

    public Given_A_FeitelijkeVereniging(Post_A_New_Contactgegeven classFixture)
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
                                             ContactgegevenId: 1,
                                             Contactgegeventype.Email,
                                             Waarde: "test@example.org",
                                             Beschrijving: "algemeen",
                                             IsPrimair: false));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
