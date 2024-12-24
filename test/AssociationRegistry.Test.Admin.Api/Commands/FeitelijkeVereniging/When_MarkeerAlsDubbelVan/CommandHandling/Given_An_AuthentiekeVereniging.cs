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
    private readonly VerenigingAanvaarddeDubbeleVerenigingScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;
    private Mock<IMartenOutbox> _martenOutbox;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _martenOutbox = new Mock<IMartenOutbox>();

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _verenigingRepositoryMock,
            _martenOutbox.Object,
            Mock.Of<IDocumentSession>()
        );
    }

    [Fact]
    public async Task Then_It_Throws()
    {
        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = _fixture.Create<VCode>(),
        };

       var exception = await Assert.ThrowsAsync<AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden>(async () => await _commandHandler.Handle(new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())));

       exception.Message.Should().Be(ExceptionMessages.AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden);

       _martenOutbox.Verify(x => x.SendAsync(It.IsAny<AanvaardDubbeleVerenigingMessage>(), It.IsAny<DeliveryOptions>()), Times.Never);
    }
}
