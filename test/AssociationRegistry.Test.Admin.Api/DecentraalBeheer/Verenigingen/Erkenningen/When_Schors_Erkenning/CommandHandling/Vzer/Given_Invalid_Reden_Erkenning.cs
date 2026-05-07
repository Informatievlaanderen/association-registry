namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_Reden_Erkenning
{
    private readonly SchorsErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Invalid_Reden_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new SchorsErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async ValueTask Then_Nothing(string reden)
    {
        var teSchorsenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;
        var command = _fixture.Create<SchorsErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeSchorsenErkenning>() with
            {
                ErkenningId = teSchorsenErkenningId,
                RedenSchorsing = reden,
            },
        };

       var exception = await Assert.ThrowsAsync<ErkenningRedenSchorsingIsVerplicht>(async () => await _commandHandler.Handle(
            new CommandEnvelope<SchorsErkenningCommand>(command, _fixture.Create<CommandMetadata>())
        ));

       exception.Message.Should().Be(ExceptionMessages.ErkenningRedenSchorsingVerplicht);
    }
}
