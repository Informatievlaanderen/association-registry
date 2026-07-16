namespace AssociationRegistry.Test.E2E.When_Archiving_Stream;

using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.VerenigingZonderEigenRechtspersoonlijkheid;
using Scenarios.Requests;
using Scenarios.Requests.SuperAdmin;
using Xunit;

public class ArchiveStreamContext
    : TestContextBase<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario, NullRequest>
{
    protected override VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario() =>
        new();

    public ArchiveStreamContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override async ValueTask ExecuteScenario(
        VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario scenario
    ) => CommandResult = await new ArchiveStreamRequestFactory(scenario).ExecuteRequest(ApiSetup);
}

[CollectionDefinition(nameof(ArchiveStreamCollection))]
public class ArchiveStreamCollection : ICollectionFixture<ArchiveStreamContext> { }
