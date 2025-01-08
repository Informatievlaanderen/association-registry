namespace AssociationRegistry.Test.Admin.Api.Commands.FeitelijkeVereniging.Dubbelbeheer.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Vereniging.Exceptions;
using AutoFixture;
using DecentraalBeheer.Dubbelbeheer.MarkeerAlsDubbelVan;
using FluentAssertions;
using Marten;
using Moq;
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
