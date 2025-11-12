namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using Xunit;

public class Given_Null_Values_Does_Not_Update_Anything :
    WijzigVertegenwoordigerCommandHandlerTestBase<FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario>
{
    protected override WijzigVertegenwoordigerCommand CreateCommand() => new(
        Scenario.VCode,
        new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
            Scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
            Rol: null,
            Roepnaam: null,
            Email: null,
            Telefoon: null,
            Mobiel: null,
            SocialMedia: null,
            IsPrimair: null));

    [Fact]
    public async ValueTask Then_It_Does_Not_Update_Anything()
    {
        VerenigingRepositoryMock.ShouldNotHaveAnySaves();
    }
}
