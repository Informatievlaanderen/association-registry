namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Wijzig_Erkenning.
    CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Primitives;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.StubsMocksFakes.Wegwijs;
using FluentAssertions;
using Xunit;

public class Given_No_RedenVanWijziging
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_No_RedenVanWijziging()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
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

           await _commandHandler.Handle(new CommandEnvelope<WijzigErkenningCommand>(command, commandMetadata), new IOrganisatieBevoegdheidServiceMockStub().Object);
        });

        exception.Message.Should().Be(ExceptionMessages.RedenVanWijzigingIsVerplicht);
    }
}
