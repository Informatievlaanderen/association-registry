namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.
    CommandHandling.Vzer;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.CorrigeerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Websites.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Primitives;
using Resources;
using Xunit;

public class Given_Hernieuwingsurl
{
    private readonly CorrigeerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Hernieuwingsurl()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new CorrigeerErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_Invalid_Scheme_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var teCorrigerenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var hernieuwingsUrl = "ftp://example.com";

        var command = _fixture.Create<CorrigeerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeCorrigerenErkenning>() with
            {
                ErkenningId = teCorrigerenErkenningId,
                HernieuwingsUrl = hernieuwingsUrl,
            },
        };

        var exception = await Assert.ThrowsAsync<WebsiteMoetStartenMetHttps>(async () =>
        {
            var commandMetadata = _fixture.Create<CommandMetadata>() with
            {
                Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode
            };

            await _commandHandler.Handle(
                new CommandEnvelope<
                    CorrigeerErkenningCommand>(
                    command,
                    commandMetadata)
            );
        });

        exception.Message.Should().Be(ExceptionMessages.InvalidWebsiteStart);
    }
}
