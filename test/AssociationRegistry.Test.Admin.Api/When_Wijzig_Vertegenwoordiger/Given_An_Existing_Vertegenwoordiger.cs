namespace AssociationRegistry.Test.Admin.Api.When_Wijzig_Vertegenwoordiger;

using System.Net;
using AutoFixture;
using Events;
using Fixtures;
using Fixtures.Scenarios;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Marten;
using Vereniging.Emails;
using Vereniging.SocialMedias;
using Vereniging.TelefoonNummers;
using Xunit;
using Xunit.Categories;

public class Patch_A_New_Vertegenwoordiger : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public readonly string Email;
    public readonly string SocialMedia;
    public readonly string TelefoonNummer;
    public readonly string Mobiel;
    public readonly string Rol;
    public readonly string Roepnaam;
    public FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger AanTePassenVertegenwoordiger { get; }
    public V012_FeitelijkeVerenigingWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;


    public Patch_A_New_Vertegenwoordiger(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();

        _fixture = fixture;

        Scenario = fixture.V012FeitelijkeVerenigingWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger;
        DocumentStore = _fixture.DocumentStore;

        Rol = autoFixture.Create<string>();
        Roepnaam = autoFixture.Create<string>();
        Email = autoFixture.Create<Email>().Waarde;
        SocialMedia = autoFixture.Create<SocialMedia>().Waarde;
        TelefoonNummer = autoFixture.Create<TelefoonNummer>().Waarde;
        Mobiel = autoFixture.Create<TelefoonNummer>().Waarde;

        _jsonBody = $@"{{
            ""vertegenwoordiger"":
                {{
                    ""rol"": ""{Rol}"",
                    ""roepnaam"": ""{Roepnaam}"",
                    ""email"": ""{Email}"",
                    ""socialMedia"": ""{SocialMedia}"",
                    ""telefoon"": ""{TelefoonNummer}"",
                    ""mobiel"": ""{Mobiel}"",
                    ""isPrimair"": false
                }},
            ""initiator"": ""OVO000001""
        }}";
        AanTePassenVertegenwoordiger = Scenario.FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers[0];
    }

    public async Task InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PatchVertegenwoordiger(Scenario.VCode, AanTePassenVertegenwoordiger.VertegenwoordigerId, _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_An_Existing_Vertegenwoordiger : IClassFixture<Patch_A_New_Vertegenwoordiger>
{
    private readonly Patch_A_New_Vertegenwoordiger _classFixture;

    public Given_An_Existing_Vertegenwoordiger(Patch_A_New_Vertegenwoordiger classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async Task Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();
        var vertegenwoordigerWerdGewijzigd = (await session.Events
                .FetchStreamAsync(_classFixture.Scenario.VCode))
            .Single(e => e.Data.GetType() == typeof(VertegenwoordigerWerdGewijzigd));

        vertegenwoordigerWerdGewijzigd.Data.Should()
            .BeEquivalentTo(
                new VertegenwoordigerWerdGewijzigd(
                    _classFixture.AanTePassenVertegenwoordiger.VertegenwoordigerId,
                    false,
                    _classFixture.Roepnaam,
                    _classFixture.Rol,
                    _classFixture.AanTePassenVertegenwoordiger.Voornaam,
                    _classFixture.AanTePassenVertegenwoordiger.Achternaam,
                    _classFixture.Email,
                    _classFixture.TelefoonNummer,
                    _classFixture.Mobiel,
                    _classFixture.SocialMedia));
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }
}
