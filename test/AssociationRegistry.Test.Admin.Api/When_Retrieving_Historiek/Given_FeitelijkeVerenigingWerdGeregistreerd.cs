﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using System.Net;
using System.Text.RegularExpressions;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using EventStore;
using AssociationRegistry.Framework;
using Fixtures;
using Fixtures.Scenarios;
using Fixtures.Scenarios.EventsInDb;
using Framework;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly HttpResponseMessage _response;
    private readonly CommandMetadata _metadata;

    private readonly V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields _scenario;


    public Given_FeitelijkeVerenigingWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields;
        _adminApiClient = fixture.DefaultClient;

        _metadata = _scenario.Metadata;
        _result = _scenario.Result;
        _response = _adminApiClient.GetHistoriek(_scenario.VCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode, _result.Sequence))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode))
            .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetHistoriek(_scenario.VCode, long.MaxValue))
            .StatusCode
            .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_registratie_gebeurtenissen()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var expected = $@"
            {{
                ""vCode"": ""{_scenario.VCode}"",
                ""gebeurtenissen"": [
                    {{
                        ""beschrijving"": ""Feitelijke vereniging werd geregistreerd met naam '{_scenario.FeitelijkeVerenigingWerdGeregistreerd.Naam}'."",
                        ""gebeurtenis"":""FeitelijkeVerenigingWerdGeregistreerd"",
                        ""data"":{JsonConvert.SerializeObject(_scenario.FeitelijkeVerenigingWerdGeregistreerd)},
                        ""initiator"":""{_metadata.Initiator}"",
                        ""tijdstip"":""{_metadata.Tijdstip.ToBelgianDateAndTime()}""
                    }}
                ]
            }}
        ";

        content.Should().BeEquivalentJson(expected);
    }
}
