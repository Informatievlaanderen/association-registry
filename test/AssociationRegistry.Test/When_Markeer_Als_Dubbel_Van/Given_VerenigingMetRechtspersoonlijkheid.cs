namespace AssociationRegistry.Test.When_Markeer_Als_Dubbel_Van;

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
using Vereniging;
using Vereniging.Exceptions;
using Wolverine.Marten;
using Xunit;

public class Given_VerenigingMetRechtspersoonlijkheid
{
    [Fact]
    public async Task Then_Throws_VerenigingKanGeenDubbelWordenVanVerwijderdeVereniging()
    {
        var fixture = new Fixture().CustomizeDomain();
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_AllFields_Scenario();
        var verenigingsRepositoryMock = new VerenigingRepositoryMock(scenario.GetVerenigingState());
        var command = fixture.Create<MarkeerAlsDubbelVanCommand>() with
        {
            VCode = VCode.Create(scenario.VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode),
        };
        var commandEnvelope = new CommandEnvelope<MarkeerAlsDubbelVanCommand>(command, fixture.Create<CommandMetadata>());

        var sut = new MarkeerAlsDubbelVanCommandHandler(verenigingsRepositoryMock,
                                                        Mock.Of<IMartenOutbox>(),
                                                        Mock.Of<IDocumentSession>()
        );

        var exception = await Assert.ThrowsAsync<ActieIsNietToegestaanVoorVerenigingstype>
            (async () => await sut.Handle(commandEnvelope, CancellationToken.None));

        exception.Message.Should().Be(ExceptionMessages.UnsupportedOperationForVerenigingstype);
    }
}
