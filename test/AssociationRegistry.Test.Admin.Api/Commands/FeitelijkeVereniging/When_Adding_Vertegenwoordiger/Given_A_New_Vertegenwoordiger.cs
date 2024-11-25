namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_Adding_Vertegenwoordiger;

using AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.VoegVertegenwoordigerToe.RequestModels;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Marten;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Post_A_New_Vertegenwoordiger : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _classFixture;
    private readonly string _jsonBody;
    public V002_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;
    public readonly VoegVertegenwoordigerToeRequest Request;

    public Post_A_New_Vertegenwoordiger(EventsInDbScenariosFixture classFixture)
    {
        _classFixture = classFixture;

        Scenario = classFixture.V002FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields;
        DocumentStore = _classFixture.DocumentStore;

        var fixture = new Fixture().CustomizeAdminApi();
        Request = fixture.Create<VoegVertegenwoordigerToeRequest>();

        _jsonBody = JsonConvert.SerializeObject(Request);
    }

    public async Task InitializeAsync()
    {
        Response = await _classFixture.AdminApiClient.PostVertegenwoordiger(Scenario.VCode, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_New_Vertegenwoordiger : IClassFixture<Post_A_New_Vertegenwoordiger>
{
    private readonly Post_A_New_Vertegenwoordiger _classFixture;

    public Given_A_New_Vertegenwoordiger(Post_A_New_Vertegenwoordiger classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var contactgegevenWerdToegevoegd = (await session.Events
                                                         .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(VertegenwoordigerWerdToegevoegd));

        var toeTeVoegenVertegenwoordiger = _classFixture.Request.Vertegenwoordiger;

        contactgegevenWerdToegevoegd.Data.Should()
                                    .BeEquivalentTo(new VertegenwoordigerWerdToegevoegd(
                                                        VertegenwoordigerId: 1,
                                                        toeTeVoegenVertegenwoordiger.Insz,
                                                        toeTeVoegenVertegenwoordiger.IsPrimair,
                                                        toeTeVoegenVertegenwoordiger.Roepnaam ?? string.Empty,
                                                        toeTeVoegenVertegenwoordiger.Rol ?? string.Empty,
                                                        toeTeVoegenVertegenwoordiger.Voornaam,
                                                        toeTeVoegenVertegenwoordiger.Achternaam,
                                                        toeTeVoegenVertegenwoordiger.Email ?? string.Empty,
                                                        toeTeVoegenVertegenwoordiger.Telefoon ?? string.Empty,
                                                        toeTeVoegenVertegenwoordiger.Mobiel ?? string.Empty,
                                                        toeTeVoegenVertegenwoordiger.SocialMedia ?? string.Empty));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
