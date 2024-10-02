namespace AssociationRegistry.Test.Admin.Api.Projections.V1.When_Retrieving_Detail;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Admin.Api.Framework.Fixtures;
using AssociationRegistry.Test.Admin.Api.Framework.templates;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_ContactgegevenWerdToegevoegd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V006_ContactgegevenWerdToegevoegd _scenario;

    public Given_ContactgegevenWerdToegevoegd(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
        _scenario = fixture.V006ContactgegevenWerdToegevoegd;
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
                      .WithContactgegeven(_scenario.ContactgegevenWerdToegevoegd.ContactgegevenId,
                                          _scenario.ContactgegevenWerdToegevoegd.Bron,
                                          _scenario.ContactgegevenWerdToegevoegd.Contactgegeventype,
                                          _scenario.ContactgegevenWerdToegevoegd.Waarde, _scenario.VCode,
                                          _scenario.ContactgegevenWerdToegevoegd.Beschrijving,
                                          _scenario.ContactgegevenWerdToegevoegd.IsPrimair)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip);

        content.Should().BeEquivalentJson(expected);
    }
}
