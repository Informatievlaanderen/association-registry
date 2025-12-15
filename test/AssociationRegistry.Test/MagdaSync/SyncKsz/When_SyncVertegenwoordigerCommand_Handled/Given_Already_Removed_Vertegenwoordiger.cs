namespace AssociationRegistry.Test.MagdaSync.SyncKsz.When_SyncVertegenwoordigerCommand_Handled;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using CommandHandling.KboSyncLambda.SyncKsz;
using CommandHandling.MagdaSync.SyncKsz;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_Already_Removed_Vertegenwoordiger
{
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncVertegenwoordigerCommandHandler _sut;
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private Mock<IMagdaGeefPersoonService> _mock;

    public Given_Already_Removed_Vertegenwoordiger()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        var state = _scenario.GetVerenigingState();

        state = state.Apply(new VertegenwoordigerWerdVerwijderd(_scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                                        _scenario.VertegenwoordigerWerdToegevoegd.Insz,
                                                        _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                                                        _scenario.VertegenwoordigerWerdToegevoegd.Achternaam));

        _mock = new Mock<IMagdaGeefPersoonService>();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(state,
                                                                 expectedLoadingDubbel: true,
                                                                 expectedLoadingVerwijderd: true);

        _sut = new SyncVertegenwoordigerCommandHandler(
            _verenigingRepositoryMock, _mock.Object,
            NullLogger<SyncVertegenwoordigerCommandHandler>.Instance);
    }

    [Fact]
    public async ValueTask Then_It_Saves_No_Event()
    {
        await _sut.Handle(new CommandEnvelope<SyncVertegenwoordigerCommand>(new SyncVertegenwoordigerCommand(_scenario.VCode,
                                                      _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId),
                                                  CommandMetadata.ForDigitaalVlaanderenProcess));

        _verenigingRepositoryMock.ShouldNotHaveAnySaves();
    }

    [Fact]
    public async ValueTask Then_It_Does_Not_Sync_With_Ksz()
    {
        _mock.Verify(x => x.GeefPersoon(It.IsAny<GeefPersoonRequest>(),
                                       It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()), Times.Never());
    }
}
