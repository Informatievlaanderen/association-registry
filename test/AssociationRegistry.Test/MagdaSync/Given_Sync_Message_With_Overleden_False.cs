namespace AssociationRegistry.Test.MagdaSync;

using AssociationRegistry.Framework;
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

public class Given_Sync_Message_With_Overleden_False
{
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly VerenigingRepositoryMock _verenigingsRepository;
    private readonly Mock<IVertegenwoordigerPersoonsgegevensRepository> _vertegenwoordigerPersoonsgegevensRepository;
    private readonly Mock<IVerenigingsRepository> _verenigingsrepository;

    public Given_Sync_Message_With_Overleden_False()
    {
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        _vertegenwoordigerPersoonsgegevensRepository = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        _verenigingsrepository = new Mock<IVerenigingsRepository>();
        _sut = new SyncKszMessageHandler(_vertegenwoordigerPersoonsgegevensRepository.Object, _verenigingsrepository.Object, Mock.Of<IFilterVzerOnlyQuery>(), NullLogger<SyncKszMessageHandler>.Instance);
        _sut.Handle(new SyncKszMessage(Insz.Hydrate(_scenario.VertegenwoordigerWerdToegevoegd.Insz), false), CancellationToken.None)
            .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_No_Persoonsgegevens_Are_Retrieved()
    {
        _vertegenwoordigerPersoonsgegevensRepository.Verify(x => x.Get(It.IsAny<Insz>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void Then_No_Vereniging_Is_Loaded()
    {
        _verenigingsrepository.Verify(x => x.Load<Vereniging>(It.IsAny<VCode>(), It.IsAny<CommandMetadata>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
    }
}
