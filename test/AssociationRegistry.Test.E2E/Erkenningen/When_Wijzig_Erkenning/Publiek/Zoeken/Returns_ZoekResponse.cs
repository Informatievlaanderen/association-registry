namespace AssociationRegistry.Test.E2E.Erkenningen.When_Wijzig_Erkenning.Publiek.Zoeken;

using DecentraalBeheer.Vereniging.Erkenningen;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.WebApi.Verenigingen.Search.ResponseModels;
using Xunit;

[Collection(nameof(WijzigErkenningCollection))]
public class Returns_ZoekResponse : End2EndTest<SearchVerenigingenResponse>
{
    private readonly WijzigErkenningContext _testContext;

    public Returns_ZoekResponse(WijzigErkenningContext testContext)
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
    public void IsErkend_Is_Set()
    {
        var isErkend =
            ErkenningStatus
                .Bepaal(
                    ErkenningsPeriode.Create(
                        _testContext.CommandRequest.Startdatum.Value,
                        _testContext.CommandRequest.Einddatum.Value
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
                .Value == ErkenningStatus.Actief.Value;

        Response.Verenigingen.Single().IsErkend.Should().Be(isErkend);
    }
}
