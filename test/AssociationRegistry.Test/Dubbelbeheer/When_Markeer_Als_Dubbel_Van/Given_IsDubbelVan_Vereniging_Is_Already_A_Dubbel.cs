namespace AssociationRegistry.Test.Dubbelbeheer.When_Markeer_Als_Dubbel_Van;

using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Marten;
using MartenDb.Store;
using Moq;
using Resources;
using Vereniging;
using Wolverine.Marten;
using Xunit;

public class Given_IsDubbelVan_Vereniging_Is_Already_A_Dubbel
{
    [Fact]
    public async ValueTask Then_Throws_VerenigingKanGeenDubbelWordenVanDubbelVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var aggregateSession = new Mock<IAggregateSession>();
        var verenigingsStateQueriesMock = new Mock<IVerenigingStateQueryService>();
        var command = fixture.Create<MarkeerAlsDubbelVanCommand>();

        var commandEnvelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(
            Command: command,
            fixture.Create<CommandMetadata>()
        );

        verenigingsStateQueriesMock.Setup(s => s.Exists(command.VCodeAuthentiekeVereniging)).ReturnsAsync(true);
        verenigingsStateQueriesMock.Setup(s => s.IsVerwijderd(command.VCodeAuthentiekeVereniging)).ReturnsAsync(false);
        verenigingsStateQueriesMock.Setup(s => s.IsDubbel(command.VCodeAuthentiekeVereniging)).ReturnsAsync(true);

        var sut = new MarkeerAlsDubbelVanCommandHandler(
            aggregateSession: aggregateSession.Object,
            queryService: verenigingsStateQueriesMock.Object,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel>(
            async () =>
                await sut.Handle(message: commandEnvelope, cancellationToken: CancellationToken.None)
        );

        exception
            .Message.Should()
            .Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel);
    }
}
