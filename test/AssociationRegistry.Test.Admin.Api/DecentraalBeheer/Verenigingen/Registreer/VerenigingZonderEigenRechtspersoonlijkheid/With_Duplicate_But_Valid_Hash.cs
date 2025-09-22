namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

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
using System.Net;
using Xunit;

[Collection(nameof(AdminApiCollection))]
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
        _setup.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async ValueTask Then_VerenigingWerdGeregistreerd()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var savedEvents = await session.Events
                                       .QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                       .ToListAsync();

        savedEvents.Should().ContainEquivalentOf(
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
                Registratiedata.DuplicatieInfo.BevestigdGeenDuplicaat(_setup.BevestigingsToken, string.Empty)            ),
            config: options => options.Excluding(e => e.VCode));
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

            var bevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());

            var requestAsJson = JsonConvert.SerializeObject(Request);

            BevestigingsToken = bevestigingsTokenHelper.Calculate(Request);

            Response = fixture.DefaultClient.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(requestAsJson, BevestigingsToken)
                              .GetAwaiter().GetResult();
        }

        public ToeTeVoegenLocatie RequestLocatie { get; }
    }
}
