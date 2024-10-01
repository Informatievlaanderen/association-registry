namespace AssociationRegistry.Test.E2E.When_Registreer_FeitelijkeVereniging_With_Potential_Duplicates;

using Admin.Api.Infrastructure;
using Admin.Api.Verenigingen;
using Admin.Api.Verenigingen.Common;
using Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Admin.Schema;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using Framework.ApiSetup;
using Framework.TestClasses;
using Hosts.Configuration.ConfigurationBindings;
using Marten;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Scenarios;
using System.Net;
using Vereniging;
using Xunit;
using Adres = Admin.Api.Verenigingen.Common.Adres;

public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext<T> : End2EndContext<RegistreerFeitelijkeVerenigingRequest, FeitelijkeVerenigingWerdGeregistreerdScenario>, IAsyncLifetime
    where T : IApiSetup, new()
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public override FeitelijkeVerenigingWerdGeregistreerdScenario Scenario => new();
    public override RegistreerFeitelijkeVerenigingRequest Request { get; }

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesContext() : base(new T())
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            //Naam = Scenario.VerenigingWerdGeregistreerd.Naam, // PUT THIS BACK KOEN
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            IsUitgeschrevenUitPubliekeDatastroom = false,
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
                        // Gemeente = Scenario.VerenigingWerdGeregistreerd.Locaties.First().Adres.Gemeente, // PUT THIS BACK KOEN
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
    }

    public async Task InitializeAsync()
    {
        // Using Marten, wipe out all data and reset the state
        await AdminApiHost.DocumentStore().Advanced.ResetAllData();

        await Given(Scenario);

        var bevestigingsTokenHelper = new BevestigingsTokenHelper(AdminApiHost.Services.GetRequiredService<AppSettings>());

        var hashForAllowingDuplicate = bevestigingsTokenHelper.Calculate(Request);

        VCode = (await AdminApiHost.Scenario(s =>
        {
            s.WithRequestHeader(WellknownHeaderNames.BevestigingsToken, hashForAllowingDuplicate);
            s.Post
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl("/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();
            s.Header("Location").SingleValueShouldMatch($"{AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response.Headers.Location.First().Split('/').Last();

        await ProjectionHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
    }

    public Metadata Metadata { get; set; }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
