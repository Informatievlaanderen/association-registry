namespace AssociationRegistry.Test.Admin.Api.VerenigingOfAnyKind.When_Wijzig_Locatie;

using AutoFixture;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Marten;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Categories;

public class Patch_A_Locatie_Given_A_VerenigingMetRechtspersoonlijkheid : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public Registratiedata.Locatie TeWijzigenLocatie { get; }
    public V032_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithLocaties_ForWijzigen Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Patch_A_Locatie_Given_A_VerenigingMetRechtspersoonlijkheid(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        var autofixture = new Fixture().CustomizeAdminApi();

        Scenario = fixture.V032VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithLocatiesForWijzigen;
        DocumentStore = _fixture.DocumentStore;

        var locatie = Scenario.LocatieWerdToegevoegd.Locatie;

        TeWijzigenLocatie = locatie with
        {
            Locatietype = Locatietype.Correspondentie,
            IsPrimair = !locatie.IsPrimair,
            Naam = autofixture.Create<string>(),
            Adres = autofixture.Create<Registratiedata.Adres>(),
        };

        _jsonBody = $@"{{
            ""locatie"": {{
                ""locatietype"": ""{TeWijzigenLocatie.Locatietype}"",
                ""isPrimair"": {TeWijzigenLocatie.IsPrimair.ToString().ToLower()},
                ""naam"": ""{TeWijzigenLocatie.Naam}"",
                ""adres"": {{
                    ""straatnaam"": ""{TeWijzigenLocatie.Adres.Straatnaam}"",
                    ""huisnummer"": ""{TeWijzigenLocatie.Adres.Huisnummer}"",
                    ""busnummer"": ""{TeWijzigenLocatie.Adres.Busnummer}"",
                    ""postcode"": ""{TeWijzigenLocatie.Adres.Postcode}"",
                    ""gemeente"": ""{TeWijzigenLocatie.Adres.Gemeente}"",
                    ""land"": ""{TeWijzigenLocatie.Adres.Land}"",
                }}
            }}
        }}";
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PatchLocatie(Scenario.VCode, TeWijzigenLocatie.LocatieId, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_VerenigingMetRechtspersoonlijkheid : IClassFixture<Patch_A_Locatie_Given_A_VerenigingMetRechtspersoonlijkheid>
{
    private readonly Patch_A_Locatie_Given_A_VerenigingMetRechtspersoonlijkheid _classFixture;

    public Given_A_VerenigingMetRechtspersoonlijkheid(Patch_A_Locatie_Given_A_VerenigingMetRechtspersoonlijkheid classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var locatieWerdGewijzigd = (await session.Events
                                                 .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(LocatieWerdGewijzigd));

        locatieWerdGewijzigd.Data.Should()
                            .BeEquivalentTo(new LocatieWerdGewijzigd(_classFixture.TeWijzigenLocatie));
    }

    [Fact]
    public async Task Then_it_returns_an_accepted_response()
        => _classFixture.Response.StatusCode.Should().Be(
            HttpStatusCode.Accepted,
            await _classFixture.Response.Content.ReadAsStringAsync());
}
