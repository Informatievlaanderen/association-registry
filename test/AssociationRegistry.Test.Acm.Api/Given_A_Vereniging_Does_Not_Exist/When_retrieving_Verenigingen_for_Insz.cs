namespace AssociationRegistry.Test.Acm.Api.Given_A_Vereniging_Does_Not_Exist;

using Fixtures;
using FluentAssertions;
using Framework;
using System.Net;
using templates;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_retrieving_Verenigingen_for_Insz
{
    private const string Insz = "00000000000";
    private readonly HttpResponseMessage _response;

    public When_retrieving_Verenigingen_for_Insz(EventsInDbScenariosFixture fixture)
    {
        _response = fixture.DefaultClient.GetVerenigingenForInsz(Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_200()
        => _response.StatusCode.Should().Be(HttpStatusCode.OK);

    [Fact]
    public async ValueTask Then_we_get_a_response_with_no_verenigingen()
    {
        var content = await _response.Content.ReadAsStringAsync();

        var expected =
            new VerenigingenPerInszResponseTemplate()
               .WithInsz(Insz);

        content.Should().BeEquivalentJson(expected);
    }
}
