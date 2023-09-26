﻿namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using System.Net;
using EventStore;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_All_BasisGegevensWerdenGewijzigd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly StreamActionResult _result;
    private readonly V004_AlleBasisGegevensWerdenGewijzigd _scenario;

    public Given_All_BasisGegevensWerdenGewijzigd(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V004AlleBasisGegevensWerdenGewijzigd;
        _adminApiClient = fixture.DefaultClient;
        _result = _scenario.Result;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, _result.Sequence))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_successful_response_if_no_sequence_provided()
        => (await _adminApiClient.GetDetail(_scenario.VCode))
          .Should().BeSuccessful();

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_if_sequence_is_less_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, long.MaxValue))
          .StatusCode
          .Should().Be(HttpStatusCode.PreconditionFailed);

    [Fact]
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                      .WithNaam(_scenario.NaamWerdGewijzigd.Naam)
                      .WithKorteNaam(_scenario.KorteNaamWerdGewijzigd.KorteNaam)
                      .WithKorteBeschrijving(_scenario.KorteBeschrijvingWerdGewijzigd.KorteBeschrijving)
                      .WithStartdatum(_scenario.StartdatumWerdGewijzigd.Startdatum)
                      .WithDoelgroep(_scenario.DoelgroepWerdGewijzigd.Doelgroep.Minimumleeftijd,
                                     _scenario.DoelgroepWerdGewijzigd.Doelgroep.Maximumleeftijd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip);

        content.Should().BeEquivalentJson(expected);
    }
}
