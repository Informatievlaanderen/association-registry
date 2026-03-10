namespace AssociationRegistry.Test.E2E.When_Triggering_KszSync.Given_A_Removed_Vertegenwoordiger_On_Vereniging.Beheer.Bewaartermijn;

using System.Net;
using Be.Vlaanderen.Basisregisters.BasicApiProblem;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Xunit;

[Collection(nameof(ForRemovedVertegenwoordigerCollection))]
public class Returns_All_Vertegenwoordigers_BevestigdDoorKsz : End2EndTest<ProblemDetails>
{
    private readonly TriggerKszSyncForRemovedVertegenwoordigerContext _context;

    public Returns_All_Vertegenwoordigers_BevestigdDoorKsz(TriggerKszSyncForRemovedVertegenwoordigerContext context)
        : base(context.ApiSetup)
    {
        _context = context;
    }

    public override async Task<ProblemDetails> GetResponse(FullBlownApiSetup setup)
    {
        return await SmartHttpClient
            .Create(_context.ApiSetup.AdminApiHost, _context.ApiSetup.SuperAdminHttpClient)
            .GetAsync<ProblemDetails>(
                $"/v1/admin/bewaartermijnen/{_context.CommandResult.VCode}/{_context.Scenario.VerwijderdeVertegenwoordiger.VertegenwoordigerId}"
            );
    }

    [Fact]
    public void Then_Returns_404_No_Bewaartermijn_Found()
    {
        Response.Should().NotBeNull();

        Response.HttpStatus.Should().Be((int)HttpStatusCode.NotFound);

        Response.Title.Should().Be("Not Found");
    }
}
