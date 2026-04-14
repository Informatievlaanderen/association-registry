namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Invalid_HernieuwingsUrl
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public Given_Invalid_HernieuwingsUrl()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario();
        var aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_URLStartNietMetHttpOfHttps_Exception_Is_Thrown()
    {
        var command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>() with
            {
                HernieuwingsUrl = "urlWithoutHttpOrHttps.vlaanderen.be",
            },
        };

        var exception = await Assert.ThrowsAsync<URLStartNietMetHttpOfHttps>(async () =>
            await _commandHandler.Handle(
                new CommandEnvelope<RegistreerErkenningCommand>(command, _fixture.Create<CommandMetadata>()),
                _fixture.Create<IpdcProduct>()
            )
        );

        exception.Message.Should().Be(ExceptionMessages.UrlDoesNotStartWithHttpOrHttps);
    }
}
