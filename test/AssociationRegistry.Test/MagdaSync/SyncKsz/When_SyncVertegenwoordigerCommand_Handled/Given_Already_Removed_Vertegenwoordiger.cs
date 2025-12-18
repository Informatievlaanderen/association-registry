namespace AssociationRegistry.Test.MagdaSync.SyncKsz.When_SyncVertegenwoordigerCommand_Handled;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AssociationRegistry.Persoonsgegevens;
using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Xunit;

public class Given_Already_Removed_Vertegenwoordiger
{
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMagdaGeefPersoonService> _magdaMock;
    private readonly Mock<IMessageBus> _messageBusMock;

    public Given_Already_Removed_Vertegenwoordiger()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        _magdaMock = new Mock<IMagdaGeefPersoonService>();

        // Return empty array - vertegenwoordiger was already removed, so no persoonsgegevens exist
        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock
            .Setup(x => x.Get(Insz.Create(_scenario.VertegenwoordigerWerdToegevoegd.Insz), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<VertegenwoordigerPersoonsgegevens>());

        _messageBusMock = new Mock<IMessageBus>();

        _sut = new SyncKszMessageHandler(
            persoonsgegevensRepoMock.Object,
            _magdaMock.Object,
            _messageBusMock.Object,
            NullLogger<SyncKszMessageHandler>.Instance);
    }

    [Fact]
    public async ValueTask Then_It_Sends_No_Command()
    {
        await _sut.Handle(
            new SyncKszMessage(Insz.Create(_scenario.VertegenwoordigerWerdToegevoegd.Insz)),
            CancellationToken.None);

        _messageBusMock.Verify(x => x.SendAsync(
            It.IsAny<object>(),
            It.IsAny<DeliveryOptions?>()), Times.Never);
    }

    [Fact]
    public async ValueTask Then_It_Does_Not_Sync_With_Ksz()
    {
        await _sut.Handle(
            new SyncKszMessage(Insz.Create(_scenario.VertegenwoordigerWerdToegevoegd.Insz)),
            CancellationToken.None);

        _magdaMock.Verify(x => x.GeefPersoon(It.IsAny<GeefPersoonRequest>(),
                                       It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
