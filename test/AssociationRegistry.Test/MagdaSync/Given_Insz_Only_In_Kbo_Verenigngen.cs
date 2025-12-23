namespace AssociationRegistry.Test.MagdaSync;

using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using KboMutations.SyncLambda.MagdaSync.SyncKsz;
using KboMutations.SyncLambda.MagdaSync.SyncKsz.Queries;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Persoonsgegevens;
using Wolverine;
using Xunit;

public class Given_Insz_Only_In_Kbo_Verenigngen
{
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly VerenigingRepositoryMock _verenigingsRepository;

    public Given_Insz_Only_In_Kbo_Verenigngen()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithVertegenwoordigersScenario();
        var vertegenwoordiger = _scenario.VertegenwoordigerWerdToegevoegdVanuitKBO1;
        _verenigingsRepository = new VerenigingRepositoryMock(_scenario.GetVerenigingState());

        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock
           .Setup(x => x.Get(Insz.Create(vertegenwoordiger.Insz), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new[]
            {
                _fixture.Create<VertegenwoordigerPersoonsgegevens>() with
                {
                    VCode = _scenario.VCode,
                    Insz = vertegenwoordiger.Insz,
                    VertegenwoordigerId = vertegenwoordiger.VertegenwoordigerId,
                }
            });

        var filterVzerOnylQueryMock = new Mock<IFilterVzerOnlyQuery>();

        filterVzerOnylQueryMock.Setup(x => x.ExecuteAsync(It.IsAny<FilterVzerOnlyQueryFilter>(), It.IsAny<CancellationToken>()))
                               .ReturnsAsync([]);

        _sut = new SyncKszMessageHandler(persoonsgegevensRepoMock.Object, _verenigingsRepository, filterVzerOnylQueryMock.Object, NullLogger<SyncKszMessageHandler>.Instance);
        _sut.Handle(new SyncKszMessage(Insz.Hydrate(vertegenwoordiger.Insz), true), CancellationToken.None)
            .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
       _verenigingsRepository.ShouldNotHaveAnySaves();
    }
}
