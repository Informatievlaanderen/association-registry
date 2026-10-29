namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Xunit;

public class Given_Vereniging_InStopzetting
{
    private readonly Fixture _fixture;
    private readonly VzerWerdInStopzettingGeplaatstScenario _scenario;
    private readonly MultipleVerenigingAggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private readonly VCode _vCodeAuthentiekeVereniging;

    public Given_Vereniging_InStopzetting()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VzerWerdInStopzettingGeplaatstScenario();

        var authentiekeVereniging = _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        _vCodeAuthentiekeVereniging = VCode.Create(authentiekeVereniging.VCode);

        _aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(_scenario.GetVerenigingState());
        _aggregateSessionMock.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _aggregateSessionMock,
            new VerenigingsStateQueriesMock()
        );
    }

    [Fact]
    public async ValueTask Then_It_Saves_VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel()
    {
        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = _vCodeAuthentiekeVereniging,
        };

        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldHaveSaved(
            _scenario.VCode,
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, command.VCodeAuthentiekeVereniging),
            new VerenigingWerdUitInStopzettingGehaaldWegensVerenigingWerdGemarkeerdAlsDubbel()
        );
    }
}
