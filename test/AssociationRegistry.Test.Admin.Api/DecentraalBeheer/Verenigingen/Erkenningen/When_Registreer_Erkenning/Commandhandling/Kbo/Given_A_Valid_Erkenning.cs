namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Xunit;

public class Given_A_Valid_Erkenning
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_Valid_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_ErkenningWerdGeregistreerd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _fixture.Create<RegistreerErkenningCommand>();
        var ipdcProduct = _fixture.Create<IpdcProduct>();
        var initiator = _fixture.Create<GegevensInitiator>();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await _commandHandler.Handle(
            new CommandEnvelope<RegistreerErkenningCommand>(command, commandMetadata),
            ipdcProduct,
            initiator
        );

        _aggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeregistreerd(
                1,
                ipdcProduct,
                command.Erkenning.ErkenningsPeriode.Startdatum,
                command.Erkenning.ErkenningsPeriode.Einddatum,
                command.Erkenning.Hernieuwingsdatum.Value,
                command.Erkenning.HernieuwingsUrl.Value,
                initiator,
                ErkenningStatus.Bepaal(
                    ErkenningsPeriode.Create(
                        command.Erkenning.ErkenningsPeriode.Startdatum,
                        command.Erkenning.ErkenningsPeriode.Einddatum
                    ),
                    DateOnly.FromDateTime(DateTime.Now)
                )
            )
        );
    }
}
