namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging;

using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
using AssociationRegistry.EventFactories;
using AssociationRegistry.Events;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
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
    public async Task Then_it_saves_an_extra_event()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var savedEvents = await session.Events
                                       .QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                       .ToListAsync();

        savedEvents.Should().ContainEquivalentOf(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                string.Empty,
                _setup.Request.Naam,
                _setup.Request.KorteNaam ?? string.Empty,
                _setup.Request.KorteBeschrijving ?? string.Empty,
                _setup.Request.Startdatum,
                EventFactory.Doelgroep(Doelgroep.Null),
                _setup.Request.IsUitgeschrevenUitPubliekeDatastroom,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[]
                {
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
                },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()
            ),
            config: options => options.Excluding(e => e.VCode));
    }

    public sealed class Setup
    {
        public readonly RegistreerFeitelijkeVerenigingRequest Request;
        public readonly HttpResponseMessage Response;

        public Setup(EventsInDbScenariosFixture fixture)
        {
            var autoFixture = new Fixture().CustomizeAdminApi();
            RequestLocatie = autoFixture.Create<ToeTeVoegenLocatie>();

            RequestLocatie.Adres!.Gemeente = fixture.V009FeitelijkeVerenigingWerdGeregistreerdForDuplicateForce
                                                    .FeitelijkeVerenigingWerdGeregistreerd.Locaties.First()
                                                    .Adres!.Gemeente;

            Request = new RegistreerFeitelijkeVerenigingRequest
            {
                Naam = fixture.V009FeitelijkeVerenigingWerdGeregistreerdForDuplicateForce.FeitelijkeVerenigingWerdGeregistreerd.Naam,
                Locaties = new[]
                {
                    RequestLocatie,
                },
            };

            var bevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());

            var requestAsJson = JsonConvert.SerializeObject(Request);

            Response = fixture.DefaultClient.RegistreerFeitelijkeVereniging(requestAsJson, bevestigingsTokenHelper.Calculate(Request))
                              .GetAwaiter().GetResult();
        }

        public ToeTeVoegenLocatie RequestLocatie { get; }
    }
}
