namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.VerenigingOfAnyKind.When_Wijzig_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.WijzigVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using FluentAssertions;
using AutoFixture;
using Xunit;

public class With_An_Unknown_VertegenwoordigerId :
    WijzigVertegenwoordigerCommandHandlerTestBase<FeitelijkeVerenigingWerdGeregistreerdWithMinimalFields>
{
    protected override WijzigVertegenwoordigerCommand CreateCommand() => new(
        Scenario.VCode,
        Fixture.Create<WijzigVertegenwoordigerCommand.CommandVertegenwoordiger>());

    public override async Task ExecuteCommand()
    {
        try
        {
            await base.ExecuteCommand();
        }
        catch (VertegenwoordigerIsNietGekend e)
        {
            ActualException = e;
        }
    }

    public VertegenwoordigerIsNietGekend ActualException { get; set; }

    [Fact]
    public async ValueTask Then_A_UnknownVertegenoordigerException_Is_Thrown()
        => ActualException.Should().BeOfType<VertegenwoordigerIsNietGekend>();
}
