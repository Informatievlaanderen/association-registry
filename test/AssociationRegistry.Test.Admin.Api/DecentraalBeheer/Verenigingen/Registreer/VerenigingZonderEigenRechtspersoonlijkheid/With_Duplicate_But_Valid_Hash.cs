namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

using System.Net;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Events.Factories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

[Collection(name: nameof(AdminApiCollection))]
public class With_Duplicate_But_Valid_Hash : IClassFixture<With_Duplicate_But_Valid_Hash.Setup>
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly Setup _setup;

    public With_Duplicate_But_Valid_Hash(EventsInDbScenariosFixture fixture, Setup setup)
    {
        _fixture = fixture;
        _setup = setup;
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _setup.Response.StatusCode.Should().Be(expected: HttpStatusCode.Accepted);
    }

    [Fact]
    public async ValueTask Then_VerenigingWerdGeregistreerd()
    {
        await using var session = _fixture.DocumentStore.LightweightSession();

        var savedEvents = await session
            .Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens>()
            .ToListAsync();

        savedEvents
            .Should()
            .ContainEquivalentOf(
                expectation: new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdZonderPersoonsgegevens(
                    VCode: string.Empty,
                    Naam: _setup.Request.Naam,
                    KorteNaam: _setup.Request.KorteNaam ?? string.Empty,
                    KorteBeschrijving: _setup.Request.KorteBeschrijving ?? string.Empty,
                    Startdatum: _setup.Request.Startdatum,
                    Doelgroep: EventFactory.Doelgroep(doelgroep: Doelgroep.Null),
                    IsUitgeschrevenUitPubliekeDatastroom: _setup.Request.IsUitgeschrevenUitPubliekeDatastroom,
                    Contactgegevens: [],
                    Locaties:
                    [
                        new Registratiedata.Locatie(
                            LocatieId: 1,
                            Locatietype: _setup.RequestLocatie.Locatietype,
                            IsPrimair: _setup.RequestLocatie.IsPrimair,
                            Naam: _setup.RequestLocatie.Naam ?? string.Empty,
                            Adres: new Registratiedata.Adres(
                                Straatnaam: _setup.RequestLocatie.Adres!.Straatnaam,
                                Huisnummer: _setup.RequestLocatie.Adres.Huisnummer,
                                Busnummer: _setup.RequestLocatie.Adres.Busnummer ?? string.Empty,
                                Postcode: _setup.RequestLocatie.Adres.Postcode,
                                Gemeente: _setup.RequestLocatie.Adres.Gemeente,
                                Land: _setup.RequestLocatie.Adres.Land
                            ),
                            AdresId: null
                        ),
                    ],
                    Vertegenwoordigers: [],
                    HoofdactiviteitenVerenigingsloket: [],
                    Bankrekeningnummers: [],
                    DuplicatieInfo: Registratiedata.DuplicatieInfo.BevestigdGeenDuplicaat(
                        bevestigingstoken: _setup.BevestigingsToken
                    )
                ),
                config: options => options.Excluding(expression: e => e.VCode)
            );
    }

    public sealed class Setup
    {
        public readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest Request;
        public readonly HttpResponseMessage Response;
        public string BevestigingsToken { get; }

        public Setup(EventsInDbScenariosFixture fixture)
        {
            var autoFixture = new Fixture().CustomizeAdminApi();
            RequestLocatie = autoFixture.Create<ToeTeVoegenLocatie>();

            RequestLocatie.Adres!.Gemeente = fixture
                .V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First()
                .Adres!.Gemeente;

            Request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
            {
                Naam = fixture
                    .V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce
                    .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                    .Naam,
                Locaties = [RequestLocatie],
                Startdatum = DateOnly
                    .FromDateTime(dateTime: DateTime.Now)
                    .AddDays(value: autoFixture.Create<int>() * -1),
            };

            var bevestigingsTokenHelper = new BevestigingsTokenHelper(
                appSettings: fixture.ServiceProvider.GetRequiredService<AppSettings>()
            );

            var requestAsJson = JsonConvert.SerializeObject(value: Request);

            BevestigingsToken = bevestigingsTokenHelper.Calculate(request: Request);

            Response = fixture
                .DefaultClient.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
                    content: requestAsJson,
                    bevestigingsToken: BevestigingsToken
                )
                .GetAwaiter()
                .GetResult();
        }

        public ToeTeVoegenLocatie RequestLocatie { get; }
    }
}
