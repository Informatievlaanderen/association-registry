namespace AssociationRegistry.Test.E2E.When_Voeg_Lidmaatschap_Toe;

using Admin.Api.Verenigingen.Lidmaatschap.VoegLidmaatschapToe.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Marten.Events;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Scenarios.Givens.FeitelijkeVereniging;
using Scenarios.Requests.FeitelijkeVereniging;
using Vereniging;
using Xunit;

public class VoegLidmaatschapToeContext : TestContextBase<MultipleWerdGeregistreerdScenario, VoegLidmaatschapToeRequest>
{

    protected override MultipleWerdGeregistreerdScenario InitializeScenario()
        => new();

    public VoegLidmaatschapToeContext(FullBlownApiSetup apiSetup) : base(apiSetup)
    {
    }

    protected override async ValueTask ExecuteScenario(MultipleWerdGeregistreerdScenario scenario)
    {
        CommandResult = await new VoegLidmaatschapToeRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}

[CollectionDefinition(nameof(VoegLidmaatschapToeCollection))]
public class VoegLidmaatschapToeCollection : ICollectionFixture<VoegLidmaatschapToeContext>
{
}
