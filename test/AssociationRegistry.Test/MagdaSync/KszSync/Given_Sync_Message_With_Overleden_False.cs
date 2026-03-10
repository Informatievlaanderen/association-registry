namespace AssociationRegistry.Test.MagdaSync;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.MartenDb.Store;
using AssociationRegistry.Persoonsgegevens;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.Faktories;
using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz;
using CommandHandling.MagdaSync.SyncKsz.Queries;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class Given_Sync_Message_With_Overleden_False
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly Mock<IAggregateSession> _aggregateSessionMock;
    private readonly Mock<IVertegenwoordigerPersoonsgegevensRepository> _vertegenwoordigerPersoonsgegevensRepository;

    public Given_Sync_Message_With_Overleden_False()
    {
        _scenario =
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        var magdaGeefPersoonService = Faktory.New(new Fixture().CustomizeDomain())
                                                     .MagdaGeefPersoonService
                                                     .ReturnsNietOverledenPersoon();
        _vertegenwoordigerPersoonsgegevensRepository = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        _aggregateSessionMock = new Mock<IAggregateSession>();

        _sut = new SyncKszMessageHandler(
            new VzerVertegenwoordigerForInszQuery(
                _vertegenwoordigerPersoonsgegevensRepository.Object,
                Mock.Of<IFilterVzerOnlyQuery>(),
                NullLogger<VzerVertegenwoordigerForInszQuery>.Instance
            ),
            _aggregateSessionMock.Object,
            magdaGeefPersoonService.Object,
            NullLogger<SyncKszMessageHandler>.Instance
        );

        _sut.Handle(
                new CommandEnvelope<SyncKszMessage>(
                    new SyncKszMessage(
                        Insz.Hydrate(_scenario.VertegenwoordigerWerdToegevoegd.Insz),
                        Guid.NewGuid()
                    ),
                    TestCommandMetadata.ForDigitaalVlaanderenProcess
                ),
                Mock.Of<IMartenOutbox>(),
                CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_No_Persoonsgegevens_Are_Retrieved()
    {
        _vertegenwoordigerPersoonsgegevensRepository.Verify(
            x => x.Get(It.IsAny<Insz>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Fact]
    public void Then_No_Vereniging_Is_Loaded()
    {
        _aggregateSessionMock.Verify(
            x => x.Load<Vereniging>(It.IsAny<VCode>(), It.IsAny<CommandMetadata>(), It.IsAny<bool>(), It.IsAny<bool>()),
            Times.Never
        );
    }
}
