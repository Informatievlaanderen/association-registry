namespace AssociationRegistry.Test.MagdaSync.SyncKsz.When_SyncVertegenwoordigerCommand_Handled;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AssociationRegistry.Persoonsgegevens;
using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz;
using Common.AutoFixture;
using Common.Framework;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Xunit;

public class Given_Overleden_Vertegenwoordiger
{
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;

    public Given_Overleden_Vertegenwoordiger()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        var magdaMock = new Mock<IMagdaGeefPersoonService>();

        magdaMock.Setup(x => x.GeefPersoon(new GeefPersoonRequest(_scenario.VertegenwoordigerWerdToegevoegd.Insz),
                                      It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PersoonUitKsz(_scenario.VertegenwoordigerWerdToegevoegd.Insz,
                                                 Overleden: true));

        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock
            .Setup(x => x.Get(Insz.Create(_scenario.VertegenwoordigerWerdToegevoegd.Insz), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new VertegenwoordigerPersoonsgegevens(
                    Guid.NewGuid(),
                    _scenario.VCode,
                    _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    _scenario.VertegenwoordigerWerdToegevoegd.Insz,
                    _scenario.VertegenwoordigerWerdToegevoegd.Roepnaam,
                    _scenario.VertegenwoordigerWerdToegevoegd.Rol,
                    _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                    _scenario.VertegenwoordigerWerdToegevoegd.Achternaam,
                    _scenario.VertegenwoordigerWerdToegevoegd.Email,
                    _scenario.VertegenwoordigerWerdToegevoegd.Telefoon,
                    _scenario.VertegenwoordigerWerdToegevoegd.Mobiel,
                    _scenario.VertegenwoordigerWerdToegevoegd.SocialMedia)
            });

        _messageBusMock = new Mock<IMessageBus>();

        _sut = new SyncKszMessageHandler(
            persoonsgegevensRepoMock.Object,
            magdaMock.Object,
            _messageBusMock.Object,
            NullLogger<SyncKszMessageHandler>.Instance);
    }

    [Fact]
    public async ValueTask Then_It_Sends_MarkeerVertegenwoordigerAlsOverleden_Command()
    {
        await _sut.Handle(
            new SyncKszMessage(Insz.Create(_scenario.VertegenwoordigerWerdToegevoegd.Insz)),
            CancellationToken.None);

        _messageBusMock.Verify(x => x.SendAsync(
            It.Is<CommandEnvelope<MarkeerVertegenwoordigerAlsOverledenMessage>>(
                cmd => cmd.Command.VCode == _scenario.VCode &&
                       cmd.Command.VertegenwoordigerId == _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId),
            It.IsAny<DeliveryOptions?>()), Times.Once);
    }
}
