namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Events;
using Events.CommonEventDataTypes;
using Fixtures;
using Framework;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_As_Duplicate_But_With_Valid_Hash
{
    public readonly RegistreerVerenigingRequest Request;
    public readonly HttpResponseMessage Response;
    public RegistreerVerenigingRequest.Locatie RequestLocatie { get; }

    public When_RegistreerVereniging_As_Duplicate_But_With_Valid_Hash(EventsInDbScenariosFixture fixture)
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

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Duplicate_But_Valid_Hash : IClassFixture<When_RegistreerVereniging_As_Duplicate_But_With_Valid_Hash>
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly When_RegistreerVereniging_As_Duplicate_But_With_Valid_Hash _classFixture;

    public With_Duplicate_But_Valid_Hash(EventsInDbScenariosFixture fixture, When_RegistreerVereniging_As_Duplicate_But_With_Valid_Hash classFixture)
    {
        _fixture = fixture;
        _classFixture = classFixture;
    }

    [Fact]
    public void Then_it_returns_an_accepted_response()
    {
        _classFixture.Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
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
                _classFixture.Request.Naam,
                _classFixture.Request.KorteNaam,
                _classFixture.Request.KorteBeschrijving,
                _classFixture.Request.Startdatum.HasValue ? _classFixture.Request.Startdatum.Value : null,
                _classFixture.Request.KboNummer,
                Array.Empty<ContactInfo>(),
                new[]
                {
                    new VerenigingWerdGeregistreerd.Locatie(
                        _classFixture.RequestLocatie.Naam,
                        _classFixture.RequestLocatie.Straatnaam,
                        _classFixture.RequestLocatie.Huisnummer,
                        _classFixture.RequestLocatie.Busnummer,
                        _classFixture.RequestLocatie.Postcode,
                        _classFixture.RequestLocatie.Gemeente,
                        _classFixture.RequestLocatie.Land,
                        _classFixture.RequestLocatie.Hoofdlocatie,
                        _classFixture.RequestLocatie.Locatietype),
                },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()
            ),
            options => options.Excluding(e => e.VCode));
    }
}
