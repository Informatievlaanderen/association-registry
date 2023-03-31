namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Fixtures;
using Framework;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;


[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Duplicate_But_Valid_Hash : IClassFixture<With_Duplicate_But_Valid_Hash.Setup>
{
    public sealed class Setup
    {
        public readonly RegistreerVerenigingRequest Request;
        public readonly HttpResponseMessage Response;
        public RegistreerVerenigingRequest.Locatie RequestLocatie { get; }

        public Setup(EventsInDbScenariosFixture fixture)
        {
            var autoFixture = new Fixture().CustomizeAll();
            RequestLocatie = autoFixture.Create<RegistreerVerenigingRequest.Locatie>();

            RequestLocatie.Gemeente = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Locaties.First().Gemeente;
            Request = new RegistreerVerenigingRequest
            {
                Naam = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Naam,
                Locaties = new[]
                {
                    RequestLocatie,
                },
                Initiator = "OVO000001",
            };
            var bevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());

            var requestAsJson = JsonConvert.SerializeObject(Request);
            Response = fixture.DefaultClient.RegistreerVereniging(requestAsJson, bevestigingsTokenHelper.Calculate(Request)).GetAwaiter().GetResult();
        }
    }

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
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .ToListAsync();

        savedEvents.Should().ContainEquivalentOf(
            new VerenigingWerdGeregistreerd(
                string.Empty,
                _setup.Request.Naam,
                _setup.Request.KorteNaam,
                _setup.Request.KorteBeschrijving,
                _setup.Request.Startdatum.HasValue ? _setup.Request.Startdatum.Value : null,
                _setup.Request.KboNummer,
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                new[]
                {
                    new VerenigingWerdGeregistreerd.Locatie(
                        _setup.RequestLocatie.Naam,
                        _setup.RequestLocatie.Straatnaam,
                        _setup.RequestLocatie.Huisnummer,
                        _setup.RequestLocatie.Busnummer,
                        _setup.RequestLocatie.Postcode,
                        _setup.RequestLocatie.Gemeente,
                        _setup.RequestLocatie.Land,
                        _setup.RequestLocatie.Hoofdlocatie,
                        _setup.RequestLocatie.Locatietype),
                },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()
            ),
            options => options.Excluding(e => e.VCode));
    }
}
