namespace AssociationRegistry.Test.E2E.When_Verwijder_Maatschappelijke_Zetel_Uit_KBO;

using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class VerwijderMaatschappelijkeZetelUitKBOContext
    : TestContextBase<MaatschappelijkeZetelWerdVerwijderdUitKboScenario, NullRequest>
{
    protected override MaatschappelijkeZetelWerdVerwijderdUitKboScenario InitializeScenario() => new();

    public VerwijderMaatschappelijkeZetelUitKBOContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(MaatschappelijkeZetelWerdVerwijderdUitKboScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(
            VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
            new NullRequest()
        );
    }
}

[CollectionDefinition(nameof(VerwijderMaatschappelijkeZetelUitKBOCollection))]
public class VerwijderMaatschappelijkeZetelUitKBOCollection
    : ICollectionFixture<VerwijderMaatschappelijkeZetelUitKBOContext> { }
