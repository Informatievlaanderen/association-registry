namespace AssociationRegistry.Test.Acm.Api.Given_A_Vereniging_Does_Not_Exist;

using System.Net;
using Fixtures;
using FluentAssertions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_retrieving_Verenigingen_for_Insz
{
    private const string Insz = "00.00.00.000-00";
    private readonly HttpResponseMessage _response;

    public When_retrieving_Verenigingen_for_Insz(EventsInDbScenariosFixture fixture)
    {
        _response = fixture.DefaultClient.GetVerenigingenForInsz(Insz).GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_we_get_a_404()
        => _response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
