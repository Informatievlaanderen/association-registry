namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using Acties.MarkeerAlsDubbelVan;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Marten;
using Messages;
using Moq;
using Resources;
using Vereniging;
using Vereniging.Exceptions;
using Wolverine;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_An_Authentieke_Vereniging
{
    private readonly Fixture _fixture;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Theory]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke)]
    public async Task Then_It_Throws(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype verenigingstype)
    {
        var scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(verenigingstype);

        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = scenario.VCode,
            VCodeAuthentiekeVereniging = _fixture.Create<VCode>(),
        };

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());

        var commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            verenigingRepositoryMock,
            new Mock<IMartenOutbox>().Object,
            Mock.Of<IDocumentSession>()
        );
        var commandEnvelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>());

       var exception = await Assert.ThrowsAsync<AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden>(async () => await commandHandler.Handle(commandEnvelope));

       exception.Message.Should().Be(ExceptionMessages.AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden);

       new Mock<IMartenOutbox>().Verify(x => x.SendAsync(It.IsAny<AanvaardDubbeleVerenigingMessage>(), It.IsAny<DeliveryOptions>()), Times.Never);
    }
}
