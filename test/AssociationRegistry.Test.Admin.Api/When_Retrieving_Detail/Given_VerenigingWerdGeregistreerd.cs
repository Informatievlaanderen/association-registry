namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using Events;
using EventStore;
using Fixtures;
using Framework;
using FluentAssertions;
using JasperFx.Core;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly string _vCode;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly HttpResponseMessage _response;
    private readonly StreamActionResult _result;

    public Given_VerenigingWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
        _vCode = fixture.V001VerenigingWerdGeregistreerdWithAllFields.VCode;
        _verenigingWerdGeregistreerd = fixture.V001VerenigingWerdGeregistreerdWithAllFields.VerenigingWerdGeregistreerd;
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
                _verenigingWerdGeregistreerd.Contactgegevens.Select(
                    c =>
                        new DetailVerenigingResponse.VerenigingDetail.Contactgegeven()
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
                    ""naam"": ""{_verenigingWerdGeregistreerd.Naam}"",
                    ""korteNaam"": ""{_verenigingWerdGeregistreerd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_verenigingWerdGeregistreerd.KorteBeschrijving}"",
                    ""kboNummer"": ""{_verenigingWerdGeregistreerd.KboNummer}"",
                    ""startdatum"": ""{_verenigingWerdGeregistreerd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
                    ""status"": ""Actief"",
                    ""contactgegevens"": [{string.Join(',', contactgegevens.Select(y => $@"{{
                        ""contactgegevenId"": {y.ContactgegevenId},
                        ""type"": ""{y.Type}"",
                        ""waarde"": ""{y.Waarde}"",
                        ""beschrijving"": ""{y.Beschrijving}"",
                        ""isPrimair"": {(y.IsPrimair ? "true" : "false")},
                    }}"))}],
                    ""locaties"":[{string.Join(',', _verenigingWerdGeregistreerd.Locaties.Select(x => $@"{{
                        ""locatietype"": ""{x.Locatietype}"",
                        {(x.Hoofdlocatie ? $"\"hoofdlocatie\": {x.Hoofdlocatie.ToString().ToLower()}," : string.Empty)}
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
                    ""vertegenwoordigers"":[{string.Join(',', _verenigingWerdGeregistreerd.Vertegenwoordigers.Select(x => $@"{{
                            ""insz"": ""{x.Insz}"",
                            ""voornaam"": ""{x.Voornaam}"",
                            ""achternaam"": ""{x.Achternaam}"",
                            ""rol"": ""{x.Rol}"",
                            ""roepnaam"": ""{x.Roepnaam}"",
                            ""primairContactpersoon"": {(x.PrimairContactpersoon ? "true" : "false")},
                            ""email"":""{x.Email}"",
                            ""telefoon"":""{x.Telefoon}"",
                            ""mobiel"":""{x.Mobiel}"",
                            ""socialMedia"":""{x.SocialMedia}""
                        }}"))}],
                    ""hoofdactiviteitenVerenigingsloket"":[{string.Join(',', _verenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x => $@"{{
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
