namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Detail;

using AssociationRegistry.Admin.Api;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using EventStore;
using System.Net;
using Fixtures;
using Fixtures.Scenarios.EventsInDb;
using FluentAssertions;
using Framework;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly AdminApiClient _adminApiClient;
    private readonly V019_AfdelingWerdGeregistreerd_WithMinimalFields _scenario;

    public Given_AfdelingWerdGeregistreerd_With_Minimal_Fields(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.V019AfdelingWerdGeregistreerdWithMinimalFields;
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
    public async Task Then_we_get_a_detail_afdeling_response()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new DetailVerenigingResponseTemplate()
                      .FromEvent(_scenario.AfdelingWerdGeregistreerd)
                      .WithDatumLaatsteAanpassing(_scenario.Metadata.Tijdstip);

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

    //

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_for_detail()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode, long.MaxValue);
        var content = await response.Content.ReadAsStringAsync();
        var expected = new ProblemDetailsResponseTemplate()
           .FromException(new UnexpectedAggregateVersionException(ValidationMessages.Status412Detail));
        var contentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);
        var expectedObject = JsonConvert.DeserializeObject<ProblemDetails>(expected.Build());

        response
           .StatusCode
           .Should().Be(HttpStatusCode.PreconditionFailed);

        contentObject.Should().BeEquivalentTo(
            expectedObject,
            options => options
                      .Excluding(info => info!.ProblemInstanceUri)
                      .Excluding(info => info!.ProblemTypeUri));
    }

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_for_historiek()
    {
        var response = await _adminApiClient.GetDetail(_scenario.VCode, long.MaxValue);
        var content = await response.Content.ReadAsStringAsync();
        var expected = new ProblemDetailsResponseTemplate()
           .FromException(new UnexpectedAggregateVersionException(ValidationMessages.Status412Historiek));
        var contentObject = JsonConvert.DeserializeObject<ProblemDetails>(content);
        var expectedObject = JsonConvert.DeserializeObject<ProblemDetails>(expected.Build());

        response
           .StatusCode
           .Should().Be(HttpStatusCode.PreconditionFailed);

        contentObject.Should().BeEquivalentTo(
            expectedObject,
            options => options
                      .Excluding(info => info!.ProblemInstanceUri)
                      .Excluding(info => info!.ProblemTypeUri));
    }
}
