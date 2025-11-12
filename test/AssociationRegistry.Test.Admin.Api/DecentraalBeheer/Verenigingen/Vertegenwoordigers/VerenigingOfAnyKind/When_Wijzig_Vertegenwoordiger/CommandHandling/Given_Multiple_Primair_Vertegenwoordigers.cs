namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using FluentAssertions;
using Xunit;

public class Given_Multiple_Primair_Vertegenwoordigers :
    WijzigVertegenwoordigerCommandHandlerTestBase<FeitelijkeVerenigingWerdGeregistreerdWithAPrimairVertegenwoordigerScenario>
{
    private MeerderePrimaireVertegenwoordigers ActualException;
    protected override WijzigVertegenwoordigerCommand CreateCommand() => new(
        Scenario.VCode,
        new WijzigVertegenwoordigerCommand.CommandVertegenwoordiger(
            Scenario.VertegenwoordigerWerdToegevoegd2.VertegenwoordigerId,
            Rol: null,
            Roepnaam: null,
            Email: null,
            Telefoon: null,
            Mobiel: null,
            SocialMedia: null,
            IsPrimair: true)); // <== changed value

    public override async Task ExecuteCommand()
    {
        try
        {
           await base.ExecuteCommand();
        }
        catch (MeerderePrimaireVertegenwoordigers e)
        {
            ActualException = e;
        }
    }

    [Fact]
    public async ValueTask Then_A_MultiplePrimaryVertegenwoordiger_Is_Thrown()
        => ActualException.Should().BeOfType<MeerderePrimaireVertegenwoordigers>();
}
