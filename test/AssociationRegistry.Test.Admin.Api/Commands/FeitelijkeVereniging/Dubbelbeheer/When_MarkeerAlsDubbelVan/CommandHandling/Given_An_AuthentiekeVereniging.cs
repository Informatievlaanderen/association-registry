namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.Dubbelbeheer.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.Acties.Dubbelbeheer.MarkeerAlsDubbelVan;
using AssociationRegistry.Framework;
using AssociationRegistry.Messages;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using FluentAssertions;
using Marten;
using Moq;
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
