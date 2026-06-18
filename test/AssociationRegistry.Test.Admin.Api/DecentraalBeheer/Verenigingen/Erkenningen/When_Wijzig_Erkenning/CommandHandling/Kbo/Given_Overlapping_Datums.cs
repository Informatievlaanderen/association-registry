namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Kbo;

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
using Primitives;
using Resources;
using Xunit;

public class Given_Overlapping_Datums
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Overlapping_Datums()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws_ErkenningBestaatAl()
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerdInVerleden.ErkenningId;

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Create(
                    _scenario.ErkenningWerdGeregistreerdInHuidig.Startdatum.Value
                ),
                EindDatum = NullOrEmpty<DateOnly>.Create(_scenario.ErkenningWerdGeregistreerdInHuidig.Einddatum.Value),
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(
                    _scenario.ErkenningWerdGeregistreerdInHuidig.Hernieuwingsdatum.Value
                ),
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdInVerleden.GeregistreerdDoor.OvoCode,
        };

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () =>
        {
            await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata), new IOrganisatieBevoegdheidServiceMockStub().Object);
        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningBestaatAl, teWijzigenErkenningId));
    }
}
