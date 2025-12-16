namespace AssociationRegistry.Test.MagdaSync.SyncKsz.When_SyncVertegenwoordigerCommand_Handled;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using CommandHandling.KboSyncLambda.SyncKsz;
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
using Xunit;

public class Given_Overleden_Vertegenwoordiger
{
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario _scenario;
    private readonly SyncVertegenwoordigerCommandHandler _sut;
    private VerenigingRepositoryMock _verenigingRepositoryMock;

    public Given_Overleden_Vertegenwoordiger()
    {
        _fixture = new Fixture().CustomizeDomain();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithAPrimairVertegenwoordigerScenario();

        var state = _scenario.GetVerenigingState();

        var mock = new Mock<IMagdaGeefPersoonService>();

        mock.Setup(x => x.GeefPersoon(new GeefPersoonRequest(_scenario.VertegenwoordigerWerdToegevoegd.Insz,
                                                             _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                                                             _scenario.VertegenwoordigerWerdToegevoegd.Achternaam),
                                      It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PersoonUitKsz(_scenario.VertegenwoordigerWerdToegevoegd.Insz,
                                                 _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                                                 _scenario.VertegenwoordigerWerdToegevoegd.Achternaam,
                                                 Overleden: true));

        _verenigingRepositoryMock = new VerenigingRepositoryMock(state,
                                                                 expectedLoadingDubbel: true,
                                                                 expectedLoadingVerwijderd: true);

        _sut = new SyncVertegenwoordigerCommandHandler(
            _verenigingRepositoryMock,
            mock.Object,
            NullLogger<SyncVertegenwoordigerCommandHandler>.Instance);
    }

    [Fact]
    public async ValueTask Then_It_Saves_No_Event()
    {
        await _sut.Handle(
            new CommandEnvelope<SyncVertegenwoordigerCommand>(new SyncVertegenwoordigerCommand(_scenario.VCode,
                                                      _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId),
                                                  CommandMetadata.ForDigitaalVlaanderenProcess));

        _verenigingRepositoryMock.ShouldHaveSavedExact(new KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden(
                                                           _scenario.VertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                                           _scenario.VertegenwoordigerWerdToegevoegd.Insz,
                                                           _scenario.VertegenwoordigerWerdToegevoegd.Voornaam,
                                                           _scenario.VertegenwoordigerWerdToegevoegd.Achternaam));
    }
}
