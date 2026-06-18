namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.
    CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Unknown_Erkenning
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;

    public Given_Unknown_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        var verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());
        _commandHandler = new WijzigErkenningCommandHandler(verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws_ErkenningIsNietGekend()
    {
        var unknownErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId + _fixture.Create<int>();

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with { ErkenningId = unknownErkenningId },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        var exception = await Assert.ThrowsAsync<ErkenningIsNietGekend>(async () =>
        {
            await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata),
                                         new IOrganisatieBevoegdheidServiceMockStub().Object);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningIsNietGekend, unknownErkenningId));
    }
}
