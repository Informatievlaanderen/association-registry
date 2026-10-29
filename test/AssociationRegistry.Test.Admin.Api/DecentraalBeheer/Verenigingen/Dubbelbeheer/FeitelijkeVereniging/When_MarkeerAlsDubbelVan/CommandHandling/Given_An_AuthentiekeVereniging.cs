namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Xunit;

/// <summary>
///     Een vereniging die eerder als authentiek fungeerde (A is dubbel van B) mag alsnog dubbel worden (B is dubbel van C).
/// </summary>
public class Given_An_Authentieke_Vereniging
{
    private readonly Fixture _fixture;
    private readonly VerenigingAanvaarddeDubbeleVerenigingScenario _scenario;
    private readonly MultipleVerenigingAggregateSessionMock _aggregateSessionMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private readonly MarkeerAlsDubbelVanCommand _command;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(
            VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke
        );

        var nieuweAuthentiekeVereniging =
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        _aggregateSessionMock = new MultipleVerenigingAggregateSessionMock(_scenario.GetVerenigingState());
        _aggregateSessionMock.WithVereniging(new VerenigingState().Apply(nieuweAuthentiekeVereniging));

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _aggregateSessionMock,
            new VerenigingsStateQueriesMock()
        );

        _command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = VCode.Create(nieuweAuthentiekeVereniging.VCode),
        };
    }

    private async Task Handle() =>
        await _commandHandler.Handle(
            new CommandEnvelope<MarkeerAlsDubbelVanCommand>(_command, _fixture.Create<CommandMetadata>())
        );

    [Fact]
    public async ValueTask Then_It_Saves_A_VerenigingWerdGemarkeerdAlsDubbelVan()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _scenario.VCode,
            new VerenigingWerdGemarkeerdAlsDubbelVan(_scenario.VCode, _command.VCodeAuthentiekeVereniging)
        );
    }

    [Fact]
    public async ValueTask Then_The_New_Authentieke_Vereniging_Aanvaardt_The_Dubbel()
    {
        await Handle();

        _aggregateSessionMock.ShouldHaveSaved(
            _command.VCodeAuthentiekeVereniging,
            new VerenigingAanvaarddeDubbeleVereniging(_command.VCodeAuthentiekeVereniging, _scenario.VCode)
        );
    }
}
