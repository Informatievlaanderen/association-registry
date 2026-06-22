namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging_In_KBO;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Test.E2E.Framework.ApiSetup;
using AssociationRegistry.Test.E2E.Framework.TestClasses;
using AssociationRegistry.Test.E2E.Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.E2E.Scenarios.Requests;
using Xunit;

public class StopVerenigingInKBOContext : TestContextBase<VerenigingWerdGestoptInKBOScenario, NullRequest>
{
    protected override VerenigingWerdGestoptInKBOScenario InitializeScenario() => new();

    public StopVerenigingInKBOContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(VerenigingWerdGestoptInKBOScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(
            VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
            new NullRequest()
        );
    }
}

[CollectionDefinition(nameof(StopVerenigingInKBOCollection))]
public class StopVerenigingInKBOCollection : ICollectionFixture<StopVerenigingInKBOContext> { }
