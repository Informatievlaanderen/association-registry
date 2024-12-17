namespace AssociationRegistry.Test.When_Markeer_Als_Dubbel_Van;

using Acties.MarkeerAlsDubbelVan;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using FluentAssertions;
using Marten;
using Moq;
using Resources;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;
using Xunit;

public class Given_IsDubbelVan_Vereniging_Bestaat_Niet
{
    [Fact]
    public async Task Then_Throws_VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var verenigingsRepositoryMock = new Mock<IVerenigingsRepository>();
        var command = fixture.Create<MarkeerAlsDubbelVanCommand>();
        var commandEnvelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, fixture.Create<CommandMetadata>());

        verenigingsRepositoryMock.Setup(s => s.Exists(command.VCodeAuthentiekeVereniging))
                                 .ReturnsAsync(false);

        var sut = new MarkeerAlsDubbelVanCommandHandler(verenigingsRepositoryMock.Object,
                                                        Mock.Of<IMartenOutbox>(),
                                                        Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging>
            (async () => await sut.Handle(commandEnvelope, CancellationToken.None));

        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanEenNietBestaandeVereniging);
    }
}
