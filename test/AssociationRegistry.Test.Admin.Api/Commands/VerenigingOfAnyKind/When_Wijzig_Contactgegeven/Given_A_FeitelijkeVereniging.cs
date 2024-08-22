namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven;

using AutoFixture;
using Common.Scenarios.EventsInDb;
using Events;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Marten;
using System.Net;
using Test.Framework.Customizations;
using Xunit;
using Xunit.Categories;

public class Patch_A_New_Contactgegeven_Given_A_FeitelijkeVereniging : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public string WaardeVolgensType { get; }
    public Registratiedata.Contactgegeven AanTePassenContactGegeven { get; }
    public V008_FeitelijkeVerenigingWerdGeregistreerd_WithContactgegeven Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Patch_A_New_Contactgegeven_Given_A_FeitelijkeVereniging(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        _fixture = fixture;

        Scenario = fixture.V008FeitelijkeVerenigingWerdGeregistreerdWithContactgegeven;
        DocumentStore = _fixture.DocumentStore;

        var contactgegeven = Scenario.FeitelijkeVerenigingWerdGeregistreerd.Contactgegevens.First();
        WaardeVolgensType = autoFixture.CreateContactgegevenVolgensType(contactgegeven.Contactgegeventype).Waarde;

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
        Response = await _fixture.AdminApiClient.PatchContactgegevens(Scenario.VCode, AanTePassenContactGegeven.ContactgegevenId,
                                                                      _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_FeitelijkeVereniging : IClassFixture<Patch_A_New_Contactgegeven_Given_A_FeitelijkeVereniging>
{
    private readonly Patch_A_New_Contactgegeven_Given_A_FeitelijkeVereniging _classFixture;

    public Given_A_FeitelijkeVereniging(Patch_A_New_Contactgegeven_Given_A_FeitelijkeVereniging classFixture)
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
                                   .BeEquivalentTo(new ContactgegevenWerdGewijzigd(_classFixture.AanTePassenContactGegeven.ContactgegevenId,
                                                                                   _classFixture.AanTePassenContactGegeven
                                                                                      .Contactgegeventype, _classFixture.WaardeVolgensType,
                                                                                   Beschrijving: "algemeen", IsPrimair: false));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
