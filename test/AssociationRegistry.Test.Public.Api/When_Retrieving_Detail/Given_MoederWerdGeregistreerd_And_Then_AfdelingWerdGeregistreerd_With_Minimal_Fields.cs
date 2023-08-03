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
public class Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields
{
    private readonly HttpResponseMessage _afdelingResponse;
    private readonly HttpResponseMessage _moederResponse;

    public Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields(GivenEventsFixture fixture)
    {
        var moederWerdGeregistreerd = fixture.V009MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario.MoederWerdGeregistreerd;

        string afdelingVCode = fixture.V009MoederWerdGeregistreerdAndThenAfdelingWerdGeregistreerdScenario.VCode;
        var moederVCode = moederWerdGeregistreerd.VCode;

        var publicApiClient = fixture.PublicApiClient;
        _afdelingResponse = publicApiClient.GetDetail(afdelingVCode).GetAwaiter().GetResult();
        _moederResponse = publicApiClient.GetDetail(moederVCode).GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Then_we_get_a_detail_afdeling_response()
    {
        var content = await _afdelingResponse.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields)}_{nameof(Then_we_get_a_detail_afdeling_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }

    [Fact]
    public async Task Then_we_get_a_detail_moeder_response()
    {
        var content = await _moederResponse.Content.ReadAsStringAsync();
        content = Regex.Replace(content, "\"datumLaatsteAanpassing\":\".+\"", "\"datumLaatsteAanpassing\":\"\"");

        var goldenMaster = GetType().GetAssociatedResourceJson(
            $"files.{nameof(Given_MoederWerdGeregistreerd_And_Then_AfdelingWerdGeregistreerd_With_Minimal_Fields)}_{nameof(Then_we_get_a_detail_moeder_response)}");

        content.Should().BeEquivalentJson(goldenMaster);
    }
}

[Collection(nameof(PublicApiCollection))]
[Category("PublicApi")]
[IntegrationTest]
public class Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd
{
    private readonly HttpResponseMessage _response;

    public Given_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(GivenEventsFixture fixture)
    {
        var verenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.V014VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

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
