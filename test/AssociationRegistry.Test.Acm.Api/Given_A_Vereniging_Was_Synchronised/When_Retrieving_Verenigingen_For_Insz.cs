namespace AssociationRegistry.Test.Acm.Api.Given_A_Vereniging_Was_Synchronised;

using Fixtures;
using Fixtures.Scenarios;
using FluentAssertions;
using Framework;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using templates;
using Vereniging;

using Xunit;
using Xunit.Categories;

[Collection(nameof(AcmApiCollection))]
[Category("AcmApi")]
[IntegrationTest]
public class When_Retrieving_Verenigingen_For_Insz
{
    private readonly HttpResponseMessage _response;
    private readonly VerenigingMetRechtspersoonlijkheid_WithAllFields_EventsInDbScenario _scenario;

    public When_Retrieving_Verenigingen_For_Insz(EventsInDbScenariosFixture fixture)
    {
        _scenario = fixture.VerenigingMetRechtspersoonlijkheidWithAllFieldsEventsInDbScenario;
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
               .WithInsz(_scenario.Insz)
               .WithVereniging(
                    _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode,
                    _scenario.VertegenwoordigerWerdOvergenomenUitKBO.VertegenwoordigerId,
                    _scenario.NaamWerdGewijzigdInKbo.Naam,
                    Verenigingstype.Parse(_scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Rechtsvorm),
                    kboNummer: _scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer
                );

        content.Should().BeEquivalentJson(expected);
    }
}
