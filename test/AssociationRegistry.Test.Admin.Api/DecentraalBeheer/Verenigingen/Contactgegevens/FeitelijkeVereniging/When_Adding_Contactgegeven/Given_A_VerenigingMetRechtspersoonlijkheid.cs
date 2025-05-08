namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Contactgegevens.FeitelijkeVereniging.When_Adding_Contactgegeven;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging;
using FluentAssertions;
using Marten;
using System.Net;
using Xunit;

public class Post_A_New_Contactgegeven_To_VerenigingMetRechtspersoonlijkheid : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;

    public Post_A_New_Contactgegeven_To_VerenigingMetRechtspersoonlijkheid(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;

        Scenario = fixture.V035VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForAddingContactgegeven;
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

    public V035_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForAddingContactgegeven Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PostContactgegevens(Scenario.VCode, _jsonBody);
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

[Collection(nameof(AdminApiCollection))]
public class Given_A_VerenigingMetRechtspersoonlijkheid : IClassFixture<Post_A_New_Contactgegeven_To_VerenigingMetRechtspersoonlijkheid>
{
    private readonly Post_A_New_Contactgegeven_To_VerenigingMetRechtspersoonlijkheid _classFixture;

    public Given_A_VerenigingMetRechtspersoonlijkheid(Post_A_New_Contactgegeven_To_VerenigingMetRechtspersoonlijkheid classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
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
