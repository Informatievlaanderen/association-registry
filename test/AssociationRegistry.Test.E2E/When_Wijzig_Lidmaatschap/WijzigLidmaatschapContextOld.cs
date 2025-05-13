namespace AssociationRegistry.Test.E2E.When_Wijzig_Lidmaatschap;

using Admin.Api.Verenigingen.Lidmaatschap.WijzigLidmaatschap.RequestModels;
using Admin.Schema;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Xunit;


// CollectionFixture for database setup ==> Context
[CollectionDefinition(nameof(WijzigLidmaatschapCollection))]
public class WijzigLidmaatschapCollection : ICollectionFixture<WijzigLidmaatschapContext>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
public class WijzigLidmaatschapContext : TestContextBase<LidmaatschapWerdToegevoegdScenario, WijzigLidmaatschapRequest>
{
    protected override LidmaatschapWerdToegevoegdScenario InitializeScenario()
        => new(new MultipleWerdGeregistreerdScenario());

    public WijzigLidmaatschapContext(FullBlownApiSetup apiSetup): base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(LidmaatschapWerdToegevoegdScenario scenario)
    {
        CommandResult = await new WijzigLidmaatschapRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }

}
