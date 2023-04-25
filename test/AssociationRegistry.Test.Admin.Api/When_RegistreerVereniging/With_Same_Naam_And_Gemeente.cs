namespace AssociationRegistry.Test.Admin.Api.When_RegistreerVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerVereniging_With_Same_Naam_And_Gemeente
{
    private static When_RegistreerVereniging_With_Same_Naam_And_Gemeente? called;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public readonly string Naam;
    public readonly RegistreerVerenigingRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly string VCode;
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;


    private When_RegistreerVereniging_With_Same_Naam_And_Gemeente(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();
        var locatie = autoFixture.Create<ToeTeVoegenLocatie>();

        locatie.Gemeente = fixture.V001VerenigingWerdGeregistreerdWithAllFields.VerenigingWerdGeregistreerd.Locaties.First().Gemeente;
        Request = new RegistreerVerenigingRequest
        {
            Naam = fixture.V001VerenigingWerdGeregistreerdWithAllFields.VerenigingWerdGeregistreerd.Naam,
            Locaties = new[]
            {
                locatie,
            },
            Initiator = "OVO000001",
        };
        VCode = fixture.V001VerenigingWerdGeregistreerdWithAllFields.VCode;
        Naam = fixture.V001VerenigingWerdGeregistreerdWithAllFields.Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());
        RequestAsJson = JsonConvert.SerializeObject(
            Request);
        VerenigingWerdGeregistreerd = fixture.V001VerenigingWerdGeregistreerdWithAllFields.VerenigingWerdGeregistreerd;
        Response = fixture.DefaultClient.RegistreerVereniging(RequestAsJson).GetAwaiter().GetResult();
    }

    public string RequestAsJson { get; }

    public static When_RegistreerVereniging_With_Same_Naam_And_Gemeente Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerVereniging_With_Same_Naam_And_Gemeente(fixture);
}

//TODO: Rework to unit test
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Same_Naam_And_Gemeente
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Same_Naam_And_Gemeente(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private HttpResponseMessage Response
        => When_RegistreerVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).Response;

    private BevestigingsTokenHelper BevestigingsTokenHelper
        => When_RegistreerVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).BevestigingsTokenHelper;

    private string Naam
        => When_RegistreerVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).Naam;

    private VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd
        => When_RegistreerVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).VerenigingWerdGeregistreerd;

    private RegistreerVerenigingRequest Request
        => When_RegistreerVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).Request;

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
          ""hoofdlocatie"": {(locatie.Hoofdlocatie ? "true" : "false")},
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
        Response.Headers.Should().NotContainKey(HeaderNames.Location);
    }

    [Fact]
    public async Task Then_it_saves_no_extra_events()
    {
        using var session = _fixture.DocumentStore
            .LightweightSession();
        var savedEvents = await session.Events
            .QueryRawEventDataOnly<VerenigingWerdGeregistreerd>()
            .ToListAsync();

        savedEvents.Should().NotContainEquivalentOf(
            new VerenigingWerdGeregistreerd(
                string.Empty,
                Request.Naam,
                Request.KorteNaam ?? string.Empty,
                Request.KorteBeschrijving ?? string.Empty,
                Request.Startdatum,
                Request.KboNummer ?? string.Empty,
                Array.Empty<VerenigingWerdGeregistreerd.Contactgegeven>(),
                new[]
                {
                    new VerenigingWerdGeregistreerd.Locatie(
                        Request.Locaties.First().Naam ?? string.Empty,
                        Request.Locaties.First().Straatnaam,
                        Request.Locaties.First().Huisnummer,
                        Request.Locaties.First().Busnummer ?? string.Empty,
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
