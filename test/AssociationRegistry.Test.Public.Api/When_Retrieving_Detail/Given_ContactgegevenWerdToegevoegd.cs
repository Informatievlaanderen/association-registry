namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Public.Api.Constants;
using AssociationRegistry.Public.Api.Verenigingen.Detail;
using AssociationRegistry.Public.ProjectionHost.Infrastructure.Extensions;
using Fixtures;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_ContactgegevenWerdToegevoegd
{
    private readonly string _vCode;
    private readonly PublicApiClient _publicApiClient;
    private readonly HttpResponseMessage _response;
    private readonly VerenigingWerdGeregistreerd _verenigingWerdGeregistreerd;
    private readonly ContactgegevenWerdToegevoegd _contactgegevenWerdToegevoegd;
    private readonly CommandMetadata _metadata;

    public Given_ContactgegevenWerdToegevoegd(GivenEventsFixture fixture)
    {
        _publicApiClient = fixture.PublicApiClient;
        _vCode = fixture.ContactgegevenWerdToegevoegdScenario.VCode;
        _verenigingWerdGeregistreerd = fixture.ContactgegevenWerdToegevoegdScenario.VerenigingWerdGeregistreerd;
        _contactgegevenWerdToegevoegd = fixture.ContactgegevenWerdToegevoegdScenario.ContactgegevenWerdToegevoegd;
        _metadata = fixture.ContactgegevenWerdToegevoegdScenario.GetCommandMetadata();
        _response = _publicApiClient.GetDetail(_vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response()
        => (await _publicApiClient.GetDetail(_vCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_json_ld_as_content_type()
    {
        var response = await _publicApiClient.GetDetail(_vCode);
        response.Content.Headers.ContentType!.MediaType.Should().Be(WellknownMediaTypes.JsonLd);
    }

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var contactgegevens = Array.Empty<Contactgegeven>()
            .Append(
                new Contactgegeven(
                    Enum.GetName(_contactgegevenWerdToegevoegd.Type),
                    _contactgegevenWerdToegevoegd.Waarde,
                    _contactgegevenWerdToegevoegd.Omschrijving,
                    _contactgegevenWerdToegevoegd.IsPrimair
                ));

        var expected = $@"
        {{
            ""@context"": ""https://127.0.0.1:11003/v1/contexten/detail-vereniging-context.json"",
            ""vereniging"": {{
                    ""vCode"": ""{_vCode}"",
                    ""naam"": ""{_verenigingWerdGeregistreerd.Naam}"",
                    ""korteNaam"": null,
                    ""korteBeschrijving"": null,
                    ""kboNummer"": null,
                    ""startdatum"": null,
                    ""status"": ""Actief"",
                    ""contactgegevens"": [{string.Join(',', contactgegevens.Select(y => $@"{{
                        ""type"": ""{y.Type}"",
                        ""waarde"": ""{y.Waarde}"",
                        ""omschrijving"": ""{y.Omschrijving}"",
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
                    ""hoofdactiviteitenVerenigingsloket"":[{string.Join(',', _verenigingWerdGeregistreerd.HoofdactiviteitenVerenigingsloket.Select(x => $@"{{
                        ""code"":""{x.Code}"",
                        ""beschrijving"":""{x.Beschrijving}""
                    }}"))}]
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}""
                }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
