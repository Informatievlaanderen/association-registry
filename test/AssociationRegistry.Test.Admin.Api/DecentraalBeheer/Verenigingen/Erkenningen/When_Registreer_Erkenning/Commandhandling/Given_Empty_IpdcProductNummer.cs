namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

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

public class Given_Empty_IpdcProductNummer
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;
    private readonly AggregateSessionMock _aggregateSessionMock;

    public Given_Empty_IpdcProductNummer()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        _aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(_aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_IpdcProductNummerOntbreekt_Exception_Is_Thrown()
    {

        var command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>() with
            {
                Startdatum = new DateOnly(2026, 1, 1),
                Einddatum = new DateOnly(2026, 1, 2),
                IpdcProduct = null,
                HernieuwingsUrl = "https://www.website.com/renew",
            },
        };

        var exception = await Assert.ThrowsAsync<IpdcProductNummerOntbreekt>(async () =>
                                                                                await _commandHandler.Handle(
                                                                                    new CommandEnvelope<RegistreerErkenningCommand>(command, _fixture.Create<CommandMetadata>())
                                                                                )
        );

        exception.Message.Should().Be(ExceptionMessages.IpdcProductNummerRequired);
    }
}
