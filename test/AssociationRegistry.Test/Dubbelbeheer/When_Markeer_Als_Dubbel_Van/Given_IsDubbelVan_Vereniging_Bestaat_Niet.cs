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
using Moq;
using Resources;
using Vereniging;
using Wolverine.Marten;
using Xunit;

public class Given_IsDubbelVan_Vereniging_Bestaat_Niet
{
    [Fact]
    public async ValueTask Then_Throws_VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var verenigingsRepositoryMock = new Mock<IVerenigingsRepository>();
        var verenigingsStateQueriesMock = new Mock<IVerenigingStateQueryService>();
        var command = fixture.Create<MarkeerAlsDubbelVanCommand>();

        var commandEnvelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(
            Command: command,
            fixture.Create<CommandMetadata>()
        );

        verenigingsStateQueriesMock.Setup(s => s.Exists(command.VCodeAuthentiekeVereniging)).ReturnsAsync(false);

        var sut = new MarkeerAlsDubbelVanCommandHandler(
            verenigingsRepository: verenigingsRepositoryMock.Object,
            queryService: verenigingsStateQueriesMock.Object,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging>(async () =>
            await sut.Handle(message: commandEnvelope, cancellationToken: CancellationToken.None)
        );

        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging);
    }
}
