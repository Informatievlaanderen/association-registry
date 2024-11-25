namespace AssociationRegistry.Test.Admin.Api.Commands.VerenigingOfAnyKind.When_Wijzig_Contactgegeven;

using AutoFixture;
using Common.AutoFixture;
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

public class Patch_A_New_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public string WaardeVolgensType { get; }
    public V036_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMinimalFields_ForWijzigenContactgegeven Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Patch_A_New_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        _fixture = fixture;

        Scenario = fixture.V036VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithMinimalFieldsForWijzigenContactgegeven;
        DocumentStore = _fixture.DocumentStore;

        WaardeVolgensType = autoFixture.CreateContactgegevenVolgensType(Scenario.ContactgegevenWerdToegevoegd.Contactgegeventype).Waarde;

        _jsonBody = $@"{{
            ""contactgegeven"":
                {{
                    ""waarde"": ""{WaardeVolgensType}"",
                    ""beschrijving"": ""algemeen"",
                    ""isPrimair"": false
                }},
            ""initiator"": ""OVO000001""
        }}";
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PatchContactgegevens(Scenario.VCode,
                                                                      Scenario.ContactgegevenWerdToegevoegd.ContactgegevenId, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_VerenigingMetRechtspersoonlijkheid : IClassFixture<
    Patch_A_New_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid>
{
    private readonly Patch_A_New_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid _classFixture;

    public Given_A_VerenigingMetRechtspersoonlijkheid(Patch_A_New_Contactgegeven_Given_A_VerenigingMetRechtspersoonlijkheid classFixture)
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
                                   .BeEquivalentTo(new ContactgegevenWerdGewijzigd(
                                                       _classFixture.Scenario.ContactgegevenWerdToegevoegd.ContactgegevenId,
                                                       _classFixture.Scenario.ContactgegevenWerdToegevoegd.Contactgegeventype,
                                                       _classFixture.WaardeVolgensType, Beschrijving: "algemeen", IsPrimair: false));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
