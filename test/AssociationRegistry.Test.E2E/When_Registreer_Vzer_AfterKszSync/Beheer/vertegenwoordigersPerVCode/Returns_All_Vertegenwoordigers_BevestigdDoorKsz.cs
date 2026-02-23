namespace AssociationRegistry.Test.E2E.When_Registreer_Vzer_AfterKszSync.Beheer.vertegenwoordigersPerVCode;

using Admin.Api.WebApi.Administratie.VertegenwoordigersPerVCode;
using Admin.Schema.Vertegenwoordiger;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(vzerWerdGeregistreerdAfterKszSyncCollection))]
public class Returns_All_Vertegenwoordigers_BevestigdDoorKsz : End2EndTest<VertegenwoordigerResponse[]>
{
    private readonly vzerWerdGeregistreerdAfterKszSyncContext _testContext;

    public Returns_All_Vertegenwoordigers_BevestigdDoorKsz(vzerWerdGeregistreerdAfterKszSyncContext testContext)
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
        Response.All(x => x.Status == VertegenwoordigerKszStatus.Bevestigd).Should().BeTrue();
    }
}
