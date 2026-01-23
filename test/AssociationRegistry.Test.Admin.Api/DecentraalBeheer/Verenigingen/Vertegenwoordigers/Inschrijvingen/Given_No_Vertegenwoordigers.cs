namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Vertegenwoordigers.Inschrijvingen;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using AutoFixture;
using CommandHandling.InschrijvingenVertegenwoordigers;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

public class Given_No_Vertegenwoordigers
{
    private AggregateSessionMock _aggregateSessionMock;
    private SchrijfVertegenwoordigersInMessage _message;
    private VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithMinimalFields _scenario;
    private SchrijfVertegenwoordigersInMessageHandler _commandHandler;
    private Fixture _fixture;

    public Given_No_Vertegenwoordigers()
    {
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithMinimalFields();

        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState(), true, true);

        _fixture = new Fixture().CustomizeAdminApi();

        var magdaGeefPersoonService = new Mock<IMagdaGeefPersoonService>();

        _commandHandler = new SchrijfVertegenwoordigersInMessageHandler(
            _aggregateSessionMock,
            magdaGeefPersoonService.Object,
            NullLogger<SchrijfVertegenwoordigersInMessageHandler>.Instance
        );

        _message = new SchrijfVertegenwoordigersInMessage(_scenario.VCode);
    }

    [Fact]
    public async ValueTask Then_A_VertegenwoordigerWerdToegevoegd_Event_Is_Saved()
    {
        await _commandHandler.Handle(
            new CommandEnvelope<SchrijfVertegenwoordigersInMessage>(_message, _fixture.Create<CommandMetadata>()),
            CancellationToken.None
        );

        _aggregateSessionMock.ShouldNotHaveAnySaves();
    }
}
