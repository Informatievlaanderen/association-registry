namespace AssociationRegistry.Test.E2E.Erkenningen.When_Schors_Erkenning.Publiek.Zoeken;

using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;

[Collection(nameof(SchorsErkenningCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly SchorsErkenningContext _testContext;

    public Returns_ZoekResponse(SchorsErkenningContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.PublicApiHost.GetPubliekZoeken(
            $"vCode:{_testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}",
            _testContext.CommandResult.Sequence
        );

    [Fact]
    public void IsErkend_Is_False() => Response.Verenigingen.Single().IsErkend.Should().BeFalse();
}
