namespace AssociationRegistry.Test.E2E.When_Wijzig_Maatschappelijke_Zetel_In_KBO;

using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class WijzigMaatschappleijkeZetelInKBOContext
    : TestContextBase<MaatschappelijkeZetelWerdGewijzigdInKboScenario, NullRequest>
{
    protected override MaatschappelijkeZetelWerdGewijzigdInKboScenario InitializeScenario() => new();

    public WijzigMaatschappleijkeZetelInKBOContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(MaatschappelijkeZetelWerdGewijzigdInKboScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(
            VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
            new NullRequest()
        );
    }
}

[CollectionDefinition(nameof(WijzigMaatschappleijkeZetelInKBOCollection))]
public class WijzigMaatschappleijkeZetelInKBOCollection
    : ICollectionFixture<WijzigMaatschappleijkeZetelInKBOContext> { }
