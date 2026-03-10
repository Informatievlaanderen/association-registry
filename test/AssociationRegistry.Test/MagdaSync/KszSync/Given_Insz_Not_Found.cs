namespace AssociationRegistry.Test.MagdaSync.KszSync;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.MartenDb.Store;
using AssociationRegistry.Persoonsgegevens;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz;
using CommandHandling.MagdaSync.SyncKsz.Queries;
using Common.StubsMocksFakes.Faktories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class Given_Insz_Not_Found
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly Mock<IAggregateSession> _aggregateSessionMock;
    private readonly Fixture _fixture;

    public Given_Insz_Not_Found()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario =
            new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();
        _aggregateSessionMock = new Mock<IAggregateSession>();
        var magdaGeefPersoonService = Faktory.New(_fixture)
                                             .MagdaGeefPersoonService
                                             .ReturnsOverledenPersoon();
        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock.Setup(x => x.Get(It.IsAny<Insz>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        _sut = new SyncKszMessageHandler(
            new VzerVertegenwoordigerForInszQuery(
                persoonsgegevensRepoMock.Object,
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
    public void Then_No_Vereniging_Is_Loaded()
    {
        _aggregateSessionMock.Verify(
            x => x.Load<Vereniging>(It.IsAny<VCode>(), It.IsAny<CommandMetadata>(), It.IsAny<bool>(), It.IsAny<bool>()),
            Times.Never
        );
    }
}
