namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Xunit;

public class Given_A_Valid_Erkenning
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_Valid_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_ErkenningWerdGeregistreerd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _fixture.Create<RegistreerErkenningCommand>();
        var ipdcProduct = _fixture.Create<IpdcProduct>();
        var commandMetadata = _fixture.Create<CommandMetadata>();

        await _commandHandler.Handle(
            new CommandEnvelope<RegistreerErkenningCommand>(command, commandMetadata),
            ipdcProduct
        );

        _aggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeregistreerd(
                1,
                ipdcProduct,
                command.Erkenning.Startdatum,
                command.Erkenning.Einddatum,
                command.Erkenning.Hernieuwingsdatum,
                command.Erkenning.HernieuwingsUrl,
                new GegevensInitiator() { OvoCode = commandMetadata.Initiator },
                ErkenningStatus.Bepaal(
                    ErkenningsPeriode.Create(command.Erkenning.Startdatum, command.Erkenning.Einddatum),
                    DateOnly.FromDateTime(DateTime.Now)
                )
            )
        );
    }
}
