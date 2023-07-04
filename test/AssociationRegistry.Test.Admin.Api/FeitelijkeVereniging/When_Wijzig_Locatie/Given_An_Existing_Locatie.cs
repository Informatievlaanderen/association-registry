namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_Wijzig_Locatie;

using System.Net;
using AutoFixture;
using Events;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using FluentAssertions.Execution;
using Framework;
using Marten;
using Vereniging;
using Xunit;
using Xunit.Categories;

public class Patch_A_Locatie : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public Registratiedata.Locatie TeWijzigenLocatie { get; }
    public V026_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Patch_A_Locatie(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        var autofixture = new Fixture().CustomizeAll();

        Scenario = fixture.V026FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigen;
        DocumentStore = _fixture.DocumentStore;

        var locatie = Scenario.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First();
        TeWijzigenLocatie = locatie with
        {
            Locatietype = Locatietype.Correspondentie,
            IsPrimair = !locatie.IsPrimair,
            Naam = autofixture.Create<string>(),
            Adres = autofixture.Create<Registratiedata.Adres>(),
            AdresId = autofixture.Create<Registratiedata.AdresId>(),
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
                }},
                ""adresId"": {{
                    ""broncode"": ""{TeWijzigenLocatie.AdresId.Broncode}"",
                    ""bronwaarde"": ""{TeWijzigenLocatie.AdresId.Bronwaarde}"",
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
public class Given_An_Existing_Locatie : IClassFixture<Patch_A_Locatie>
{
    private readonly Patch_A_Locatie _classFixture;

    public Given_An_Existing_Locatie(Patch_A_Locatie classFixture)
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
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted, await _classFixture.Response.Content.ReadAsStringAsync());
    }
}
