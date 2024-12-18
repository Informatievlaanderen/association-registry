namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using Acties.MarkeerAlsDubbelVan;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling;
using FluentAssertions;
using Marten;
using Moq;
using Resources;
using Vereniging.Exceptions;
using Wolverine.Marten;
using Xunit;
using Xunit.Categories;

[UnitTest]
public class Given_VCode_Equals_IsDubbelVan
{
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly VerenigingRepositoryMock _verenigingRepositoryMock;
    private readonly MarkeerAlsDubbelVanCommandHandler _commandHandler;

    public Given_VCode_Equals_IsDubbelVan()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        _commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            _verenigingRepositoryMock,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>()
        );
    }

    [Fact]
    public async Task Then_Throws_VerenigingKanGeenDubbelWordenVanZichzelf()
    {
        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = _scenario.VCode,
            VCodeAuthentiekeVereniging = _scenario.VCode,
        };

        var exception = await Assert
           .ThrowsAsync<VerenigingKanGeenDubbelWordenVanZichzelf>
            (async () => await _commandHandler.Handle(
                 new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, _fixture.Create<CommandMetadata>())));
        exception.Message.Should().Be(ExceptionMessages.VerenigingKanGeenDubbelWordenVanZichzelf);
    }
}
