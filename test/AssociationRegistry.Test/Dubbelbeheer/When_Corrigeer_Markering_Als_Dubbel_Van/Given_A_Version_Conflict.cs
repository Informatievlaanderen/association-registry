namespace AssociationRegistry.Test.Dubbelbeheer.When_Corrigeer_Markering_Als_Dubbel_Van;

using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.CorrigeerMarkeringAlsDubbelVan;
using DecentraalBeheer.Vereniging;
using Events;
using MartenDb.Store;
using Moq;
using Xunit;

/// <summary>
///     Werd de dubbele of de authentieke vereniging gewijzigd tussen laden en bewaren,
///     dan worden beide opnieuw geladen en wordt alles opnieuw uitgevoerd.
/// </summary>
public class Given_A_Version_Conflict
{
    [Fact]
    public async ValueTask Then_It_Reloads_Both_Verenigingen_And_Saves_Again()
    {
        var fixture = new Fixture().CustomizeDomain();
        var aggregateSession = new Mock<IAggregateSession>();

        var authentiekeGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();
        var dubbeleGeregistreerd = fixture.Create<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>();

        var dubbele = new VerenigingState()
            .Apply(dubbeleGeregistreerd)
            .Apply(
                new VerenigingWerdGemarkeerdAlsDubbelVan(dubbeleGeregistreerd.VCode, authentiekeGeregistreerd.VCode)
            );

        var authentieke = new VerenigingState()
            .Apply(authentiekeGeregistreerd)
            .Apply(
                new VerenigingAanvaarddeDubbeleVereniging(authentiekeGeregistreerd.VCode, dubbeleGeregistreerd.VCode)
            );

        aggregateSession
            .Setup(x =>
                x.Load<Vereniging>(dubbele.VCode, It.IsAny<CommandMetadata>(), It.IsAny<bool>(), It.IsAny<bool>())
            )
            .ReturnsAsync(() => Hydrate<Vereniging>(dubbele));

        aggregateSession
            .Setup(x =>
                x.Load<VerenigingOfAnyKind>(
                    authentieke.VCode,
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()
                )
            )
            .ReturnsAsync(() => Hydrate<VerenigingOfAnyKind>(authentieke));

        aggregateSession
            .SetupSequence(x =>
                x.SaveTransactional(
                    It.IsAny<IReadOnlyCollection<VerenigingsBase>>(),
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ThrowsAsync(new UnexpectedAggregateVersionException())
            .ReturnsAsync([StreamActionResult.Empty, StreamActionResult.Empty]);

        await new CorrigeerMarkeringAlsDubbelVanCommandHandler(aggregateSession.Object).Handle(
            new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(
                new CorrigeerMarkeringAlsDubbelVanCommand(dubbele.VCode),
                fixture.Create<CommandMetadata>()
            ),
            CancellationToken.None
        );

        aggregateSession.Verify(
            x => x.Load<Vereniging>(dubbele.VCode, It.IsAny<CommandMetadata>(), It.IsAny<bool>(), It.IsAny<bool>()),
            Times.Exactly(2)
        );

        aggregateSession.Verify(
            x =>
                x.Load<VerenigingOfAnyKind>(
                    authentieke.VCode,
                    It.IsAny<CommandMetadata>(),
                    It.IsAny<bool>(),
                    It.IsAny<bool>()
                ),
            Times.Exactly(2)
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
