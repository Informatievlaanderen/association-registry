namespace AssociationRegistry.Test.Admin.Api.When_Retrieving_Historiek;

using AssociationRegistry.Admin.Api;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using EventStore;
using Fixtures;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AdminApiCollection))]
[Category("AdminApi")]
[IntegrationTest]
public class Given_An_Outdated_Sequence
{
    private readonly EventsInDbScenariosFixture _fixture;
    private readonly AdminApiClient _adminApiClient;

    public Given_An_Outdated_Sequence(EventsInDbScenariosFixture fixture)
    {
        _fixture = fixture;
        _adminApiClient = fixture.AdminApiClient;
    }

    [Fact]
    public async Task Then_we_get_a_precondition_failed_response_for_historiek()
    {
        var response = await _adminApiClient.GetHistoriek(_fixture.V001FeitelijkeVerenigingWerdGeregistreerdWithAllFields.VCode, long.MaxValue);
        var content = await response.Content.ReadAsStringAsync();

        var expected = new ProblemDetailsResponseTemplate()
                      .WithStatus(StatusCodes.Status412PreconditionFailed)
                      .WithDetail(ValidationMessages.Status412Historiek);

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
