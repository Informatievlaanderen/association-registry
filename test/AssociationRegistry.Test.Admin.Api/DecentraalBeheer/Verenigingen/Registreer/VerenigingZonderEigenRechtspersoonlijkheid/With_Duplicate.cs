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
                vCode: string.Empty,
                naam: _setup.Request.Naam,
                korteNaam: _setup.Request.KorteNaam ?? string.Empty,
                korteBeschrijving: _setup.Request.KorteBeschrijving ?? string.Empty,
                startdatum: _setup.Request.Startdatum,
                ddoelgroep: EventFactory.Doelgroep(Doelgroep.Null),
                isUitgeschrevenUitPubliekeDatastroom: _setup.Request.IsUitgeschrevenUitPubliekeDatastroom,
                contactgegevens: [],
                locaties:
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
                vertegenwoordigers: [],
                hoofdactiviteitenVerenigingsloket: []
            ),
            config: options => options.Excluding(e => e.VCode));
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






