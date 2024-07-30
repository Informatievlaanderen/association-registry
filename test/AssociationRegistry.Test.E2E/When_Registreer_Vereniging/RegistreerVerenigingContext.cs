namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Vereniging;
using Xunit;
using Adres = Admin.Api.Verenigingen.Common.Adres;
using AdresId = Admin.Api.Verenigingen.Common.AdresId;

[CollectionDefinition(nameof(RegistreerVerenigingContext))]
public class AppFixtureCollection : ICollectionFixture<AppFixture>
{

}
public abstract class RegistreerVerenigingContext : IAsyncLifetime
{
    private IScenarioResult _result;
    private readonly AppFixture _fixture;
    private readonly IDocumentStore ProjectionStore;
    public RegistreerFeitelijkeVerenigingRequest Request { get; private set; }

    protected RegistreerVerenigingContext(AppFixture fixture)
    {
        _fixture = fixture;
        Host = fixture.Host;
        Store = Host.Services.GetRequiredService<IDocumentStore>();
        ProjectionStore = fixture.ProjectionHostServer.Services.GetRequiredService<IDocumentStore>();
    }

    public IAlbaHost Host { get; }
    public IDocumentStore Store { get; }

    public async Task InitializeAsync()
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = autoFixture.Create<string>(),
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            IsUitgeschrevenUitPubliekeDatastroom = true,
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            Contactgegevens = new ToeTeVoegenContactgegeven[]
            {
                new()
                {
                    Contactgegeventype = Contactgegeventype.Email,
                    Waarde = "random@example.org",
                    Beschrijving = "Algemeen",
                    IsPrimair = false,
                },
            },
            Locaties = new[]
            {
                new ToeTeVoegenLocatie
                {
                    Naam = "Kantoor",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Busnummer = "",
                        Postcode = "9200",
                        Gemeente = "Dendermonde",
                        Land = "Belgie",
                    },
                    IsPrimair = true,
                    Locatietype = Locatietype.Correspondentie,
                },
                new ToeTeVoegenLocatie
                {
                    Naam = "Afhalingen",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Busnummer = "",
                        Postcode = "1234",
                        Gemeente = "Dendermonde",
                        Land = "Belgie",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
                new ToeTeVoegenLocatie
                {
                    Naam = "Speeltuin",
                    Adres = new Adres
                    {
                        Straatnaam = "dorpelstraat",
                        Huisnummer = "169",
                        Busnummer = "2",
                        Postcode = "4567",
                        Gemeente = "Nothingham",
                        Land = "Belgie",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
                // new ToeTeVoegenLocatie
                // {
                //     Naam = "Zwembad",
                //     AdresId = new AdresId
                //     {
                //         Broncode = "AR",
                //         Bronwaarde = AssociationRegistry.Vereniging.AdresId.DataVlaanderenAdresPrefix + "5",
                //     },
                //     IsPrimair = false,
                //     Locatietype = Locatietype.Activiteiten,
                // },
            },
            Vertegenwoordigers = new[]
            {
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = autoFixture.Create<Insz>(),
                    Voornaam = "Jane",
                    Achternaam = "Doe",
                    Rol = "Voorzitter, Hoofdcoach",
                    Roepnaam = "QTPY",
                    IsPrimair = true,
                    Email = "qtpy@example.org",
                    Telefoon = "0123456789",
                    Mobiel = "987654321",
                    SocialMedia = "http://example.com",
                },
                new ToeTeVoegenVertegenwoordiger
                {
                    Insz = autoFixture.Create<Insz>(),
                    Voornaam = "Kim",
                    Achternaam = "Possible",
                    Rol = "Master",
                    Roepnaam = "Lara",
                    IsPrimair = false,
                    Email = "master@example.org",
                    Telefoon = "0000000000",
                    Mobiel = "6666666666",
                    SocialMedia = "http://example.com/scrum",
                },
            },
            HoofdactiviteitenVerenigingsloket = new[] { "BIAG", "BWWC" },
        };
        // Using Marten, wipe out all data and reset the state
        await Store.Advanced.ResetAllData();

        _result = await Host.Scenario(s =>
        {
            s.Post
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl($"/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
            s.Header("Location").ShouldHaveValues();
        });

        ResultingVCode = _result.Context.Response.Headers.Location.First().Split('/').Last();

        var daemon = await ProjectionStore.BuildProjectionDaemonAsync();
        await daemon.StartAllAsync();
        await daemon.WaitForNonStaleData(TimeSpan.FromSeconds(15));
    }

    public string ResultingVCode { get; set; }

    // This is required because of the IAsyncLifetime
    // interface. Note that I do *not* tear down database
    // state after the test. That's purposeful
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
