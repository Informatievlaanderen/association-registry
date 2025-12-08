namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.Inschrijvingen;

using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using CommandHandling.InschrijvingenVertegenwoordigers;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Events;
using Moq;
using Xunit;

public class Given_Niet_Gekende_Vertegenwoordigers
{
    private VerenigingRepositoryMock _verenigingRepositoryMock;
    private SchrijfVertegenwoordigersInMessage _message;
    private VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private SchrijfVertegenwoordigersInMessageHandler _commandHandler;
    private Fixture _fixture;

    public Given_Niet_Gekende_Vertegenwoordigers()
    {
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();

        _verenigingRepositoryMock = new VerenigingRepositoryMock(_scenario.GetVerenigingState(), true, true);

        _fixture = new Fixture().CustomizeAdminApi();

        var magdaGeefPersoonService = new Mock<IMagdaGeefPersoonService>();

        foreach (var vertegenwoordiger in _scenario.VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd.Vertegenwoordigers)
        {
            magdaGeefPersoonService.Setup(x => x.GeefPersoon(GeefPersoonRequest.From(vertegenwoordiger), It.IsAny<CommandMetadata>(), It.IsAny<CancellationToken>()))
                                   .ThrowsAsync(new EenOfMeerdereInszWaardenKunnenNietGevalideerdWordenBijKsz());
        }

        _commandHandler = new SchrijfVertegenwoordigersInMessageHandler(_verenigingRepositoryMock, magdaGeefPersoonService.Object);

        _message = new SchrijfVertegenwoordigersInMessage(_scenario.VCode);
    }

    [Fact]
    public async ValueTask Then_KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend_Events_Are_Saved()
    {
        await _commandHandler
           .Handle(new CommandEnvelope<SchrijfVertegenwoordigersInMessage>(_message, _fixture.Create<CommandMetadata>()),
                   CancellationToken.None);

        var events =
            _scenario
               .VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd
               .Vertegenwoordigers
               .Select(v => new KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend(
                           v.VertegenwoordigerId,
                           v.Insz,
                           v.Voornaam,
                           v.Achternaam))
               .Cast<IEvent>()
               .ToArray();

        _verenigingRepositoryMock.ShouldHaveSavedExact(events);
    }
}
