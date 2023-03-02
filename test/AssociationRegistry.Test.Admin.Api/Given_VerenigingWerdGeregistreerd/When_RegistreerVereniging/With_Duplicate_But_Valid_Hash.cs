namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Duplicate_But_Valid_Hash
{
    public readonly string VCode;
    public readonly string Naam;
    public readonly RegistreerVerenigingRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public string RequestAsJson { get; }


    private When_RegistreerVereniging_With_Duplicate_But_Valid_Hash(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();
        var locatie = autoFixture.Create<RegistreerVerenigingRequest.Locatie>();

        locatie.Gemeente = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Locaties.First().Gemeente;
        Request = new RegistreerVerenigingRequest()
        {
            Naam = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Naam,
            Locaties = new[]
            {
                locatie,
            },
            Initiator = "OVO000001",
        };
        VCode = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VCode;
        Naam = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());

        RequestAsJson = JsonConvert.SerializeObject(Request);
        Response = fixture.DefaultClient.RegistreerVereniging(RequestAsJson, BevestigingsTokenHelper.Calculate(Request)).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_With_Duplicate_But_Valid_Hash? called;

    public static When_RegistreerVereniging_With_Duplicate_But_Valid_Hash Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerVereniging_With_Duplicate_But_Valid_Hash(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Duplicate_But_Valid_Hash
{
    private readonly EventsInDbScenariosFixture _fixture;

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).Response;
    private BevestigingsTokenHelper BevestigingsTokenHelper
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).BevestigingsTokenHelper;

    private string VCode
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).VCode;

    private string Naam
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).Naam;

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_With_Duplicate_But_Valid_Hash.Called(_fixture).Request;

    public When_Duplicate_But_Valid_Hash(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_a_conflict_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Accepted);
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
                Request.Naam,
                Request.KorteNaam,
                Request.KorteBeschrijving,
                Request.StartDatum,
                Request.KboNummer,
                Array.Empty<VerenigingWerdGeregistreerd.ContactInfo>(),
                new[]
                {
                    new VerenigingWerdGeregistreerd.Locatie(
                        Request.Locaties.First().Naam,
                        Request.Locaties.First().Straatnaam,
                        Request.Locaties.First().Huisnummer,
                        Request.Locaties.First().Busnummer,
                        Request.Locaties.First().Postcode,
                        Request.Locaties.First().Gemeente,
                        Request.Locaties.First().Land,
                        Request.Locaties.First().Hoofdlocatie,
                        Request.Locaties.First().Locatietype),
                },
                Array.Empty<VerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<VerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()
            ),
            options => options.Excluding(e => e.VCode));
    }
}
