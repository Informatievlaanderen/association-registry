namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_CorrigeerMarkeringAlsDubbelVan.CommandHandling;

using Acties.CorrigeerMarkeringAlsDubbelVan;
using AssociationRegistry.Framework;
using Messages;
using Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using Vereniging.Exceptions;
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
    private readonly VerenigingAanvaarddeDubbeleVerenigingScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly CorrigeerMarkeringAlsDubbelVanCommandHandler _commandHandler;
    private Mock<IMartenOutbox> _martenOutbox;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _martenOutbox = new Mock<IMartenOutbox>();

        _commandHandler = new CorrigeerMarkeringAlsDubbelVanCommandHandler(
            _verenigingRepositoryMock,
            _martenOutbox.Object,
            Mock.Of<IDocumentSession>()
        );
    }

    [Fact]
    public async Task Then_It_Throws()
    {
        var command = _fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
        };

       var exception = await Assert.ThrowsAsync<AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden>(async () => await _commandHandler.Handle(new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())));

       exception.Message.Should().Be(ExceptionMessages.AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden);

       _martenOutbox.Verify(x => x.SendAsync(It.IsAny<AanvaardDubbeleVerenigingMessage>(), It.IsAny<DeliveryOptions>()), Times.Never);
    }
}
