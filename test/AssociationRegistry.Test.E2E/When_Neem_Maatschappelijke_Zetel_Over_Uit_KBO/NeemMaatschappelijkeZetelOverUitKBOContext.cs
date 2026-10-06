namespace AssociationRegistry.Test.E2E.When_Neem_Maatschappelijke_Zetel_Over_Uit_KBO;

using DecentraalBeheer.Vereniging;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Xunit;

public class NeemMaatschappelijkeZetelOverUitKBOContext
    : TestContextBase<MaatschappelijkeZetelWerdOvergenomenUitKboScenario, NullRequest>
{
    protected override MaatschappelijkeZetelWerdOvergenomenUitKboScenario InitializeScenario() => new();

    public NeemMaatschappelijkeZetelOverUitKBOContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(MaatschappelijkeZetelWerdOvergenomenUitKboScenario scenario)
    {
        CommandResult = new CommandResult<NullRequest>(
            VCode.Hydrate(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
            new NullRequest()
        );
    }
}

[CollectionDefinition(nameof(NeemMaatschappelijkeZetelOverUitKBOCollection))]
public class NeemMaatschappelijkeZetelOverUitKBOCollection
    : ICollectionFixture<NeemMaatschappelijkeZetelOverUitKBOContext> { }
