namespace AssociationRegistry.Test.Admin.Api.When_RegistreerFeitelijkeVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Verenigingen;
using AssociationRegistry.Admin.Api.Verenigingen.Common;
using AssociationRegistry.Admin.Api.Verenigingen.Registreer.DecentraalBeheerdeVereniging;
using AutoFixture;
using Events;
using Fixtures;
using FluentAssertions;
using Framework;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Vereniging;
using Xunit;
using Xunit.Categories;

public sealed class When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode
{
    private static When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode? called;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public readonly string Naam;
    public readonly RegistreerDecentraalBeheerdeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    private When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAll();
        var locatie = autoFixture.Create<ToeTeVoegenLocatie>();

        locatie.Postcode = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First().Postcode;
        Request = new RegistreerDecentraalBeheerdeVerenigingRequest
        {
            Naam = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd.Naam,
            Locaties = new[]
            {
                locatie,
            },
        };
        Naam = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());
        FeitelijkeVerenigingWerdGeregistreerd = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd;
        Response = fixture.DefaultClient.RegistreerFeitelijkeVereniging(JsonConvert.SerializeObject(Request)).GetAwaiter().GetResult();
    }

    public static When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode Called(EventsInDbScenariosFixture fixture)
        => called ??= new When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode(fixture);
}

//TODO: Rework to unit test
[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class With_Same_Naam_And_Postcode
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerDecentraalBeheerdeVerenigingRequest Request
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Request;

    private BevestigingsTokenHelper BevestigingsTokenHelper
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(_fixture).BevestigingsTokenHelper;

    private HttpResponseMessage Response
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Response;

    private FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(_fixture).FeitelijkeVerenigingWerdGeregistreerd;

    private string Naam
        => When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(_fixture).Naam;

    private string ResponseBody
        => @$"{{
  ""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(Request)}"",
  ""mogelijkeDuplicateVerenigingen"": [
    {{
      ""vCode"": ""{FeitelijkeVerenigingWerdGeregistreerd.VCode}"",
        ""type"": {{
            ""code"": ""{VerenigingsType.FeitelijkeVereniging.Code}"",
            ""beschrijving"": ""{VerenigingsType.FeitelijkeVereniging.Beschrijving}"",
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
          ""postcode"": ""{locatie.Postcode}"",
          ""gemeente"": ""{locatie.Gemeente}""
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
        await using var session = _fixture.DocumentStore
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
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Contactgegeven>(),
                new[]
                {
                    new FeitelijkeVerenigingWerdGeregistreerd.Locatie(
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
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.Vertegenwoordiger>(),
                Array.Empty<FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitVerenigingsloket>()
            ),
            options => options.Excluding(e => e.VCode));
    }
}
