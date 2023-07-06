namespace AssociationRegistry.Test.Admin.Api.Afdeling.When_RegistreerAfdeling;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.Afdeling;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Formatters;
using Framework;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Vereniging;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerAfdeling_With_Same_Naam_And_Gemeente
{
    private static When_RegistreerAfdeling_With_Same_Naam_And_Gemeente? called;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public readonly string Naam;
    public readonly RegistreerAfdelingRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;


    private When_RegistreerAfdeling_With_Same_Naam_And_Gemeente(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();
        var locatie = autoFixture.Create<ToeTeVoegenLocatie>();

        locatie.Adres!.Gemeente = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck
            .FeitelijkeVerenigingWerdGeregistreerd.Locaties.First()
            .Adres!.Gemeente;
        Request = new RegistreerAfdelingRequest
        {
            Naam = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd.Naam,
            KboNummerMoedervereniging = autoFixture.Create<KboNummer>(),
            Locaties = new[]
            {
                locatie,
            },
        };
        Naam = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());
        RequestAsJson = JsonConvert.SerializeObject(Request);
        FeitelijkeVerenigingWerdGeregistreerd = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd;
        Response = fixture.DefaultClient.RegistreerAfdeling(RequestAsJson).GetAwaiter().GetResult();
    }

    public string RequestAsJson { get; }

    public static When_RegistreerAfdeling_With_Same_Naam_And_Gemeente Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerAfdeling_With_Same_Naam_And_Gemeente(fixture);
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
        => When_RegistreerAfdeling_With_Same_Naam_And_Gemeente.Called(_fixture).Response;

    private BevestigingsTokenHelper BevestigingsTokenHelper
        => When_RegistreerAfdeling_With_Same_Naam_And_Gemeente.Called(_fixture).BevestigingsTokenHelper;

    private string Naam
        => When_RegistreerAfdeling_With_Same_Naam_And_Gemeente.Called(_fixture).Naam;

    private FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd
        => When_RegistreerAfdeling_With_Same_Naam_And_Gemeente.Called(_fixture).FeitelijkeVerenigingWerdGeregistreerd;

    private RegistreerAfdelingRequest Request
        => When_RegistreerAfdeling_With_Same_Naam_And_Gemeente.Called(_fixture).Request;

    private string ResponseBody
        => @$"{{
  ""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(Request)}"",
  ""mogelijkeDuplicateVerenigingen"": [
    {{
      ""vCode"": ""{FeitelijkeVerenigingWerdGeregistreerd.VCode}"",
      ""type"": {{
        ""code"": ""{Verenigingstype.FeitelijkeVereniging.Code}"",
        ""beschrijving"": ""{Verenigingstype.FeitelijkeVereniging.Beschrijving}"",
      }},
      ""naam"": ""{FeitelijkeVerenigingWerdGeregistreerd.Naam}"",
      ""korteNaam"": ""{FeitelijkeVerenigingWerdGeregistreerd.KorteNaam}"",
      ""hoofdactiviteitenVerenigingsloket"": [{string.Join(",",
          FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket
              .Select(hoofdactiviteit => $@"{{
          ""code"": ""{hoofdactiviteit.Code}"",
          ""beschrijving"": ""{hoofdactiviteit.Beschrijving}""
        }}"))}
      ],
      ""locaties"": [{string.Join(",",
          FeitelijkeVerenigingWerdGeregistreerd.Locaties
              .Select(locatie => $@"{{
          ""locatietype"": ""{locatie.Locatietype}"",
          ""isPrimair"": {(locatie.IsPrimair ? "true" : "false")},
          ""adresvoorstelling"": ""{locatie.Adres.ToAdresString()}"",
          ""naam"": ""{locatie.Naam}"",
          ""postcode"": ""{locatie.Adres?.Postcode ?? string.Empty}"",
          ""gemeente"": ""{locatie.Adres?.Gemeente ?? string.Empty}""
        }}"))}
      ],
      ""links"": {{
        ""detail"": ""http://127.0.0.1:11004/v1/verenigingen/{FeitelijkeVerenigingWerdGeregistreerd.VCode}""
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
            .QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerd>()
            .ToListAsync();

        savedEvents.Should().NotContainEquivalentOf(
            new AfdelingWerdGeregistreerd(
                string.Empty,
                Request.Naam,
                new AfdelingWerdGeregistreerd.MoederverenigingsData(
                    Request.KboNummerMoedervereniging,
                    string.Empty,
                    $"Moeder {Request.KboNummerMoedervereniging}"),
                Request.KorteNaam ?? string.Empty,
                Request.KorteBeschrijving ?? string.Empty,
                Request.Startdatum,
                new Registratiedata.Doelgroep(
                    Doelgroep.StandaardMinimumleeftijd,
                    Doelgroep.StandaardMaximumleeftijd),
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[]
                {
                    new Registratiedata.Locatie(
                        1,
                        Request.Locaties.First().Locatietype,
                        Request.Locaties.First().IsPrimair,
                        Request.Locaties.First().Naam ?? string.Empty,
                        new Registratiedata.Adres(
                            Request.Locaties.First().Adres!.Straatnaam,
                            Request.Locaties.First().Adres!.Huisnummer,
                            Request.Locaties.First().Adres!.Busnummer ?? string.Empty,
                            Request.Locaties.First().Adres!.Postcode,
                            Request.Locaties.First().Adres!.Gemeente,
                            Request.Locaties.First().Adres!.Land),
                        null),
                },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()
            ),
            options => options.Excluding(e => e.VCode));
    }
}
