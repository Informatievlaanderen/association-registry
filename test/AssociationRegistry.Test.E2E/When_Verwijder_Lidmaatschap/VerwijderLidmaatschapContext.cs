namespace AssociationRegistry.Test.E2E.When_Verwijder_Lidmaatschap;

using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;
using Xunit;

public class VerwijderLidmaatschapContext : TestContextBase<LidmaatschapWerdToegevoegdScenario, NullRequest>
{
    protected override LidmaatschapWerdToegevoegdScenario InitializeScenario()
        => new(new MultipleWerdGeregistreerdScenario());
    public VerwijderLidmaatschapContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(LidmaatschapWerdToegevoegdScenario scenario)
    {
        CommandResult = await new VerwijderLidmaatschapRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VerwijderLidmaatschapCollection))]
public class VerwijderLidmaatschapCollection : ICollectionFixture<VerwijderLidmaatschapContext>
{
}
