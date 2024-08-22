namespace AssociationRegistry.Test.Acm.Api.Given_VerengingWerdVerwijderd;

using Common.Extensions;
using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using System.Net;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_retrieving_Vereniging_for_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly FeitelijkeVerenigingWerdVerwijderd_EventsInDbScenario _scenario;

    public When_retrieving_Vereniging_for_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.FeitelijkeVerenigingWerdVerwijderdEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_200()
        => _response.StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public async Task Then_we_get_a_response_with_one_vereniging()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected =
            new VerenigingenPerInszResponseTemplate()
               .WithInsz(_scenario.Insz);

        content.Should().BeEquivalentJson(expected);
    }
}
