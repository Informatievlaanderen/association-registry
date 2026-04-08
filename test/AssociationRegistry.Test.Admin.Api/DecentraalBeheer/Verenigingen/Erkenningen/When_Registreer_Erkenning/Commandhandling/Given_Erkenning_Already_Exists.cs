namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.
    Commandhandling;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Erkenning_Already_Exists
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly ErkenningWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Erkenning_Already_Exists()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new ErkenningWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_ErkenningAlreadyExists_Exception_Is_Thrown()
    {
        var command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>() with
            {
                IpdcProduct = _scenario.ErkenningWerdGeregistreerd.IpdcProduct,
                Startdatum = new DateOnly(2026, 1, 1),
                Einddatum = new DateOnly(2026, 1, 2),
                HernieuwingsUrl = "https://hernieuwingen.vlaanderen.be",
            },
        };

        var exception = await Assert.ThrowsAsync<ErkenningBestaatAl>(async () =>
                                                                             await _commandHandler.Handle(
                                                                                 new CommandEnvelope<
                                                                                     RegistreerErkenningCommand>(
                                                                                     command,
                                                                                     _fixture
                                                                                        .Create<CommandMetadata>())
                                                                             )
        );

        exception.Message.Should().Be(ExceptionMessages.ErkenningAlreadyExists);
    }
}
