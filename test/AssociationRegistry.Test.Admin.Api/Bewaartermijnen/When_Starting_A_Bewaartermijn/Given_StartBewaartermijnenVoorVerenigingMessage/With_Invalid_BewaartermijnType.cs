namespace AssociationRegistry.Test.Admin.Api.Bewaartermijnen.When_Starting_A_Bewaartermijn.Given_StartBewaartermijnenVoorVerenigingMessage;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen.Messages;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.Bewaartermijnen.Acties.Start;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Events;
using FluentAssertions;
using Integrations.Grar.Bewaartermijnen;
using Moq;
using NodaTime;
using Resources;
using Xunit;

public class With_Invalid_BewaartermijnType
{
    [Fact]
    public async Task Then_Throws_OngeldigBewaartermijnType()
    {
        var fixture = new Fixture();

        var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;
        var scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var bewaartermijnOptions = new BewaartermijnOptions() { Duration = TimeSpan.FromDays(1) };
        var expectedVervaldag = commandMetadata.Tijdstip.PlusTicks(bewaartermijnOptions.Duration.Ticks);

        var invalidPersoonsgegevensType = fixture.Create<string>();

        var message = new StartBewaartermijnenVoorVerenigingMessage(
            scenario.VCode,
            invalidPersoonsgegevensType,
            expectedVervaldag,
            BewaartermijnReden.VerenigingWerdVerwijderd
        );

        var commandHandler = new StartBewaartermijnenVoorVerenigingMessageHandler();
        var eventStore = new Mock<IEventStore>();

        eventStore.Setup(x => x.Load<VerenigingState>(scenario.VCode, null)).ReturnsAsync(scenario.GetVerenigingState);

        var exception = await Assert.ThrowsAsync<OngeldigBewaartermijnType>(() =>
            commandHandler.Handle(message, eventStore.Object, CancellationToken.None)
        );

        exception.Message.Should().Be(ExceptionMessages.OngeldigBewaartermijnType);
    }
}
