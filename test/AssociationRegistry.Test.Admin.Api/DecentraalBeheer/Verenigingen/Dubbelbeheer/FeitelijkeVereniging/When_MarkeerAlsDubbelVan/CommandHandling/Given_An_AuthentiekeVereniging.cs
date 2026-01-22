namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Dubbelbeheer.FeitelijkeVereniging.When_MarkeerAlsDubbelVan.CommandHandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.MarkeerAlsDubbelVan;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Reacties.AanvaardDubbel;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AutoFixture;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Marten;
using Moq;
using Resources;
using Vereniging;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class Given_An_Authentieke_Vereniging
{
    private readonly Fixture _fixture;

    public Given_An_Authentieke_Vereniging()
    {
        _fixture = new Fixture().CustomizeDomain();
    }

    [Theory]
    [InlineData(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype.Feitelijke)]
    public async ValueTask Then_It_Throws(VerenigingAanvaarddeDubbeleVerenigingScenario.Verenigingstype verenigingstype)
    {
        var scenario = new VerenigingAanvaarddeDubbeleVerenigingScenario(verenigingstype);

        var command = _fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = scenario.VCode,
            VCodeAuthentiekeVereniging = _fixture.Create<VCode>(),
        };

        var verenigingRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var verenigingsStateQueriesMock = new VerenigingsStateQueriesMock(scenario.GetVerenigingState());

        var commandHandler = new MarkeerAlsDubbelVanCommandHandler(
            verenigingsRepository: verenigingRepositoryMock,
            queryService: verenigingsStateQueriesMock,
            outbox: new Mock<IMartenOutbox>().Object,
            Mock.Of<IDocumentSession>()
        );

        var commandEnvelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(
            Command: command,
            _fixture.Create<CommandMetadata>()
        );

        var exception = await Assert.ThrowsAsync<AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden>(async () =>
            await commandHandler.Handle(commandEnvelope)
        );

        exception.Message.Should().Be(ExceptionMessages.AuthentiekeVerenigingKanNietAlsDubbelGemarkeerdWorden);

        new Mock<IMartenOutbox>().Verify(
            expression: x => x.SendAsync(It.IsAny<AanvaardDubbeleVerenigingMessage>(), It.IsAny<DeliveryOptions>()),
            times: Times.Never
        );
    }
}
