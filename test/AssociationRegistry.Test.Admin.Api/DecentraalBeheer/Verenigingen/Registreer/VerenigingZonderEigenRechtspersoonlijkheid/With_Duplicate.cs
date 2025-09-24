namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

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
using FluentAssertions.Execution;
using Formats;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
public class With_Duplicate : IClassFixture<With_Duplicate.Setup>
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly Setup _setup;

    public With_Duplicate(EventsInDbScenariosFixture fixture, Setup setup)
    {
        _fixture = fixture;
        _setup = setup;
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _setup.Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async ValueTask Then_VerenigingWerdNietGeregistreerd()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var savedEvents = await session.Events
                                       .QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                       .ToListAsync();

        savedEvents.Should().NotContainEquivalentOf(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                VCode: string.Empty,
                Naam: _setup.Request.Naam,
                KorteNaam: _setup.Request.KorteNaam ?? string.Empty,
                KorteBeschrijving: _setup.Request.KorteBeschrijving ?? string.Empty,
                Startdatum: _setup.Request.Startdatum,
                Doelgroep: EventFactory.Doelgroep(Doelgroep.Null),
                IsUitgeschrevenUitPubliekeDatastroom: _setup.Request.IsUitgeschrevenUitPubliekeDatastroom,
                Contactgegevens: [],
                Locaties:
                [
                    new Registratiedata.Locatie(
                        LocatieId: 1,
                        _setup.RequestLocatie.Locatietype,
                        _setup.RequestLocatie.IsPrimair,
                        _setup.RequestLocatie.Naam ?? string.Empty,
                        new Registratiedata.Adres(_setup.RequestLocatie.Adres!.Straatnaam,
                                                  _setup.RequestLocatie.Adres.Huisnummer,
                                                  _setup.RequestLocatie.Adres.Busnummer ?? string.Empty,
                                                  _setup.RequestLocatie.Adres.Postcode,
                                                  _setup.RequestLocatie.Adres.Gemeente,
                                                  _setup.RequestLocatie.Adres.Land),
                        AdresId: null),
                ],
                Vertegenwoordigers: [],
                HoofdactiviteitenVerenigingsloket: [],
                Registratiedata.DuplicatieInfo.GeenDuplicaten
            ),
            config: options => options.Excluding(e => e.VCode));
    }

    [Fact]
    public async ValueTask Then_A_DubbelDetectieWerdGedetecteerd_Event_Was_Saved()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var savedEvents = await session.Events
                                       .QueryRawEventDataOnly<DubbeleVerenigingenWerdenGedetecteerd>()
                                       .ToListAsync();

        var potentieleDubbel = _fixture.V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce
                                      .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

        using (new AssertionScope())
        {
            AssertionScope.Current.FormattingOptions.MaxDepth = 20;   // default is 5
            AssertionScope.Current.FormattingOptions.MaxLines = 2000; // default is 100

            var p = JsonConvert.SerializeObject(savedEvents.First());

            var dubbeleVerenigingenWerdenGedetecteerd = new DubbeleVerenigingenWerdenGedetecteerd(Bevestigingstoken: string.Empty,
                                                                                                  Naam: _setup.Request.Naam,
                                                                                                  Locaties:
                                                                                                  [
                                                                                                      new Registratiedata.Locatie(
                                                                                                          LocatieId: 0,
                                                                                                          _setup.RequestLocatie.Locatietype,
                                                                                                          _setup.RequestLocatie.IsPrimair,
                                                                                                          _setup.RequestLocatie.Naam ?? string.Empty,
                                                                                                          new Registratiedata.Adres(_setup.RequestLocatie.Adres!.Straatnaam,
                                                                                                                                    _setup.RequestLocatie.Adres.Huisnummer,
                                                                                                                                    _setup.RequestLocatie.Adres.Busnummer ?? string.Empty,
                                                                                                                                    _setup.RequestLocatie.Adres.Postcode,
                                                                                                                                    _setup.RequestLocatie.Adres.Gemeente,
                                                                                                                                    _setup.RequestLocatie.Adres.Land),
                                                                                                          AdresId: null),
                                                                                                  ],
                                                                                                  GedetecteerdeDubbels: new Registratiedata.DuplicateVereniging[]
                                                                                                  {
                                                                                                      new Registratiedata.DuplicateVereniging(potentieleDubbel.VCode,
                                                                                                                                              new Registratiedata.Verenigingstype(Verenigingstype.VZER.Code,Verenigingstype.VZER.Naam),
                                                                                                                                              new Registratiedata.Verenigingssubtype(VerenigingssubtypeCode.Default.Code,VerenigingssubtypeCode.Default.Naam),
                                                                                                                                              potentieleDubbel.Naam,
                                                                                                                                              potentieleDubbel.KorteNaam,
                                                                                                                                              potentieleDubbel.HoofdactiviteitenVerenigingsloket.Select(x => new Registratiedata.HoofdactiviteitVerenigingsloket(x.Code, x.Naam)).ToArray(),
                                                                                                                                              potentieleDubbel.Locaties.Select(x => new Registratiedata.DuplicateVerenigingLocatie(x.Locatietype, x.IsPrimair, x.Adres.ToAdresString(), x.Naam, x.Adres.Postcode, x.Adres.Gemeente )).ToArray()
                                                                                                      )
                                                                                                  }
            );

            var expected = JsonConvert.SerializeObject(dubbeleVerenigingenWerdenGedetecteerd);
           savedEvents.Should().ContainEquivalentOf(
            dubbeleVerenigingenWerdenGedetecteerd,
            config: options => options.Excluding(e => e.Bevestigingstoken));
        }


    }

    public sealed class Setup
    {
        public readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest Request;
        public readonly HttpResponseMessage Response;

        public Setup(EventsInDbScenariosFixture fixture)
        {
            var autoFixture = new Fixture().CustomizeAdminApi();
            RequestLocatie = autoFixture.Create<ToeTeVoegenLocatie>();

            RequestLocatie.Adres!.Gemeente = fixture.V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce
                                                    .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First()
                                                    .Adres!.Gemeente;

            Request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
            {
                Naam = fixture.V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam,
                Locaties =
                [
                    RequestLocatie,
                ],
                Startdatum = DateOnly.FromDateTime(DateTime.Now).AddDays(autoFixture.Create<int>()* -1),
            };

            var requestAsJson = JsonConvert.SerializeObject(Request);

            Response = fixture.DefaultClient.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(requestAsJson)
                              .GetAwaiter().GetResult();
        }

        public ToeTeVoegenLocatie RequestLocatie { get; }
    }
}






