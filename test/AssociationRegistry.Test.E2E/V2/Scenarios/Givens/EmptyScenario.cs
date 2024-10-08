namespace AssociationRegistry.Test.E2E.V2.Scenarios.Givens;

using Alba;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Adres = Admin.Api.Verenigingen.Common.Adres;
using IEvent = AssociationRegistry.Framework.IEvent;

public class EmptyScenario : Framework.TestClasses.IScenario
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";
    public RegistreerFeitelijkeVerenigingRequest Request { get; private set; }
    public string VCode { get; private set; }

    public Task<Dictionary<string, IEvent[]>> GivenEvents(IVCodeService requiredService)
        => Task.FromResult(new Dictionary<string, IEvent[]>());

    public async Task WhenCommand(FullBlownApiSetup setup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = autoFixture.Create<string>(),
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

        VCode = (await setup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(Request, JsonStyle.MinimalApi)
             .ToUrl("/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();
            s.Header("Location").SingleValueShouldMatch($"{setup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response.Headers.Location.First().Split('/').Last();

        await setup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));
    }

    public CommandMetadata GetCommandMetadata()
        => null;
}
