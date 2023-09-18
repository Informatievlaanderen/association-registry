namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using Events;
using EventStore;
using Fixtures;
using FluentAssertions;
using Framework;
using JasperFx.Core;
using System.Net;
using Vereniging;
using Vereniging.Bronnen;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGestopt
{
    private readonly AdminApiClient _adminApiClient;
    private readonly HttpResponseMessage _response;
    private readonly StreamActionResult _result;
    private readonly string _vCode;
    private readonly FeitelijkeVerenigingWerdGeregistreerd _feitelijkeVerenigingWerdGeregistreerd;
    private readonly CommandMetadata _metadata;
    private readonly VerenigingWerdGestopt _verenigignWerdGestop;
    private readonly EinddatumWerdGewijzigd _einddatumWerdGewijzigd;

    public Given_FeitelijkeVerenigingWerdGestopt(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
        _vCode = fixture.V041FeitelijkeVerenigingWerdGestopt.VCode;
        _feitelijkeVerenigingWerdGeregistreerd = fixture.V041FeitelijkeVerenigingWerdGestopt.FeitelijkeVerenigingWerdGeregistreerd;
        _verenigignWerdGestop = fixture.V041FeitelijkeVerenigingWerdGestopt.VerenigingWerdGestopt;
        _einddatumWerdGewijzigd = fixture.V041FeitelijkeVerenigingWerdGestopt.EinddatumWerdGewijzigd;
        _result = fixture.V041FeitelijkeVerenigingWerdGestopt.Result;
        _metadata = fixture.V041FeitelijkeVerenigingWerdGestopt.Metadata;
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
                                                }));

        var expected = $@"
        {{
            ""@context"": ""{"http://127.0.0.1:11003/v1/contexten/beheer/detail-vereniging-context.json"}"",
            ""vereniging"": {{
                    ""vCode"": ""{_vCode}"",
                    ""type"": {{
                        ""code"": ""{Verenigingstype.FeitelijkeVereniging.Code}"",
                        ""beschrijving"": ""{Verenigingstype.FeitelijkeVereniging.Beschrijving}"",
                    }},
                    ""naam"": ""{_feitelijkeVerenigingWerdGeregistreerd.Naam}"",
                    ""korteNaam"": """",
                    ""korteBeschrijving"": """",
                    ""startdatum"": null,
                    ""einddatum"": ""{_einddatumWerdGewijzigd.Einddatum.ToString(WellknownFormats.DateOnly)}"",
                    ""doelgroep"": {{
                        ""minimumleeftijd"": {_feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Minimumleeftijd},
                        ""maximumleeftijd"": {_feitelijkeVerenigingWerdGeregistreerd.Doelgroep.Maximumleeftijd}
                    }},
                    ""status"": ""Gestopt"",
                    ""isUitgeschrevenUitPubliekeDatastroom"": false,
                    ""contactgegevens"": [],
                    ""locaties"":[],
                    ""vertegenwoordigers"":[],
                    ""hoofdactiviteitenVerenigingsloket"":[],
                    ""sleutels"":[],
                    ""relaties"":[],
                    ""bron"": ""{Bron.Initiator.Waarde}"",
                }},
                ""metadata"": {{
                    ""datumLaatsteAanpassing"": ""{_metadata.Tijdstip.ToBelgianDate()}"",
                }}
                }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
