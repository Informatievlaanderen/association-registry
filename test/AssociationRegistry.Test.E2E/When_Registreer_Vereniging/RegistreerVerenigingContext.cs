namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging;

using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.TestClasses;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Coordination;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;
using Vereniging;
using Xunit;
using Xunit.Abstractions;
using Adres = Admin.Api.Verenigingen.Common.Adres;
using AdresId = Admin.Api.Verenigingen.Common.AdresId;

[CollectionDefinition(nameof(RegistreerVerenigingContext))]
public class RegistreerVerenigingCollection : ICollectionFixture<RegistreerVerenigingContext>
{

}
public class RegistreerVerenigingContext : AppFixture, IAsyncLifetime, IEnd2EndContext<RegistreerFeitelijkeVerenigingRequest>
{
    private IScenarioResult _result;
    public RegistreerFeitelijkeVerenigingRequest Request { get; private set; }

    public RegistreerVerenigingContext(): base(nameof(RegistreerVerenigingContext))
    {

    }


    public async Task InitializeAsync()
    {
        await base.InitializeAsync();
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
                        Land = "België",
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
                        Land = "België",
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
                        Land = "België",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
            },
            Vertegenwoordigers =
            [
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
            ],
            HoofdactiviteitenVerenigingsloket = new[] { "BIAG", "BWWC" },
        };
        // Using Marten, wipe out all data and reset the state
        await AdminApiHost.DocumentStore().Advanced.ResetAllData();

        _result = await AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl($"/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);
            s.Header("Location").ShouldHaveValues();
        });


        ResultingVCode = _result.Context.Response.Headers.Location.First().Split('/').Last();

        await AdminProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
    }

    public string ResultingVCode { get; set; }
    public Metadata Metadata { get; set; }

    public new Task DisposeAsync()
        => Task.CompletedTask;
}
