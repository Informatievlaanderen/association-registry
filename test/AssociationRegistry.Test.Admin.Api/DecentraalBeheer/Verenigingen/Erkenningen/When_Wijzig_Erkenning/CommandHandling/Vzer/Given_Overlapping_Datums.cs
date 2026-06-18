namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
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
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Overlapping_Datums()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningenScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask Then_Throws_ErkenningBestaatAl()
    {
        var teWijzigenErkenning = _scenario.ErkenningWerdGeregistreerdInToekomst.ErkenningId;

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenning,
                StartDatum = NullOrEmpty<DateOnly>.Create(
                    _scenario.ErkenningWerdGeregistreerdInToekomst.Startdatum.Value
                ),
                EindDatum = NullOrEmpty<DateOnly>.Create(
                    _scenario.ErkenningWerdGeregistreerdInToekomst.Einddatum.Value
                ),
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Create(
                    _scenario.ErkenningWerdGeregistreerdInToekomst.Hernieuwingsdatum.Value
                ),
            },
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerdInToekomst.GeregistreerdDoor.OvoCode,
        };

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () =>
        {
            await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata),
                                         new IOrganisatieBevoegdheidServiceMockStub().Object);

        });

        exception.Message.Should().Be(string.Format(ExceptionMessages.ErkenningBestaatAl, teWijzigenErkenning));
    }
}
