namespace AssociationRegistry.Test.MagdaSync;

using AutoFixture;
using CommandHandling.MagdaSync.SyncKsz;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using DecentraalBeheer.Vereniging;
using Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Persoonsgegevens;
using Wolverine;
using Xunit;

public class Given_Overeleden_Vertegenwoordiger
{
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncKszMessageHandler _sut;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly VerenigingRepositoryMock _verenigingsRepository;

    public Given_Overeleden_Vertegenwoordiger()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        _verenigingsRepository = new VerenigingRepositoryMock(_scenario.GetVerenigingState(), true, true);

        var persoonsgegevensRepoMock = new Mock<IVertegenwoordigerPersoonsgegevensRepository>();
        persoonsgegevensRepoMock
           .Setup(x => x.Get(Insz.Create(_scenario.VertegenwoordigerWerdToegevoegd.Insz), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new[]
            {
                _fixture.Create<VertegenwoordigerPersoonsgegevens>() with
                {
                    VCode = _scenario.VCode,
                    Insz = _scenario.VertegenwoordigerWerdToegevoegd.Insz,
                    VertegenwoordigerId = _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                }
            });

        _sut = new SyncKszMessageHandler(persoonsgegevensRepoMock.Object, _verenigingsRepository, NullLogger<SyncKszMessageHandler>.Instance);
        _sut.Handle(new SyncKszMessage(Insz.Hydrate(_scenario.VertegenwoordigerWerdToegevoegd.Insz), true), CancellationToken.None)
            .GetAwaiter().GetResult();
    }

    [Fact]
    public void Then_No_Event_Is_Saved()
    {
       _verenigingsRepository.ShouldHaveSavedExact(new KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
                                                       _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                                       _scenario.VertegenwoordigerWerdToegevoegd.Insz,
                                                       _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                                                       _scenario.VertegenwoordigerWerdToegevoegd.Achternaam
                                                       ));
    }
}
