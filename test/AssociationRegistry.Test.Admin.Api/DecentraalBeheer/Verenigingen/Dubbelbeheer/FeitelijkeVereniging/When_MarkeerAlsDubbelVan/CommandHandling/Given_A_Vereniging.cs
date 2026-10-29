namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Xunit;

public class Given_A_Vereniging
{
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly MultipleVerenigingAggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_A_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();

        var authentiekeVereniging = _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        _aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(_scenario.GetVerenigingState());
        _aggregateSessionMock.WithVereniging(new VerenigingState().Apply(authentiekeVereniging));

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _aggregateSessionMock,
            new VerenigingsStateQueriesMock()
        );

        _command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = VCode.Create(authentiekeVereniging.VCode),
        };
    }

    private async Task Handle() =>
        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>())
        );

    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingWerdGemarkeerdAlsDubbelVan_Event()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _scenario.VCode,
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, _command.VCodeAuthentiekeVereniging)
        );
    }

    [Fact]
    public async ValueTask Then_The_Authentieke_Vereniging_Aanvaardt_The_Dubbel()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _command.VCodeAuthentiekeVereniging,
            new VerenigingAanvaarddeDubbeleVereniging(_command.VCodeAuthentiekeVereniging, _scenario.VCode)
        );
    }

    [Fact]
    public async ValueTask Then_Both_Verenigingen_Are_Saved_In_One_Transaction()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSavedTransactional(_scenario.VCode, _command.VCodeAuthentiekeVereniging);
    }
}
