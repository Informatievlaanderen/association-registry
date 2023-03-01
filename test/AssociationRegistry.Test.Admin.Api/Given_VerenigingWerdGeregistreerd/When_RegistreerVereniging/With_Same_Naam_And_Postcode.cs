namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Fixtures;
using Framework;
using AutoFixture;
using Events;
using FluentAssertions;
using Marten;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Same_Naam_And_Postcode
{
    public readonly string Naam;
    public readonly RegistreerVerenigingRequest Request;
    public readonly HttpResponseMessage Response;

    private When_RegistreerVereniging_With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();
        var locatie = autoFixture.Create<RegistreerVerenigingRequest.Locatie>();

        locatie.Postcode = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Locaties.First().Postcode;
        Request = new RegistreerVerenigingRequest()
        {
            Naam = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd.Naam,
            Locaties = new[]
            {
                locatie,
            },
            Initiator = "OVO000001",
        };
        Naam = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.Naam;

        Response = fixture.DefaultClient.RegistreerVereniging(JsonConvert.SerializeObject(Request)).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_With_Same_Naam_And_Postcode? called;

    public static When_RegistreerVereniging_With_Same_Naam_And_Postcode Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerVereniging_With_Same_Naam_And_Postcode(fixture);
}

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Same_Naam_And_Postcode
{
    private readonly EventsInDbScenariosFixture _fixture;

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Request;

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Response;

    private string Naam
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Naam;

    private string ResponseBody
        => @$"{{""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(Request)}"", ""duplicaten"":[{{""vCode"":""V0001001"",""naam"":""{Naam}""}}]}}";

    public With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Then_it_returns_a_conflict_response()
    {
        Response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Then_it_returns_the_list_of_potential_duplicates()
    {
        var content = await Response.Content.ReadAsStringAsync();
        content.Should().BeEquivalentJson(ResponseBody);
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        Response.Headers.Should().NotContainKey(WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_returns_no_location_header()
    {
        Response.Headers.Should().NotContainKey(Microsoft.Net.Http.Headers.HeaderNames.Location);
    }

    [Fact]
    public async Task Then_it_saves_no_extra_events()
    {
        await using var session = _fixture.DocumentStore
            .LightweightSession();
        var savedEvents = await session.Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .ToListAsync();

        savedEvents.Should().NotContainEquivalentOf(
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
