namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Subtype.CommandHandling.VerfijnNaarSubvereniging;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Subtype;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Subtypes.Subvereniging;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using MartenDb.Store;
using Moq;
using Resources;
using Vereniging;
using Xunit;

public class Given_Andere_Vereniging_Is_Loaded
{
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _rechtspersoonScenario;
    private readonly Mock<IAggregateSession> _verenigingRepositoryMock;

    public Given_Andere_Vereniging_Is_Loaded()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        _rechtspersoonScenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _verenigingRepositoryMock = new Mock<IAggregateSession>();
    }

    [Fact]
    public async ValueTask With_Verwijderde_Andere_Vereniging_Then_Throws_AndereVerenigingIsVerwijderd()
    {
        _verenigingRepositoryMock
            .Setup(x =>
                x.Load<VerenigingMetRechtspersoonlijkheid>(
                    _rechtspersoonScenario.VCode,
                    It.IsAny<CommandMetadata>(),
                    false,
                    false
                )
            )
            .ThrowsAsync(new VerenigingIsVerwijderd(_rechtspersoonScenario.VCode));

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(_verenigingRepositoryMock.Object);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(
            _scenario.VCode,
            new SubverenigingVanDto(_rechtspersoonScenario.VCode, null, null)
        );

        var exception = await Assert.ThrowsAnyAsync<AndereVerenigingIsVerwijderd>(() =>
            commandHandler.Handle(
                new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>())
            )
        );

        exception.Message.Should().Be(ExceptionMessages.AndereVerenigingIsVerwijderd);
    }

    [Fact]
    public async ValueTask With_VZER_As_Andere_Vereniging_Then_Throws_ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype()
    {
        _verenigingRepositoryMock
            .Setup(x =>
                x.Load<VerenigingMetRechtspersoonlijkheid>(It.IsAny<VCode>(), It.IsAny<CommandMetadata>(), false, false)
            )
            .ThrowsAsync(new ActieIsNietToegestaanVoorVerenigingstype());

        var commandHandler = new VerfijnSubtypeNaarSubverenigingCommandHandler(_verenigingRepositoryMock.Object);

        var command = new VerfijnSubtypeNaarSubverenigingCommand(
            _scenario.VCode,
            new SubverenigingVanDto(_fixture.Create<VCode>(), null, null)
        );

        var exception = await Assert.ThrowsAnyAsync<ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype>(() =>
            commandHandler.Handle(
                new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, _fixture.Create<CommandMetadata>())
            )
        );

        exception.Message.Should().Be(ExceptionMessages.ActieIsNietToegestaanVoorAndereVerenigingVerenigingstype);
    }
}
