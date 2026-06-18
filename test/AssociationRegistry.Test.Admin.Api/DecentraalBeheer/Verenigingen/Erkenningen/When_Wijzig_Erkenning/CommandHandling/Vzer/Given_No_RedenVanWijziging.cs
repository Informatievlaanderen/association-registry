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
using Events;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_No_RedenVanWijziging
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_No_RedenVanWijziging()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }


    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async ValueTask Then_It_Throws_RedenVanWijzigingIsVerplicht(
        string redenVanWijziging)
    {
        var teWijzigenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var command = _fixture.Create<WijzigErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeWijzigenErkenning>() with
            {
                ErkenningId = teWijzigenErkenningId,
                StartDatum = NullOrEmpty<DateOnly>.Null,
                EindDatum = NullOrEmpty<DateOnly>.Null,
                Hernieuwingsdatum = NullOrEmpty<DateOnly>.Null,
                HernieuwingsUrl = string.Empty,
                RedenVanWijziging = redenVanWijziging
            },
        };

        var exception = await Assert.ThrowsAsync<RedenVanWijzigingIsVerplicht>(async () =>
        {
            var commandMetadata = _fixture.Create<CommandMetadata>() with
            {
                Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
            };

            await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata),
                                         new IOrganisatieBevoegdheidServiceMockStub().Object);

        });

        exception.Message.Should().Be(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }
}
