namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using Events;
using EventStore;
using Fixtures;
using FluentAssertions;
using Framework;
using JasperFx.Core;
using Vereniging;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly HttpResponseMessage _response;
    private readonly StreamActionResult _result;
    private readonly string _vCode;
    private readonly FeitelijkeVerenigingWerdGeregistreerd _feitelijkeVerenigingWerdGeregistreerd;

    public Given_VerenigingWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
        _vCode = fixture.V001VerenigingWerdGeregistreerdWithAllFields.VCode;
        _feitelijkeVerenigingWerdGeregistreerd = fixture.V001VerenigingWerdGeregistreerdWithAllFields.FeitelijkeVerenigingWerdGeregistreerd;
        _result = fixture.V001VerenigingWerdGeregistreerdWithAllFields.Result;
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
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var contactgegevens = Array.Empty<DetailVerenigingResponse.VerenigingDetail.Contactgegeven>()
            .Append(
                _feitelijkeVerenigingWerdGeregistreerd.Contactgegevens.Select(
                    c =>
                        new DetailVerenigingResponse.VerenigingDetail.Contactgegeven
                        {
                            ContactgegevenId = c.ContactgegevenId,
                            Type = c.Type,
                            Waarde = c.Waarde,
                            Beschrijving = c.Beschrijving,
                            IsPrimair = c.IsPrimair,
                        }));
        var expected = $@"
        {{
            ""vereniging"": {{
                    ""vCode"": ""{_vCode}"",
                    ""type"": {{
                        ""code"": ""{VerenigingsType.FeitelijkeVereniging.Code}"",
                        ""beschrijving"": ""{VerenigingsType.FeitelijkeVereniging.Beschrijving}"",
                    }},
                    ""naam"": ""{_feitelijkeVerenigingWerdGeregistreerd.Naam}"",
                    ""korteNaam"": ""{_feitelijkeVerenigingWerdGeregistreerd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_feitelijkeVerenigingWerdGeregistreerd.KorteBeschrijving}"",
                    ""startdatum"": ""{_feitelijkeVerenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""status"": ""Actief"",
                    ""contactgegevens"": [{string.Join(separator: ',', contactgegevens.Select(y => $@"{{
                        ""contactgegevenId"": {y.ContactgegevenId},
                        ""type"": ""{y.Type}"",
                        ""waarde"": ""{y.Waarde}"",
                        ""beschrijving"": ""{y.Beschrijving}"",
                        ""isPrimair"": {(y.IsPrimair ? "true" : "false")},
                    }}"))}],
                    ""locaties"":[{string.Join(separator: ',', _feitelijkeVerenigingWerdGeregistreerd.Locaties.Select(x => $@"{{
                        ""locatietype"": ""{x.Locatietype}"",
                        ""hoofdlocatie"": {(x.Hoofdlocatie ? "true" : "false")},
                        ""adres"": ""{x.ToAdresString()}"",
                        ""naam"": ""{x.Naam}"",
                        ""straatnaam"": ""{x.Straatnaam}"",
                        ""huisnummer"": ""{x.Huisnummer}"",
                        ""busnummer"": ""{x.Busnummer}"",
                        ""postcode"": ""{x.Postcode}"",
                        ""gemeente"": ""{x.Gemeente}"",
                        ""land"": ""{x.Land}""
                    }}"))}
                    ],
                    ""vertegenwoordigers"":[{string.Join(separator: ',', _feitelijkeVerenigingWerdGeregistreerd.Vertegenwoordigers.Select(x => $@"{{
                            ""vertegenwoordigerId"": {x.VertegenwoordigerId},
                            ""insz"": ""{x.Insz}"",
                            ""voornaam"": ""{x.Voornaam}"",
                            ""achternaam"": ""{x.Achternaam}"",
                            ""rol"": ""{x.Rol}"",
                            ""roepnaam"": ""{x.Roepnaam}"",
                            ""isPrimair"": {(x.IsPrimair ? "true" : "false")},
                            ""email"":""{x.Email}"",
                            ""telefoon"":""{x.Telefoon}"",
                            ""mobiel"":""{x.Mobiel}"",
                            ""socialMedia"":""{x.SocialMedia}""
                        }}"))}],
                    ""hoofdactiviteitenVerenigingsloket"":[{string.Join(separator: ',', _feitelijkeVerenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x => $@"{{
                        ""code"":""{x.Code}"",
                        ""beschrijving"":""{x.Beschrijving}""
                    }}"))}
                    ]
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": """"
                }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
