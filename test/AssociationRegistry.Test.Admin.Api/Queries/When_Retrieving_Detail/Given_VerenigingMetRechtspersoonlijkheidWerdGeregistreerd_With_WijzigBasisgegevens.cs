<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens.cs
﻿namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
========
﻿namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens.cs
using Framework.Fixtures;
using Framework.templates;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V038_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V038VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens;
        _adminApiClient = fixture.DefaultClient;
    }

    [Fact]
    public async Task Then_we_get_a_successful_response_if_sequence_is_equal_or_greater_than_expected_sequence()
        => (await _adminApiClient.GetDetail(_scenario.VCode, _scenario.Result.Sequence))
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
    public async Task Then_we_get_a_detail_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var vCode = _scenario.VCode;

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip)
                      .WithRoepnaam(_scenario.RoepnaamWerdGewijzigd.Roepnaam);

        content.Should().BeEquivalentJson(expected);
    }

    [Fact]
    public async Task Then_it_returns_an_etag_header()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        response.Headers.ETag.Should().NotBeNull();

        var etag = response.Headers.GetValues(HeaderNames.ETag).ToList().Should().ContainSingle().Subject;
        etag.Should().StartWith("W/\"").And.EndWith("\"");
    }
}
