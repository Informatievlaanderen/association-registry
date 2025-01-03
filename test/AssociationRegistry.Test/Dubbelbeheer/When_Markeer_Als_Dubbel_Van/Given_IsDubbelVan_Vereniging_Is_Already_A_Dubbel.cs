namespace AssociationRegistry.Test.Dubbelbeheer.When_Markeer_Als_Dubbel_Van;

using AssociationRegistry.Acties.Dubbelbeheer.MarkeerAlsDubbelVan;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Marten;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_IsDubbelVan_Vereniging_Is_Already_A_Dubbel
{
    [Fact]
    public async Task Then_Throws_VerenigingKanGeenDubbelWordenVanDubbelVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var verenigingsRepositoryMock = new Mock<IVerenigingsRepository>();
        var command = fixture.Create<MarkeerAlsDubbelVanCommand>();
        var commandEnvelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, fixture.Create<CommandMetadata>());

        verenigingsRepositoryMock.Setup(s => s.Exists(command.VCodeAuthentiekeVereniging))
                                 .ReturnsAsync(true);
        verenigingsRepositoryMock.Setup(s => s.IsVerwijderd(command.VCodeAuthentiekeVereniging))
                                 .ReturnsAsync(false);
        verenigingsRepositoryMock.Setup(s => s.IsDubbel(command.VCodeAuthentiekeVereniging))
                                 .ReturnsAsync(true);

        var sut = new MarkeerAlsDubbelVanCommandHandler(verenigingsRepositoryMock.Object,
                                                        Mock.Of<IMartenOutbox>(),
                                                        Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel>
            (async () => await sut.Handle(commandEnvelope, CancellationToken.None));

        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanEenVerenigingReedsGemarkeerdAlsDubbel);
    }
}
