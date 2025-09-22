namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid.RequestModels;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Formats;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AutoFixture;
using Events.Factories;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using Xunit;

public sealed class RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode
{
    private static RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode? called;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public readonly string Naam;
    public readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    private RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();
        var locatie = autoFixture.Create<ToeTeVoegenLocatie>();

        locatie.Adres!.Postcode = fixture.V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck
                                         .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First()
                                         .Adres!.Postcode;

        Request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            Naam = fixture.V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                          .Naam,
            Locaties = new[]
            {
                locatie,
            },
        };

        Naam = fixture.V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck.Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(fixture.ServiceProvider.GetRequiredService<AppSettings>());

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture.V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck
                                                                             .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

        Response = fixture.DefaultClient.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(JsonConvert.SerializeObject(Request)).GetAwaiter().GetResult();
    }

    public static RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode Called(EventsInDbScenariosFixture fixture)
        => called ??= new RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode(fixture);
}

//TODO: Rework to unit test
[Collection(nameof(AdminApiCollection))]
public class With_Same_Naam_And_Postcode
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest Request
        => RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode.Called(_fixture).Request;

    private BevestigingsTokenHelper BevestigingsTokenHelper
        => RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode.Called(_fixture).BevestigingsTokenHelper;

    private HttpResponseMessage Response
        => RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode.Called(_fixture).Response;

    private VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
        => RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode.Called(_fixture).VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    private string Naam
        => RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Postcode.Called(_fixture).Naam;

    private string ResponseBody
        => @$"{{
  ""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(Request)}"",
  ""mogelijkeDuplicateVerenigingen"": [
    {{
      ""vCode"": ""{VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}"",
        ""verenigingstype"": {{
            ""code"": ""{Verenigingstype.VZER.Code}"",
            ""naam"": ""{Verenigingstype.VZER.Naam}"",
        }},
        ""verenigingssubtype"": {{
        ""code"": ""{VerenigingssubtypeCode.Default.Code}"",
        ""naam"": ""{VerenigingssubtypeCode.Default.Naam}"",
      }},
      ""naam"": ""{VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Naam}"",
      ""korteNaam"": ""{VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.KorteNaam}"",
      ""hoofdactiviteitenVerenigingsloket"": [{string.Join(separator: ",",
                                                           VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.HoofdactiviteitenVerenigingsloket
                                                              .Select(hoofdactiviteit => $@"{{
          ""code"": ""{hoofdactiviteit.Code}"",
          ""naam"": ""{hoofdactiviteit.Naam}""
        }}"))}
      ],
      ""locaties"": [{string.Join(separator: ",",
                                  VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties
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
        ""detail"": ""http://127.0.0.1:11004/v1/verenigingen/{VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}""
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
    public async ValueTask Then_it_returns_the_list_of_potential_duplicates()
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
    public async ValueTask Then_it_saves_no_extra_events()
    {
        await using var session = _fixture.DocumentStore
                                          .LightweightSession();

        var savedEvents = await session.Events
                                       .QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                       .ToListAsync();

        savedEvents.Should().NotContainEquivalentOf(
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                string.Empty,
                Request.Naam,
                Request.KorteNaam ?? string.Empty,
                Request.KorteBeschrijving ?? string.Empty,
                Request.Startdatum,
                EventFactory.Doelgroep(Doelgroep.Null),
                Request.IsUitgeschrevenUitPubliekeDatastroom,
                Array.Empty<Registratiedata.Contactgegeven>(),
                new[]
                {
                    new Registratiedata.Locatie(
                        LocatieId: 1,
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
                        AdresId: null),
                },
                Array.Empty<Registratiedata.Vertegenwoordiger>(),
                Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
                false
            ),
            config: options => options.Excluding(e => e.VCode));
    }
}
