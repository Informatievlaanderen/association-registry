namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework.Fixtures;
using Microsoft.Net.Http.Headers;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[System.ComponentModel.Category("AdminApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced_With_Adressenregister
{
    private readonly AdminApiClient _adminApiClient;

    private readonly
        V072_FeitelijkeVerenigingWerdGeregistreerd_WithLocaties_ForWijzigen_For_AdresKonNietOvergenomenWordenUitAdressenregister _scenario;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced_With_Adressenregister(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture
           .V072FeitelijkeVerenigingWerdGeregistreerdWithLocatiesForWijzigenForAdresKonNietOvergenomenWordenUitAdressenregister;

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
    public async Task Then_it_returns_an_etag_header()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        response.Headers.ETag.Should().NotBeNull();

        var etag = response.Headers.GetValues(HeaderNames.ETag).ToList().Should().ContainSingle().Subject;
        etag.Should().Be("W/\"5\"");
    }
}
