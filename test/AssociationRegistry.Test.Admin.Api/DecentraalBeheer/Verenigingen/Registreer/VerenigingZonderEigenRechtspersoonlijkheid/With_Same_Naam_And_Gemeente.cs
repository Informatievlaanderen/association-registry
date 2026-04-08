namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.VerenigingZonderEigenRechtspersoonlijkheid;

using System.Net;
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
using Xunit;

public sealed class RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente
{
    private static RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente? called;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public readonly string Naam;
    public readonly RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    private RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente(
        EventsInDbScenariosFixture fixture
    )
    {
        var autoFixture = new Fixture().CustomizeAdminApi();
        var locatie = autoFixture.Create<ToeTeVoegenLocatie>();

        locatie.Adres!.Gemeente = fixture
            .V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties.First()
            .Adres!.Gemeente;

        Request = new RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest
        {
            Naam = fixture
                .V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck
                .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
                .Naam,
            Locaties = [locatie],
        };

        Naam = fixture
            .V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck
            .Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(
            appSettings: fixture.ServiceProvider.GetRequiredService<AppSettings>()
        );
        RequestAsJson = JsonConvert.SerializeObject(value: Request);

        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd = fixture
            .V083VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAllFieldsForDuplicateCheck
            .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

        Response = fixture
            .DefaultClient.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(content: RequestAsJson)
            .GetAwaiter()
            .GetResult();
    }

    public string RequestAsJson { get; }

    public static RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente Called(
        EventsInDbScenariosFixture fixture
    ) =>
        called ??= new RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente(
            fixture: fixture
        );
}

//TODO: Rework to unit test
[Collection(name: nameof(AdminApiCollection))]
public class With_Same_Naam_And_Gemeente
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Same_Naam_And_Gemeente(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private HttpResponseMessage Response =>
        RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente
            .Called(fixture: _fixture)
            .Response;

    private BevestigingsTokenHelper BevestigingsTokenHelper =>
        RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente
            .Called(fixture: _fixture)
            .BevestigingsTokenHelper;

    private string Naam =>
        RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente.Called(fixture: _fixture).Naam;

    private VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd =>
        RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente
            .Called(fixture: _fixture)
            .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd;

    private RegistreerVerenigingZonderEigenRechtspersoonlijkheidRequest Request =>
        RegistreerVerenigingZonderEigenRechtspersoonlijkheid_With_Same_Naam_And_Gemeente
            .Called(fixture: _fixture)
            .Request;

    private string ResponseBody =>
        @$"{{
  ""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(request: Request)}"",
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
                                                           values: VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.HoofdactiviteitenVerenigingsloket
                                                                                                                              .Select(selector: hoofdactiviteit => $@"{{
          ""code"": ""{hoofdactiviteit.Code}"",
          ""naam"": ""{hoofdactiviteit.Naam}""
        }}"))}
      ],
      ""locaties"": [{string.Join(separator: ",",
                                  values: VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Locaties
                                                                                                     .Select(selector: locatie => $@"{{
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
        Response.StatusCode.Should().Be(expected: HttpStatusCode.Conflict);
    }

    [Fact]
    public async ValueTask Then_it_returns_the_list_of_potential_duplicates()
    {
        var content = await Response.Content.ReadAsStringAsync();
        content.Should().BeEquivalentJson(json: ResponseBody);
    }

    [Fact]
    public void Then_it_returns_no_sequence_header()
    {
        Response.Headers.Should().NotContainKey(unexpected: WellknownHeaderNames.Sequence);
    }

    [Fact]
    public void Then_it_returns_no_location_header()
    {
        Response.Headers.Should().NotContainKey(unexpected: HeaderNames.Location);
    }

    [Fact]
    public async ValueTask Then_it_saves_no_extra_events()
    {
        using var session = _fixture.DocumentStore.LightweightSession();

        var savedEvents = await session
            .Events.QueryRawEventDataOnly<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
            .ToListAsync();

        savedEvents
            .Should()
            .NotContainEquivalentOf(
                unexpected: new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd(
                    VCode: string.Empty,
                    Naam: Request.Naam,
                    KorteNaam: Request.KorteNaam ?? string.Empty,
                    KorteBeschrijving: Request.KorteBeschrijving ?? string.Empty,
                    Startdatum: Request.Startdatum,
                    Doelgroep: EventFactory.Doelgroep(doelgroep: Doelgroep.Null),
                    IsUitgeschrevenUitPubliekeDatastroom: Request.IsUitgeschrevenUitPubliekeDatastroom,
                    Contactgegevens: [],
                    Locaties:
                    [
                        new Registratiedata.Locatie(
                            LocatieId: 1,
                            Locatietype: Request.Locaties.First().Locatietype,
                            IsPrimair: Request.Locaties.First().IsPrimair,
                            Naam: Request.Locaties.First().Naam ?? string.Empty,
                            Adres: new Registratiedata.Adres(
                                Straatnaam: Request.Locaties.First().Adres!.Straatnaam,
                                Huisnummer: Request.Locaties.First().Adres!.Huisnummer,
                                Busnummer: Request.Locaties.First().Adres!.Busnummer ?? string.Empty,
                                Postcode: Request.Locaties.First().Adres!.Postcode,
                                Gemeente: Request.Locaties.First().Adres!.Gemeente,
                                Land: Request.Locaties.First().Adres!.Land
                            ),
                            AdresId: null
                        ),
                    ],
                    Vertegenwoordigers: [],
                    HoofdactiviteitenVerenigingsloket: [],
                    Bankrekeningnummers: [],
                    DuplicatieInfo: Registratiedata.DuplicatieInfo.GeenDuplicaten
                ),
                config: options => options.Excluding(expression: e => e.VCode)
            );
    }
}
