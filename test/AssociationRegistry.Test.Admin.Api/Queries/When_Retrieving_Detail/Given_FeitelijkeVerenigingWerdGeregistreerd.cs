<<<<<<<< HEAD:test/AssociationRegistry.Test.Admin.Api/Projections/When_Retrieving_Detail/Given_FeitelijkeVerenigingWerdGeregistreerd.cs
namespace AssociationRegistry.Test.Admin.Api.Projections.When_Retrieving_Detail;

using Common.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
========
namespace AssociationRegistry.Test.Admin.Api.Queries.When_Retrieving_Detail;

using AssociationRegistry.Test.Admin.Api.Framework;
using AssociationRegistry.Test.Common.Scenarios.EventsInDb;
using FluentAssertions;
>>>>>>>> 7835cb64 (WIP: or-2313 refactor tests):test/AssociationRegistry.Test.Admin.Api/Queries/When_Retrieving_Detail/Given_FeitelijkeVerenigingWerdGeregistreerd.cs
using Framework.Fixtures;
using Framework.templates;
using System.Net;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_FeitelijkeVerenigingWerdGeregistreerd
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V001_FeitelijkeVerenigingWerdGeregistreerd_WithAllFields _scenario;

    public Given_FeitelijkeVerenigingWerdGeregistreerd(EventsInDbScenariosFixture fixture)
    {
        _adminApiClient = fixture.DefaultClient;
        _scenario = fixture.V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields;
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
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip);

        content.Should().BeEquivalentJson(expected);
    }
}
