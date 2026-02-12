namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

using System.Net;
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
using Xunit;

[Collection(name: nameof(AdminApiCollection))]
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
        _setup.Response.StatusCode.Should().Be(expected: HttpStatusCode.Conflict);
    }

    [Fact]
    public async ValueTask Then_VerenigingWerdNietGeregistreerd()
    {
        await using var session = _fixture.DocumentStore.LightweightSession();

        var savedEvents = await session
            .Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .ToListAsync();

        savedEvents
            .Should()
            .NotContainEquivalentOf(
                unexpected: new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
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
                    DuplicatieInfo: Registratiedata.DuplicatieInfo.GeenDuplicaten
                ),
                config: options => options.Excluding(expression: e => e.VCode)
            );
    }

    [Fact]
    public async ValueTask Then_A_DubbelDetectieWerdGedetecteerd_Event_Was_Saved()
    {
        await using var session = _fixture.DocumentStore.LightweightSession();

        var savedEvents = await session
            .Events.QueryRawEventDataOnly<DubbeleVerenigingenWerdenGedetecteerd>()
            .ToListAsync();

        var potentieleDubbel = _fixture
            .V082VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdForDuplicateForce
            .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

        using (new AssertionScope())
        {
            AssertionScope.Current.FormattingOptions.MaxDepth = 20; // default is 5
            AssertionScope.Current.FormattingOptions.MaxLines = 2000; // default is 100

            var p = JsonConvert.SerializeObject(value: savedEvents.First());

            var dubbeleVerenigingenWerdenGedetecteerd = new DubbeleVerenigingenWerdenGedetecteerd(
                Bevestigingstoken: string.Empty,
                Naam: _setup.Request.Naam,
                Locaties:
                [
                    new Registratiedata.Locatie(
                        LocatieId: 0,
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
                GedetecteerdeDubbels:
                [
                    new Registratiedata.DuplicateVereniging(
                        VCode: potentieleDubbel.VCode,
                        Verenigingstype: new Registratiedata.Verenigingstype(
                            Code: Verenigingstype.VZER.Code,
                            Naam: Verenigingstype.VZER.Naam
                        ),
                        Verenigingssubtype: new Registratiedata.Verenigingssubtype(
                            Code: VerenigingssubtypeCode.Default.Code,
                            Naam: VerenigingssubtypeCode.Default.Naam
                        ),
                        Naam: potentieleDubbel.Naam,
                        KorteNaam: potentieleDubbel.KorteNaam,
                        HoofdactiviteitenVerenigingsloket: potentieleDubbel
                            .HoofdactiviteitenVerenigingsloket.Select(
                                selector: x => new Registratiedata.HoofdactiviteitVerenigingsloket(
                                    Code: x.Code,
                                    Naam: x.Naam
                                )
                            )
                            .ToArray(),
                        Locaties: potentieleDubbel
                            .Locaties.Select(selector: x => new Registratiedata.DuplicateVerenigingLocatie(
                                Locatietype: x.Locatietype,
                                IsPrimair: x.IsPrimair,
                                Adres: x.Adres.ToAdresString(),
                                Naam: x.Naam,
                                Postcode: x.Adres.Postcode,
                                Gemeente: x.Adres.Gemeente
                            ))
                            .ToArray()
                    ),
                ]
            );

            var expected = JsonConvert.SerializeObject(value: dubbeleVerenigingenWerdenGedetecteerd);
            savedEvents
                .Should()
                .ContainEquivalentOf(
                    expectation: dubbeleVerenigingenWerdenGedetecteerd,
                    config: options => options.Excluding(expression: e => e.Bevestigingstoken)
                );
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

            var requestAsJson = JsonConvert.SerializeObject(value: Request);

            Response = fixture
                .DefaultClient.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(content: requestAsJson)
                .GetAwaiter()
                .GetResult();
        }

        public ToeTeVoegenLocatie RequestLocatie { get; }
    }
}
