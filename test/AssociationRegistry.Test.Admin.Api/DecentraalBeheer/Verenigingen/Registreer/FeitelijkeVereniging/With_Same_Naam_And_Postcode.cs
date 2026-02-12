namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging;

using System.Net;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Common;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;
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

public sealed class When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode
{
    private static When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode? called;
    public readonly BevestigingsTokenHelper BevestigingsTokenHelper;
    public readonly string Naam;
    public readonly RegistreerFeitelijkeVerenigingRequest Request;
    public readonly HttpResponseMessage Response;
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;

    private When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        var autoFixture = new Fixture().CustomizeAdminApi();
        var locatie = autoFixture.Create<ToeTeVoegenLocatie>();

        locatie.Adres!.Postcode = fixture
            .V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.FeitelijkeVerenigingWerdGeregistreerd.Locaties.First()
            .Adres!.Postcode;

        Request = new RegistreerFeitelijkeVerenigingRequest
        {
            Naam = fixture
                .V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck
                .FeitelijkeVerenigingWerdGeregistreerd
                .Naam,
            Locaties = [locatie],
        };

        Naam = fixture.V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck.Naam;
        BevestigingsTokenHelper = new BevestigingsTokenHelper(
            appSettings: fixture.ServiceProvider.GetRequiredService<AppSettings>()
        );

        FeitelijkeVerenigingWerdGeregistreerd = fixture
            .V013FeitelijkeVerenigingWerdGeregistreerdWithAllFieldsForDuplicateCheck
            .FeitelijkeVerenigingWerdGeregistreerd;

        Response = fixture
            .DefaultClient.RegistreerFeitelijkeVereniging(content: JsonConvert.SerializeObject(value: Request))
            .GetAwaiter()
            .GetResult();
    }

    public static When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode Called(
        EventsInDbScenariosFixture fixture
    ) => called ??= new When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode(fixture: fixture);
}

//TODO: Rework to unit test
[Collection(name: nameof(AdminApiCollection))]
public class With_Same_Naam_And_Postcode
{
    private readonly EventsInDbScenariosFixture _fixture;

    public With_Same_Naam_And_Postcode(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
    }

    private RegistreerFeitelijkeVerenigingRequest Request =>
        When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(fixture: _fixture).Request;

    private BevestigingsTokenHelper BevestigingsTokenHelper =>
        When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode
            .Called(fixture: _fixture)
            .BevestigingsTokenHelper;

    private HttpResponseMessage Response =>
        When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(fixture: _fixture).Response;

    private FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd =>
        When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode
            .Called(fixture: _fixture)
            .FeitelijkeVerenigingWerdGeregistreerd;

    private string Naam =>
        When_RegistreerFeitelijkeVereniging_With_Same_Naam_And_Postcode.Called(fixture: _fixture).Naam;

    private string ResponseBody =>
        @$"{{
  ""bevestigingsToken"": ""{BevestigingsTokenHelper.Calculate(request: Request)}"",
  ""mogelijkeDuplicateVerenigingen"": [
    {{
      ""vCode"": ""{FeitelijkeVerenigingWerdGeregistreerd.VCode}"",
        ""verenigingstype"": {{
            ""code"": ""{Verenigingstype.FeitelijkeVereniging.Code}"",
            ""naam"": ""{Verenigingstype.FeitelijkeVereniging.Naam}"",
        }},
      ""naam"": ""{FeitelijkeVerenigingWerdGeregistreerd.Naam}"",
      ""korteNaam"": ""{FeitelijkeVerenigingWerdGeregistreerd.KorteNaam}"",
      ""hoofdactiviteitenVerenigingsloket"": [{string.Join(separator: ",",
                                                           values: FeitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket
                                                                                                        .Select(selector: hoofdactiviteit => $@"{{
          ""code"": ""{hoofdactiviteit.Code}"",
          ""naam"": ""{hoofdactiviteit.Naam}""
        }}"))}
      ],
      ""locaties"": [{string.Join(separator: ",",
                                  values: FeitelijkeVerenigingWerdGeregistreerd.Locaties
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
        ""detail"": ""http://127.0.0.1:11004/v1/verenigingen/{FeitelijkeVerenigingWerdGeregistreerd.VCode}""
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
        await using var session = _fixture.DocumentStore.LightweightSession();

        var savedEvents = await session
            .Events.QueryRawEventDataOnly<FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens>()
            .ToListAsync();

        savedEvents
            .Should()
            .NotContainEquivalentOf(
                unexpected: new FeitelijkeVerenigingWerdGeregistreerdZonderPersoonsgegevens(
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
                    HoofdactiviteitenVerenigingsloket: []
                ),
                config: options => options.Excluding(expression: e => e.VCode)
            );
    }
}
