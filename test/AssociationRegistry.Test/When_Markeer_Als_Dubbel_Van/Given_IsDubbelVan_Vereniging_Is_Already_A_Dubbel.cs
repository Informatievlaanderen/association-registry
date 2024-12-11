namespace AssociationRegistry.Test.When_Markeer_Als_Dubbel_Van;

using Acties.MarkeerAlsDubbelVan;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Marten;
using Moq;
using Vereniging;
using Vereniging.Exceptions;
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

        verenigingsRepositoryMock.Setup(s => s.IsDubbel(command.VCodeAuthentiekeVereniging))
                                 .ReturnsAsync(true);

        var sut = new MarkeerAlsDubbelVanCommandHandler(verenigingsRepositoryMock.Object,
                                                        Mock.Of<IMartenOutbox>(),
                                                        Mock.Of<IDocumentSession>()
        );

        await Assert.ThrowsAsync<VerenigingKanGeenDubbelWordenVanDubbelVereniging>
            (async () => await sut.Handle(commandEnvelope, CancellationToken.None));
    }
}
