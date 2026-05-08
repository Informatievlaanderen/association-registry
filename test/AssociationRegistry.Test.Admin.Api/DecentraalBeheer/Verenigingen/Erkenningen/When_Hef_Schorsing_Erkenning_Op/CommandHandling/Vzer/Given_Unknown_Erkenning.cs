namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Hef_Schorsing_Erkenning_Op.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.HefSchorsingErkenningOp;
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

public class Given_Unknown_Erkenning
{
    private readonly HefSchorsingErkenningOpCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Unknown_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithGeschorsteErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());
        _commandHandler = new HefSchorsingErkenningOpCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws_ErkenningIsNietGekend()
    {
        var unknownErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId + _fixture.Create<int>();

        var command = _fixture.Create<HefSchorsingErkenningOpCommand>() with
        {
            VCode = _scenario.VCode,
            ErkenningId = unknownErkenningId,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        var commandEnvelope = new CommandEnvelope<HefSchorsingErkenningOpCommand>(command, commandMetadata);

        var exception = await Assert.ThrowsAsync<ErkenningIsNietGekend>(async () =>
        {
            await _commandHandler.Handle(commandEnvelope);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningIsNietGekend, unknownErkenningId));
    }
}
