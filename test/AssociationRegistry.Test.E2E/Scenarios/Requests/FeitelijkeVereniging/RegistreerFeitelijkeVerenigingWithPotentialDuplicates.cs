namespace AssociationRegistry.Test.E2E.Scenarios.Requests.FeitelijkeVereniging;

using Admin.Api.DecentraalBeheer.Verenigingen;
using Admin.Api.DecentraalBeheer.Verenigingen.Common;
using Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using Alba;
using Admin.Api.Infrastructure;
using Events;
using Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Common.AutoFixture;
using Framework.ApiSetup;
using Vereniging;
using AutoFixture;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Adres = Admin.Api.DecentraalBeheer.Verenigingen.Common.Adres;

public class RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory : ITestRequestFactory<RegistreerFeitelijkeVerenigingRequest>
{
    private readonly FeitelijkeVerenigingWerdGeregistreerd _potentialDuplicateVerenigingWerdGeregistreerd;
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";

    public RegistreerFeitelijkeVerenigingWithPotentialDuplicatesRequestFactory(FeitelijkeVerenigingWerdGeregistreerd potentialDuplicateVerenigingWerdGeregistreerd)
    {
        _potentialDuplicateVerenigingWerdGeregistreerd = potentialDuplicateVerenigingWerdGeregistreerd;
    }

    public async Task<RequestResult<RegistreerFeitelijkeVerenigingRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = _potentialDuplicateVerenigingWerdGeregistreerd.Naam, // PUT THIS BACK KOEN
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
                        Gemeente = _potentialDuplicateVerenigingWerdGeregistreerd.Locaties.First().Adres.Gemeente, // PUT THIS BACK KOEN
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

        var bevestigingsTokenHelper = new BevestigingsTokenHelper(apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>());

        var hashForAllowingDuplicate = bevestigingsTokenHelper.Calculate(request);

        var vCode = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.WithRequestHeader(WellknownHeaderNames.BevestigingsToken, hashForAllowingDuplicate);
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/feitelijkeverenigingen");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();
            s.Header("Location").SingleValueShouldMatch($"{apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response.Headers.Location.First().Split('/').Last();

        await apiSetup.AdminApiHost.WaitForNonStaleProjectionDataAsync(TimeSpan.FromSeconds(60));

        return new RequestResult<RegistreerFeitelijkeVerenigingRequest>(VCode.Create(vCode), request);
    }
}
