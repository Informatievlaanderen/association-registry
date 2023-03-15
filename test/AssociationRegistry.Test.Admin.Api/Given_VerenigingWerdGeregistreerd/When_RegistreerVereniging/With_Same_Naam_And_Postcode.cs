namespace AssociationRegistry.Test.Admin.Api.Given_VerenigingWerdGeregistreerd.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using Fixtures;
using Framework;
using AutoFixture;
using Events;
using Events.CommonEventDataTypes;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Same_Naam_And_Postcode
{
    public readonly string Naam;
    public readonly RegistreerVerenigingRequest Request;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly HttpResponseMessage Response;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;

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
        BevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());
        VerenigingWerdGeregistreerd = fixture.VerenigingWerdGeregistreerdWithAllFieldsEventsInDbScenario.VerenigingWerdGeregistreerd;
        Response = fixture.DefaultClient.RegistreerVereniging(JsonConvert.SerializeObject(Request)).GetAwaiter().GetResult();
    }

    private static When_RegistreerVereniging_With_Same_Naam_And_Postcode? called;

    public static When_RegistreerVereniging_With_Same_Naam_And_Postcode Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerVereniging_With_Same_Naam_And_Postcode(fixture);
}

//TODO: Rework to unit test
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Same_Naam_And_Postcode
{
    private readonly EventsInDbScenariosFixture _fixture;

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Request;

    private BevestigingsTokenHelper BevestigingsTokenHelper
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).BevestigingsTokenHelper;

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Response;

    private VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).VerenigingWerdGeregistreerd;

    private string Naam
        => When_RegistreerVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Naam;

    private string ResponseBody
        => @$"{{
  ""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(Request)}"",
  ""mogelijkeDuplicateVerenigingen"": [
    {{
      ""vCode"": ""{VerenigingWerdGeregistreerd.VCode}"",
      ""naam"": ""{VerenigingWerdGeregistreerd.Naam}"",
      ""korteNaam"": ""{VerenigingWerdGeregistreerd.KorteNaam}"",
      ""hoofdactiviteitenVerenigingsloket"": [{string.Join(",",
          VerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket
              .Select(hoofdactiviteit => $@"{{
          ""code"": ""{hoofdactiviteit.Code}"",
          ""beschrijving"": ""{hoofdactiviteit.Beschrijving}""
        }}"))}
      ],
      ""doelgroep"": """",
      ""locaties"": [{string.Join(",",
          VerenigingWerdGeregistreerd.Locaties
              .Select(locatie => $@"{{
          ""locatietype"": ""{locatie.Locatietype}"",
          ""adres"": ""{locatie.ToAdresString()}"",
          ""naam"": ""{locatie.Naam}"",
          ""postcode"": ""{locatie.Postcode}"",
          ""gemeente"": ""{locatie.Gemeente}""
        }}"))}
      ],
      ""activiteiten"": [],
      ""links"": {{
        ""detail"": ""http://127.0.0.1:11004/v1/verenigingen/{VerenigingWerdGeregistreerd.VCode}""
      }}
    }}
  ]
}}";

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
                Request.Startdatum.HasValue ? Request.Startdatum.Value : null,
                Request.KboNummer,
                Array.Empty<ContactInfo>(),
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
