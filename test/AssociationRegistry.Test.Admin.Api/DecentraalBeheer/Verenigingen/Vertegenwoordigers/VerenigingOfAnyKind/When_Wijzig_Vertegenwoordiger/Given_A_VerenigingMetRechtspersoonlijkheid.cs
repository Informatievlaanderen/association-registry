namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger;

using AssociationRegistry.Events;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using AssociationRegistry.Vereniging.Emails;
using AssociationRegistry.Vereniging.SocialMedias;
using AssociationRegistry.Vereniging.TelefoonNummers;
using AutoFixture;
using FluentAssertions;
using Marten;
using System.Net;
using Xunit;
using Xunit.Categories;

public class Patch_A_New_Vertegenwoordiger_For_VerenigingMetRechtspersoonlijkheid : IAsyncLifetime
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly string _jsonBody;
    public readonly string Email;
    public readonly string SocialMedia;
    public readonly string TelefoonNummer;
    public readonly string Mobiel;
    public readonly string Rol;
    public readonly string Roepnaam;
    public VertegenwoordigerWerdOvergenomenUitKBO AanTePassenVertegenwoordiger { get; }
    public V042_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger Scenario { get; }
    public IDocumentStore DocumentStore { get; }
    public HttpResponseMessage Response { get; private set; } = null!;

    public Patch_A_New_Vertegenwoordiger_For_VerenigingMetRechtspersoonlijkheid(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        _fixture = fixture;

        Scenario = fixture.V042VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigerForWijzigVertegenwoordiger;
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
                    ""e-mail"": ""{Email}"",
                    ""socialMedia"": ""{SocialMedia}"",
                    ""telefoon"": ""{TelefoonNummer}"",
                    ""mobiel"": ""{Mobiel}"",
                    ""isPrimair"": false
                }},
            ""initiator"": ""OVO000001""
        }}";

        AanTePassenVertegenwoordiger = Scenario.VertegenwoordigerWerdOvergenomenUitKBO;
    }

    public async ValueTask InitializeAsync()
    {
        Response = await _fixture.AdminApiClient.PatchVertegenwoordiger(Scenario.VCode, AanTePassenVertegenwoordiger.VertegenwoordigerId,
                                                                        _jsonBody);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}

[IntegrationTest]
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
public class Given_A_VerenigingMetRechtspersoonlijkheid : IClassFixture<
    Patch_A_New_Vertegenwoordiger_For_VerenigingMetRechtspersoonlijkheid>
{
    private readonly Patch_A_New_Vertegenwoordiger_For_VerenigingMetRechtspersoonlijkheid _classFixture;

    public Given_A_VerenigingMetRechtspersoonlijkheid(Patch_A_New_Vertegenwoordiger_For_VerenigingMetRechtspersoonlijkheid classFixture)
    {
        _classFixture = classFixture;
    }

    [Fact]
    public async ValueTask Then_it_saves_the_events()
    {
        await using var session = _classFixture.DocumentStore.LightweightSession();

        var vertegenwoordigerWerdGewijzigd = (await session.Events
                                                           .FetchStreamAsync(_classFixture.Scenario.VCode))
           .Single(e => e.Data.GetType() == typeof(VertegenwoordigerWerdGewijzigd));

        vertegenwoordigerWerdGewijzigd.Data.Should()
                                      .BeEquivalentTo(
                                           new VertegenwoordigerWerdGewijzigd(
                                               _classFixture.AanTePassenVertegenwoordiger.VertegenwoordigerId,
                                               IsPrimair: false,
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
