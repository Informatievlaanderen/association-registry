namespace AssociationRegistry.Test.Public.Api.When_Retrieving_Detail;

using System.Text.RegularExpressions;
using Fixtures.GivenEvents;
using FluentAssertions;
using Framework;
using Xunit;
using Xunit.Categories;

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly HttpResponseMessage _response;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(GivenEventsFixture fixture)
    {
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.V014VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithAllDataScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

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
            $"files.{nameof(Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd)}_{nameof(Then_we_get_a_detail_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens
{
    private readonly HttpResponseMessage _response;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens(GivenEventsFixture fixture)
    {
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.V015VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithWijzigBasisgegevens.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

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
            $"files.{nameof(Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigBasisgegevens)}_{nameof(Then_we_get_a_detail_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}
