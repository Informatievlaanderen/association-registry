namespace AssociationRegistry.Test.MagdaSync.KszSync;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Persoonsgegevens;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Framework;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz;
using CommandHandling.MagdaSync.SyncKsz.Queries;
using Common.StubsMocksFakes.Faktories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Wolverine;
using Wolverine.Marten;
using Xunit;

public class Given_Insz_Only_In_Kbo_Verenigngen
{
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Insz_Only_In_Kbo_Verenigngen()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario();
        var vertegenwoordiger = _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1;
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        var magdaGeefPersoonService = Faktory.New(_fixture)
                                             .MagdaGeefPersoonService
                                             .ReturnsOverledenPersoon();
        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock
            .Setup(x => x.Get(Insz.Create(vertegenwoordiger.Insz), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                new[]
                {
                    _fixture.Create<VertegenwoordigerPersoonsgegevens>() with
                    {
                        VCode = _scenario.VCode,
                        Insz = vertegenwoordiger.Insz,
                        VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                    },
                }
            );

        var filterVzerOnylQueryMock = new Mock<IFilterVzerOnlyQuery>();

        filterVzerOnylQueryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<FilterVzerOnlyQueryFilter>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _sut = new SyncKszMessageHandler(
            new VzerVertegenwoordigerForInszQuery(
                persoonsgegevensRepoMock.Object,
                filterVzerOnylQueryMock.Object,
                NullLogger<VzerVertegenwoordigerForInszQuery>.Instance
            ),
            _aggregateSessionMock,
            magdaGeefPersoonService.Object,
            NullLogger<SyncKszMessageHandler>.Instance
        );

        _sut.Handle(
                new CommandEnvelope<SyncKszMessage>(
                    new SyncKszMessage(Insz.Hydrate(vertegenwoordiger.Insz), Guid.NewGuid()),
                    TestCommandMetadata.ForDigitaalVlaanderenProcess
                ),
                Mock.Of<IMartenOutbox>(),
                CancellationToken.None
            )
            .GetAwaiter()
            .GetResult();
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
        _aggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
