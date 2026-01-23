namespace AssociationRegistry.Test.Dubbelbeheer.When_Corrigeer_Markering_Als_Dubbel_Van;

using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Dubbelbeheer.Commands.CorrigeerMarkeringAlsDubbelVan;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Exceptions;
using FluentAssertions;
using Marten;
using MartenDb.Store;
using Moq;
using Wolverine.Marten;
using Xunit;

public class Given_VerenigingMetRechtspersoonlijkheid
{
    [Fact]
    public async ValueTask Then_Throws_VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario();
        var aggregateSession = new AggregateSessionMock(scenario.GetVerenigingState());
        var command = fixture.Create<CorrigeerMarkeringAlsDubbelVanCommand>() with
        {
            VCode = VCode.Create(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
        };
        var commandEnvelope = new CommandEnvelope<CorrigeerMarkeringAlsDubbelVanCommand>(
            command,
            fixture.Create<CommandMetadata>()
        );

        var sut = new CorrigeerMarkeringAlsDubbelVanCommandHandler(
            aggregateSession,
            Mock.Of<IMartenOutbox>(),
            Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorVerenigingstype>(async () =>
            await sut.Handle(commandEnvelope, CancellationToken.None)
        );

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForVerenigingstype);
    }
}
