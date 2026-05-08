namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Schors_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.SchorsErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_Another_OvoCode
{
    private readonly SchorsErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Another_OvoCode()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());
        _commandHandler = new SchorsErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws_GiIsNIetBevoegd()
    {
        var erkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<SchorsErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeSchorsenErkenning>() with { ErkenningId = erkenningId },
        };

        var commandEnvelope = new CommandEnvelope<SchorsErkenningCommand>(command, _fixture.Create<CommandMetadata>());

        var exception = await Assert.ThrowsAsync<GiIsNIetBevoegd>(async () =>
        {
            await _commandHandler.Handle(commandEnvelope);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.GiIsNIetBevoegd, erkenningId));
    }
}
