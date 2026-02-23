namespace AssociationRegistry.Test.E2E.When_Voeg_Vertegenwoordiger_Toe_After_Ksz_Sync.Beheer.vertegenwoordigersPerVCode;

using Admin.Api.WebApi.Administratie.VertegenwoordigersPerVCode;
using Admin.Schema.Vertegenwoordiger;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(VoegVertegenwoordigerToeAfterKszSyncCollection))]
public class Returns_All_Vertegenwoordigers_BevestigdDoorKsz : End2EndTest<VertegenwoordigerResponse[]>
{
    private readonly VoegVertegenwoordigerToeAfterKszSyncContext _testAfterKszSyncContext;

    public Returns_All_Vertegenwoordigers_BevestigdDoorKsz(
        VoegVertegenwoordigerToeAfterKszSyncContext testAfterKszSyncContext
    )
        : base(testAfterKszSyncContext.ApiSetup)
    {
        _testAfterKszSyncContext = testAfterKszSyncContext;
    }

    public override async Task<VertegenwoordigerResponse[]> GetResponse(FullBlownApiSetup setup) =>
        await setup.AdminApiHost.GetVertegenwoordiger(
            setup.SuperAdminHttpClient,
            _testAfterKszSyncContext.VCode,
            new RequestParameters().WithExpectedSequence(_testAfterKszSyncContext.CommandResult.Sequence)
        );

    [Fact]
    public void JsonContentMatches()
    {
        var nextVertegenwoordigerId =
            _testAfterKszSyncContext.Scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers.Max(
                x => x.VertegenwoordigerId
            ) + 1;

        Response
            .First(x => x.VertegenwoordigerId == nextVertegenwoordigerId)
            .Status.Should()
            .Be(VertegenwoordigerKszStatus.Bevestigd);
    }
}
