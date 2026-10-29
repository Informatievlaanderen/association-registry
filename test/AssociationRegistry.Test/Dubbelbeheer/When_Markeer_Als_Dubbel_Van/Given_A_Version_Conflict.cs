namespace AssociationRegistry.Test.Dubbelbeheer.When_Markeer_Als_Dubbel_Van;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events;
using FluentAssertions;
using MartenDb.Store;
using Moq;
using Xunit;

/// <summary>
///     Werd de dubbele of de authentieke vereniging gewijzigd tussen laden en bewaren,
///     dan worden beide opnieuw geladen en wordt alles opnieuw uitgevoerd.
/// </summary>
public class Given_A_Version_Conflict
{
    private readonly Fixture _fixture = new Fixture().CustomizeDomain();
    private readonly Mock<IAggregateSession> _aggregateSession = new();
    private readonly CommandEnvelope<MarkeerAlsDubbelVanCommand> _envelope;

    public Given_A_Version_Conflict()
    {
        var dubbele = new VerenigingState().Apply(
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
        );

        var authentieke = new VerenigingState().Apply(
            _fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
        );

        _aggregateSession
            .Setup(x =>
                x.Load<Vereniging>(dubbele.VCode, It.IsAny<CommandMetadata>(), It.IsAny<bool>(), It.IsAny<bool>())
            )
            .ReturnsAsync(() => Hydrate<Vereniging>(dubbele));

        _aggregateSession
            .Setup(x =>
                x.Load<VerenigingOfAnyKind>(
                    authentieke.VCode,
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(() => Hydrate<VerenigingOfAnyKind>(authentieke));

        _envelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(
            new MarkeerAlsDubbelVanCommand(dubbele.VCode, authentieke.VCode),
            _fixture.Create<CommandMetadata>()
        );
    }

    private MarkeerAlsDubbelVanCommandHandler CreateHandler() =>
        new(_aggregateSession.Object, new VerenigingsStateQueriesMock());

    [Fact]
    public async ValueTask Then_It_Reloads_Both_Verenigingen_And_Saves_Again()
    {
        _aggregateSession
            .SetupSequence(x =>
                x.SaveTransactional(
                    It.IsAny<IReadOnlyCollection<VerenigingsBase>>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new UnexpectedAggregateVersionException())
            .ReturnsAsync([StreamActionResult.Empty, StreamActionResult.Empty]);

        await CreateHandler().Handle(_envelope, CancellationToken.None);

        _aggregateSession.Verify(
            x =>
                x.Load<Vereniging>(
                    _envelope.Command.VCode,
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()
                ),
            Times.Exactly(2)
        );

        _aggregateSession.Verify(
            x =>
                x.Load<VerenigingOfAnyKind>(
                    _envelope.Command.VCodeAuthentiekeVereniging,
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()
                ),
            Times.Exactly(2)
        );
    }

    [Fact]
    public async ValueTask When_The_Conflict_Keeps_Occurring_Then_It_Throws_After_The_Retries()
    {
        _aggregateSession
            .Setup(x =>
                x.SaveTransactional(
                    It.IsAny<IReadOnlyCollection<VerenigingsBase>>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new UnexpectedAggregateVersionException());

        var act = async () => await CreateHandler().Handle(_envelope, CancellationToken.None);

        await act.Should().ThrowAsync<UnexpectedAggregateVersionException>();

        _aggregateSession.Verify(
            x =>
                x.SaveTransactional(
                    It.IsAny<IReadOnlyCollection<VerenigingsBase>>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Exactly(1 + DubbelbeheerRetryPolicy.RetryCount)
        );
    }

    private static T Hydrate<T>(VerenigingState state)
        where T : IHydrate<VerenigingState>, new()
    {
        var vereniging = new T();
        vereniging.Hydrate(state);

        return vereniging;
    }
}
