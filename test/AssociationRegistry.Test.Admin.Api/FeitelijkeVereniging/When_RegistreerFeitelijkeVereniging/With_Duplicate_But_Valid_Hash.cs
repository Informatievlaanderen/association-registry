namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
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
            .QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
            .ToListAsync();

        savedEvents.Should().ContainEquivalentOf(
            new FeitelijkeVerenigingWerdGeregistreerd(
                string.Empty,
                _setup.Request.Naam,
                _setup.Request.KorteNaam ?? string.Empty,
                _setup.Request.KorteBeschrijving ?? string.Empty,
                _setup.Request.Startdatum,
                _setup.Request.IsUitgeschrevenUitPubliekeDatastroom,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[]
                {
                    new Registratiedata.Locatie(
                        1,
                        _setup.RequestLocatie.Naam ?? string.Empty,
                        new Registratiedata.Adres(_setup.RequestLocatie.Straatnaam,
                        _setup.RequestLocatie.Huisnummer,
                        _setup.RequestLocatie.Busnummer ?? string.Empty,
                        _setup.RequestLocatie.Postcode,
                        _setup.RequestLocatie.Gemeente,
                        _setup.RequestLocatie.Land),
                        _setup.RequestLocatie.Hoofdlocatie,
                        _setup.RequestLocatie.Locatietype),
                },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()
            ),
            options => options.Excluding(e => e.VCode));
    }

    public sealed class Setup
    {
        public readonly RegistreerFeitelijkeVerenigingRequest Request;
        public readonly HttpResponseMessage Response;

        public Setup(EventsInDbScenariosFixture fixture)
        {
            var autoFixture = new Fixture().CustomizeAll();
            RequestLocatie = autoFixture.Create<ToeTeVoegenLocatie>();

            RequestLocatie.Gemeente = fixture.V009FeitelijkeVerenigingWerdGeregistreerdForDuplicateForce.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().Adres.Gemeente;
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
            Response = fixture.DefaultClient.RegistreerFeitelijkeVereniging(requestAsJson, bevestigingsTokenHelper.Calculate(Request)).GetAwaiter().GetResult();
        }

        public ToeTeVoegenLocatie RequestLocatie { get; }
    }
}
