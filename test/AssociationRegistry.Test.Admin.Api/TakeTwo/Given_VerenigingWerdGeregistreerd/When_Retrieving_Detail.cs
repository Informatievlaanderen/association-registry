namespace AssociationRegistry.Test.Admin.Api.TakeTwo.Given_VerenigingWerdGeregistreerd;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.EventStore;
using Events;
using Fixtures;
using Framework;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class When_Retrieving_Detail
{
    private readonly AdminApiClient _adminApiClient;
    private readonly string _vCode;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly HttpResponseMessage _response;
    private readonly StreamActionResult _result;

    public When_Retrieving_Detail(GivenEventsFixture fixture)
    {
        _adminApiClient = fixture.AdminApiClient;
        _vCode = fixture.VerenigingWerdGeregistreerdScenario.VCode;
        _verenigingWerdGeregistreerd = fixture.VerenigingWerdGeregistreerdScenario.VerenigingWerdGeregistreerd;
        _result = fixture.VerenigingWerdGeregistreerdScenario.Result;
        _response = fixture.AdminApiClient.GetDetail(_vCode).GetAwaiter().GetResult();
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
                    ""contactInfoLijst"": [{string.Join(',', _verenigingWerdGeregistreerd.ContactInfoLijst!.Select(x => $@"{{
                        ""contactnaam"": ""{x.Contactnaam}"",
                        ""email"": ""{x.Email}"",
                        ""telefoon"": ""{x.Telefoon}"",
                        ""website"": ""{x.Website}"",
                        ""socialMedia"": ""{x.SocialMedia}""
                    }}"))}
                    ],
                    ""locaties"":[{string.Join(',', _verenigingWerdGeregistreerd.Locaties!.Select(x => $@"{{
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
