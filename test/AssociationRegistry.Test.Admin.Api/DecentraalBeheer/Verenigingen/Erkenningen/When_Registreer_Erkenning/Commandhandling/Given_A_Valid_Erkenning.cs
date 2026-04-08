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
using Xunit;

public class Given_A_Valid_Erkenning
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly FeitelijkeVerenigingWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_A_Valid_Erkenning()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new FeitelijkeVerenigingWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_A_ErkenningWerdGeregistreerd_Event_Is_Saved_With_The_Next_Id()
    {
        var command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>() with
            {
                Startdatum = new DateOnly(2026,1,1),
                Einddatum = new DateOnly(2026,12,31),
                HernieuwingsUrl = "https://www.website.com/renew",
            }
        };

        await _commandHandler.Handle(
            new CommandEnvelope<RegistreerErkenningCommand>(command, _fixture.Create<CommandMetadata>())
        );

        _aggregateSessionMock.ShouldHaveSavedExact(
            new ErkenningWerdGeregistreerd(
                1,
                command.VCode,
                command.Erkenning.IpdcProduct,
                command.Erkenning.Startdatum,
                command.Erkenning.Einddatum,
                command.Erkenning.Hernieuwingsdatum,
                command.Erkenning.HernieuwingsUrl,
                command.Erkenning.GeregistreerdDoor
            )
        );
    }
}
