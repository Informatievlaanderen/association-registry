namespace AssociationRegistry.Test.E2E.Erkenningen.When_Verwijder_Erkenning.Beheer.Zoeken;

using Admin.Api.WebApi.Verenigingen.Search.ResponseModels;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten;
using Xunit;

[Collection(nameof(VerwijderErkenningCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly VerwijderErkenningContext _testContext;

    public Returns_ZoekResponse(VerwijderErkenningContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<SearchVerenigingenResponse> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetBeheerZoeken(
            setup.AdminHttpClient,
            $"vCode:{_testContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.VCode}",
            setup.AdminApiHost.DocumentStore(),
            headers: new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void IsErkend_Is_False() => Response.Verenigingen.Single().IsErkend.Should().BeFalse();
}
