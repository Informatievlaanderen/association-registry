namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel
{
    private readonly HttpResponseMessage _response;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel(GivenEventsFixture fixture)
    {
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture
                                                                 .V017VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigMaatschappelijkeZetelScenario
                                                                 .VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

        var vCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode;

        var publicApiClient = fixture.PublicApiClient;
        _response = publicApiClient.GetDetail(vCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_detail_response()
    {
        var content = await _response.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetel)}_{nameof(Then_we_get_a_detail_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
