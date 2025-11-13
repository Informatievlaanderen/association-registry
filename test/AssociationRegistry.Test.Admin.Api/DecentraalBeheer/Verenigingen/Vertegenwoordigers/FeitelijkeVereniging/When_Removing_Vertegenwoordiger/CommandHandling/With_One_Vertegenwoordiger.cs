namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.When_Removing_Vertegenwoordiger.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VerwijderVertegenwoordiger;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Xunit;

public class With_One_Vertegenwoordiger : VerwijderVertegenwoordigerCommandHandlerTestBase<FeitelijkeVerenigingWerdGeregistreerdWithOneVertegenwoordigerScenario>
{
    public override async Task ExecuteCommand()
    {
        try
        {
            await base.ExecuteCommand();
        }
        catch (LaatsteVertegenwoordigerKanNietVerwijderdWorden e)
        {
            ActualException = e;
        }
    }

    public LaatsteVertegenwoordigerKanNietVerwijderdWorden ActualException { get; set; }

    [Fact]
    public async ValueTask Then_A_MultiplePrimaryVertegenwoordiger_Is_Thrown()
        => ActualException.Should().BeOfType<LaatsteVertegenwoordigerKanNietVerwijderdWorden>();

    protected override VerwijderVertegenwoordigerCommand CreateCommand()
    {
        var unknownVertegenwoordigerId = Scenario.VertegenwoordigerId;

        return new VerwijderVertegenwoordigerCommand(Scenario.VCode, unknownVertegenwoordigerId);
    }
}
