namespace AssociationRegistry.Test.MagdaSync;

using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using KboMutations.SyncLambda.MagdaSync.SyncKsz;
using KboMutations.SyncLambda.MagdaSync.SyncKsz.Queries;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Persoonsgegevens;
using Wolverine;
using Xunit;

public class Given_Insz_Not_Found
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly VerenigingRepositoryMock _verenigingsRepository;
    private readonly Mock<IVerenigingsRepository> _verenigingsrepository;
    private readonly Fixture _fixture;

    public Given_Insz_Not_Found()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        _verenigingsrepository = new Mock<IVerenigingsRepository>();

        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock
           .Setup(x => x.Get(It.IsAny<Insz>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync([]);

        _sut = new SyncKszMessageHandler(persoonsgegevensRepoMock.Object, _verenigingsrepository.Object, Mock.Of<IFilterVzerOnlyQuery>(), NullLogger<SyncKszMessageHandler>.Instance);
        _sut.Handle(new SyncKszMessage(Insz.Hydrate(_scenario.VertegenwoordigerWerdToegevoegd.Insz), false, Guid.NewGuid()), CancellationToken.None)
            .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_No_Vereniging_Is_Loaded()
    {
        _verenigingsrepository.Verify(x => x.Load<Vereniging>(It.IsAny<VCode>(), It.IsAny<CommandMetadata>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }
}
