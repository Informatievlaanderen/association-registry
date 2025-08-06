namespace AssociationRegistry.Test.E2E.Scenarios.Requests.VZER;

using Admin.Api.Infrastructure;
using Admin.Api.WebApi.Verenigingen.Common;
using Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using Alba;
using AutoFixture;
using Common.AutoFixture;
using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Vereniging;
using Adres = Admin.Api.WebApi.Verenigingen.Common.Adres;
using AdresId = Admin.Api.WebApi.Verenigingen.Common.AdresId;

public class RegistreerVZERRequestFactory : ITestRequestFactory<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>
{
    private readonly string _isPositiveInteger = "^[1-9][0-9]*$";
    private string _vCode;

    public RegistreerVZERRequestFactory()
    {
    }

    public async Task<CommandResult<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>> ExecuteRequest(IApiSetup apiSetup)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();

        var request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            Naam = "Een unieke naam",
            KorteNaam = autoFixture.Create<string>(),
            KorteBeschrijving = autoFixture.Create<string>(),
            Startdatum = DateOnly.FromDateTime(DateTime.Today),
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Doelgroep = new DoelgroepRequest
            {
                Minimumleeftijd = 1,
                Maximumleeftijd = 149,
            },
            Contactgegevens =
            [
                new()
                {
                    Contactgegeventype = Contactgegeventype.Email,
                    Waarde = "random@example.org",
                    Beschrijving = "Algemeen",
                    IsPrimair = false,
                },
            ],
            Locaties =
            [
                new ToeTeVoegenLocatie
                {
                    Naam = "Kantoor",
                    Adres = new Adres
                    {
                        Straatnaam = "Leopold II-laan",
                        Huisnummer = "99",
                        Busnummer = "",
                        Postcode = "1500",
                        Gemeente = "Halle",
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
                        Postcode = "1501",
                        Gemeente = "Buizingen",
                        Land = "België",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                },
                new ToeTeVoegenLocatie
                {
                    Naam = "Speeltuin",
                    AdresId = new AdresId()
                    {
                        Bronwaarde = "https://data.vlaanderen.be/id/adres/861019",
                        Broncode = "AR",
                    },
                    IsPrimair = false,
                    Locatietype = Locatietype.Activiteiten,
                }
            ],
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
            HoofdactiviteitenVerenigingsloket = ["BIAG", "BWWC"],
            //Werkingsgebieden = ["BE25", "BE25535002"],
        };

        var response = (await apiSetup.AdminApiHost.Scenario(s =>
        {
            s.Post
             .Json(request, JsonStyle.Mvc)
             .ToUrl("/v1/verenigingen/vzer");

            s.StatusCodeShouldBe(HttpStatusCode.Accepted);

            s.Header("Location").ShouldHaveValues();

            s.Header("Location")
             .SingleValueShouldMatch($"{apiSetup.AdminApiHost.Services.GetRequiredService<AppSettings>().BaseUrl}/v1/verenigingen/V");

            s.Header(WellknownHeaderNames.Sequence).ShouldHaveValues();
            s.Header(WellknownHeaderNames.Sequence).SingleValueShouldMatch(_isPositiveInteger);
        })).Context.Response;

        _vCode = response.Headers.Location.First().Split('/').Last();
        //long sequence = Convert.ToInt64(response.Headers[WellknownHeaderNames.Sequence].First());
        var newSequence = await apiSetup.WaitForAdresMatchEventForEachLocation(_vCode, request.Locaties.Length);

        return new CommandResult<RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest>(VCode.Create(_vCode), request, newSequence);
    }
}
