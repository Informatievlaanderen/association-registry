namespace AssociationRegistry.Test.Admin.Api.FeitelijkeVereniging.When_RegistreerFeitelijkeVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.FeitelijkeVereniging;
using Events;
using Fixtures;
using Framework;
using Vereniging;
using AutoFixture;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente
{
    private static When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente? called;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public readonly string Naam;
    public readonly RegistreerFeitelijkeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;


    private When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();
        var locatie = autoFixture.Create<ToeTeVoegenLocatie>();

        locatie.Adres!.Gemeente = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().Adres.Gemeente;
        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd.Naam,
            Locaties = new[]
            {
                locatie,
            },
        };
        Naam = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());
        RequestAsJson = JsonConvert.SerializeObject(Request);
        FeitelijkeVerenigingWerdGeregistreerd = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd;
        Response = fixture.DefaultClient.RegistreerFeitelijkeVereniging(RequestAsJson).GetAwaiter().GetResult();
    }

    public string RequestAsJson { get; }

    public static When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente(fixture);
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
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).Response;

    private BevestigingsTokenHelper BevestigingsTokenHelper
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).BevestigingsTokenHelper;

    private string Naam
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).Naam;

    private FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).FeitelijkeVerenigingWerdGeregistreerd;

    private RegistreerFeitelijkeVerenigingRequest Request
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Gemeente.Called(_fixture).Request;

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
      ""doelgroep"": """",
      ""locaties"": [{string.Join(",",
          FeitelijkeVerenigingWerdGeregistreerd.Locaties
              .Select(locatie => $@"{{
          ""locatietype"": ""{locatie.Locatietype}"",
          ""hoofdlocatie"": {(locatie.Hoofdlocatie ? "true" : "false")},
          ""adres"": ""{locatie.ToAdresString()}"",
          ""naam"": ""{locatie.Naam}"",
          ""postcode"": ""{locatie.Adres.Postcode}"",
          ""gemeente"": ""{locatie.Adres.Gemeente}""
        }}"))}
      ],
      ""activiteiten"": [],
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
            new FeitelijkeVerenigingWerdGeregistreerd(
                string.Empty,
                Request.Naam,
                Request.KorteNaam ?? string.Empty,
                Request.KorteBeschrijving ?? string.Empty,
                Request.Startdatum,
                Request.IsUitgeschrevenUitPubliekeDatastroom,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[]
                {
                    new Registratiedata.Locatie(
                        1,
                        Request.Locaties.First().Naam ?? string.Empty,
                        new Registratiedata.Adres(
                            Request.Locaties.First().Adres!.Straatnaam,
                            Request.Locaties.First().Adres!.Huisnummer,
                            Request.Locaties.First().Adres!.Busnummer ?? string.Empty,
                            Request.Locaties.First().Adres!.Postcode,
                            Request.Locaties.First().Adres!.Gemeente,
                            Request.Locaties.First().Adres!.Land),
                        Request.Locaties.First().Hoofdlocatie,
                        Request.Locaties.First().Locatietype),
                },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()
            ),
            options => options.Excluding(e => e.VCode));
    }
}
