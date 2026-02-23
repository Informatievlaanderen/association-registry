namespace AssociationRegistry.Test.E2E.When_Voeg_Vertegenwoordiger_Toe.Beheer.vertegenwoordigersPerVCode;

using Admin.Api.WebApi.Administratie.VertegenwoordigersPerVCode;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Detail.ResponseModels;
using AssociationRegistry.Contracts.JsonLdContext;
using AssociationRegistry.Test.E2E.Framework.AlbaHost;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Vereniging.Bronnen;
using FluentAssertions;
using Xunit;

[Collection(nameof(VoegVertegenwoordigerToeCollection))]
public class Returns_Vertegenwoordigers_Per_VCode : End2EndTest<VertegenwoordigerResponse[]>
{
    private readonly VoegVertegenwoordigerToeContext _testContext;

    public Returns_Vertegenwoordigers_Per_VCode(VoegVertegenwoordigerToeContext testContext)
        : base(testContext.ApiSetup)
    {
        _testContext = testContext;
    }

    public override async Task<VertegenwoordigerResponse[]> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetVertegenwoordiger(
            setup.SuperAdminHttpClient,
            _testContext.VCode,
            new RequestParameters().WithExpectedSequence(_testContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        Response.Should().NotBeNull();
    }
}
