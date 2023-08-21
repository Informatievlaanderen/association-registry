namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using Events;
using EventStore;
using Fixtures;
using FluentAssertions;
using Formatters;
using Framework;
using JasperFx.Core;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_ContactgegevenWerdToegevoegd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly ContactgegevenWerdToegevoegd _contactgegevenWerdToegevoegd;
    private readonly CommandMetadata _metadata;
    private readonly HttpResponseMessage _response;
    private readonly StreamActionResult _result;
    private readonly string _vCode;
    private readonly FeitelijkeVerenigingWerdGeregistreerd _feitelijkeVerenigingWerdGeregistreerd;

    public Given_ContactgegevenWerdToegevoegd(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
        _vCode = fixture.V006ContactgegevenWerdToegevoegd.VCode;
        _feitelijkeVerenigingWerdGeregistreerd = fixture.V006ContactgegevenWerdToegevoegd.FeitelijkeVerenigingWerdGeregistreerd;
        _contactgegevenWerdToegevoegd = fixture.V006ContactgegevenWerdToegevoegd.ContactgegevenWerdToegevoegd;
        _metadata = fixture.V006ContactgegevenWerdToegevoegd.Metadata;
        _result = fixture.V006ContactgegevenWerdToegevoegd.Result;
        _response = fixture.DefaultClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, _result.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_vCode, long.MaxValue))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var contactgegevens = Array.Empty<AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven>()
            .Append(
                _feitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Select(
                    c =>
                        new AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven
                        {
                            ContactgegevenId = c.ContactgegevenId,
                            Type = c.Type,
                            Waarde = c.Waarde,
                            Beschrijving = c.Beschrijving,
                            IsPrimair = c.IsPrimair,
                        }))
            .Append(
                new AssociationRegistry.Admin.Api.Verenigingen.Detail.ResponseModels.Contactgegeven
                {
                    ContactgegevenId = _contactgegevenWerdToegevoegd.ContactgegevenId,
                    Type = _contactgegevenWerdToegevoegd.Type,
                    Waarde = _contactgegevenWerdToegevoegd.Waarde,
                    Beschrijving = _contactgegevenWerdToegevoegd.Beschrijving,
                    IsPrimair = _contactgegevenWerdToegevoegd.IsPrimair,
                });


        var expected = $@"
        {{
            ""@context"": ""{"http://127.0.0.1:11004/v1/contexten/detail-vereniging-context.json"}"",
            ""vereniging"": {{
                    ""vCode"": ""{_vCode}"",
                    ""type"": {{
                        ""code"": ""{Verenigingstype.FeitelijkeVereniging.Code}"",
                        ""beschrijving"": ""{Verenigingstype.FeitelijkeVereniging.Beschrijving}"",
                    }},
                    ""naam"": ""{_feitelijkeVerenigingWerdGeregistreerd.Naam}"",
                    ""korteNaam"": ""{_feitelijkeVerenigingWerdGeregistreerd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_feitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving}"",
                    ""startdatum"": ""{_feitelijkeVerenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""doelgroep"" : {{ ""minimumleeftijd"": {_feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd}, ""maximumleeftijd"": {_feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd} }},
                    ""status"": ""Actief"",
                    ""isUitgeschrevenUitPubliekeDatastroom"": false,
                    ""contactgegevens"": [{string.Join(separator: ',', contactgegevens
                        .OrderBy(c => c.ContactgegevenId).Select(y => $@"{{
                        ""contactgegevenId"": {y.ContactgegevenId},
                        ""type"": ""{y.Type}"",
                        ""waarde"": ""{y.Waarde}"",
                        ""beschrijving"": ""{y.Beschrijving}"",
                        ""isPrimair"": {(y.IsPrimair ? "true" : "false")},
                    }}"))}],
                    ""locaties"":[{string.Join(separator: ',', _feitelijkeVerenigingWerdGeregistreerd.Locaties
                        .Select(x => $@"{{
                        ""locatieId"": {x.LocatieId},
                        ""locatietype"": ""{x.Locatietype}"",
                        ""isPrimair"": {(x.IsPrimair ? "true" : "false")},
                        ""adresvoorstelling"": ""{x.Adres.ToAdresString()}"",
                        ""naam"": ""{x.Naam}"",
                        ""adres"":{{
                            ""straatnaam"": ""{x.Adres!.Straatnaam}"",
                            ""huisnummer"": ""{x.Adres!.Huisnummer}"",
                            ""busnummer"": ""{x.Adres!.Busnummer}"",
                            ""postcode"": ""{x.Adres!.Postcode}"",
                            ""gemeente"": ""{x.Adres!.Gemeente}"",
                            ""land"": ""{x.Adres!.Land}""
                        }},
                        ""adresId"":{(x.AdresId is null ? "null" : $@"{{
                            ""bronwaarde"": {(x.AdresId is not null ? $@"""{x.AdresId.Bronwaarde}""" : "null")},
                            ""broncode"": {(x.AdresId is not null ? $@"""{x.AdresId.Broncode}""" : "null")}
                        }}")}
                    }}"))}
                    ],
                    ""vertegenwoordigers"":[{string.Join(separator: ',', _feitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers
                        .Select(x => $@"{{
                            ""vertegenwoordigerId"": {x.VertegenwoordigerId},
                            ""voornaam"": ""{x.Voornaam}"",
                            ""achternaam"": ""{x.Achternaam}"",
                            ""rol"": ""{x.Rol}"",
                            ""roepnaam"": ""{x.Roepnaam}"",
                            ""isPrimair"": {(x.IsPrimair ? "true" : "false")},
                            ""e-mail"":""{x.Email}"",
                            ""telefoon"":""{x.Telefoon}"",
                            ""mobiel"":""{x.Mobiel}"",
                            ""socialMedia"":""{x.SocialMedia}""
                    }}"))}],
                    ""hoofdactiviteitenVerenigingsloket"":[{string.Join(separator: ',', _feitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x => $@"{{
                        ""code"":""{x.Code}"",
                        ""beschrijving"":""{x.Beschrijving}""
                    }}"))}],
                    ""sleutels"":[],
                    ""relaties"":[],
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}"",
                    }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
