namespace AssociationRegistry.Test.E2E.When_Wijzig_Koepelgegevens;

using Admin.Api.WebApi.Verenigingen.Koepels.RequestModels;
using Framework.ApiSetup;
using Framework.TestClasses;
using Scenarios.Givens.MetRechtspersoonlijkheid;
using Scenarios.Requests.VerenigingMetRechtspersoonlijkheid;
using Xunit;

[CollectionDefinition(nameof(WijzigKoepelgegevensCollection))]
public class WijzigKoepelgegevensCollection : ICollectionFixture<WijzigKoepelgegevensContext> { }

public class WijzigKoepelgegevensContext
    : TestContextBase<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario, WijzigKoepelgegevensRequest>
{
    public WijzigKoepelgegevensContext(FullBlownApiSetup apiSetup)
        : base(apiSetup) { }

    protected override VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario InitializeScenario() => new();

    protected override async ValueTask ExecuteScenario(
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario scenario
    )
    {
        CommandResult = await new WijzigKoepelgegevensRequestFactory(scenario).ExecuteRequest(ApiSetup);
    }
}
