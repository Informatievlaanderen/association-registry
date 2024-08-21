namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail;

using AssociationRegistry.Admin.Schema.Constants;
using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Framework.Fixtures;
using Framework.templates;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGestopt
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V041_FeitelijkeVerenigingWerdGestopt _scenario;

    public Given_FeitelijkeVerenigingWerdGestopt(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
        _scenario = fixture.V041FeitelijkeVerenigingWerdGestopt;
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
    public async Task Then_we_get_a_detail_vereniging_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.FeitelijkeVerenigingWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip)
                      .WithEinddatum(_scenario.EinddatumWerdGewijzigd.Einddatum)
                      .WithStatus(VerenigingStatus.Gestopt);

        content.Should().BeEquivalentJson(expected);
    }
}
