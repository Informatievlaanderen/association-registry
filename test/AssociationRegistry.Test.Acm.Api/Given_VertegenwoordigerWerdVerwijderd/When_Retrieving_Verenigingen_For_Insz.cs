namespace AssociationRegistry.Test.Acm.Api.Given_VertegenwoordigerWerdVerwijderd;

using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using System.Net;
using templates;
using Xunit;

[Collection(nameof(AcmApiCollection))]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly VertegenwoordigerWerdVerwijderd_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.VertegenwoordigerWerdVerwijderdEventsInDbScenario;
        _response = fixture.DefaultClient.GetVerenigingenForInsz(_scenario.Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_200()
        => _response.StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public async ValueTask Then_we_get_a_response_with_no_vereniging()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected = new VerenigingenPerInszResponseTemplate()
           .WithInsz(_scenario.Insz);

        content.Should().BeEquivalentJson(expected);
    }
}
