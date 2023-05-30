namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Verenigingen.Detail;
using AssociationRegistry.Framework;
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
public class Given_All_BasisGegevensWerdenGewijzigd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly KorteBeschrijvingWerdGewijzigd _korteBeschrijvingWerdGewijzigd;
    private readonly KorteNaamWerdGewijzigd _korteNaamWerdGewijzigd;
    private readonly CommandMetadata _metadata;
    private readonly NaamWerdGewijzigd _naamWerdGewijzigd;
    private readonly HttpResponseMessage _response;
    private readonly StreamActionResult _result;
    private readonly StartdatumWerdGewijzigd _startdatumWerdGewijzigd;
    private readonly string _vCode;
    private readonly FeitelijkeVerenigingWerdGeregistreerd _feitelijkeVerenigingWerdGeregistreerd;

    public Given_All_BasisGegevensWerdenGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _vCode = fixture.V004AlleBasisGegevensWerdenGewijzigd.VCode;
        _adminApiClient = fixture.DefaultClient;
        _feitelijkeVerenigingWerdGeregistreerd = fixture.V004AlleBasisGegevensWerdenGewijzigd.FeitelijkeVerenigingWerdGeregistreerd;
        _naamWerdGewijzigd = fixture.V004AlleBasisGegevensWerdenGewijzigd.NaamWerdGewijzigd;
        _korteNaamWerdGewijzigd = fixture.V004AlleBasisGegevensWerdenGewijzigd.KorteNaamWerdGewijzigd;
        _korteBeschrijvingWerdGewijzigd = fixture.V004AlleBasisGegevensWerdenGewijzigd.KorteBeschrijvingWerdGewijzigd;
        _startdatumWerdGewijzigd = fixture.V004AlleBasisGegevensWerdenGewijzigd.StartdatumWerdGewijzigd;
        _metadata = fixture.V004AlleBasisGegevensWerdenGewijzigd.Metadata;
        _result = fixture.V004AlleBasisGegevensWerdenGewijzigd.Result;
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
                    ""naam"": ""{_naamWerdGewijzigd.Naam}"",
                    ""korteNaam"": ""{_korteNaamWerdGewijzigd.KorteNaam}"",
                    ""korteBeschrijving"": ""{_korteBeschrijvingWerdGewijzigd.KorteBeschrijving}"",
                    ""startdatum"": ""{_startdatumWerdGewijzigd.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}"",
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
                    }}"))}],
                    ""sleutels"":[]
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}"",
                    ""beheerBasisUri"":""/verenigingen/{_vCode}""
                    }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
