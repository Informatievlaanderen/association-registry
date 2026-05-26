namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Corrigeer_Erkenning.
    CommandHandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.WijzigErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Websites.Exceptions;
using AssociationRegistry.Framework;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Common.StubsMocksFakes.VerenigingsRepositories;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Hernieuwingsurl
{
    private readonly WijzigErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;
    private readonly AggregateSessionMock _verenigingRepositoryMock;

    public Given_Hernieuwingsurl()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        _verenigingRepositoryMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new WijzigErkenningCommandHandler(_verenigingRepositoryMock);
    }

    [Fact]
    public async ValueTask With_Invalid_Scheme_Then_Throws_WebsiteMoetStartenMetHttps()
    {
        var teCorrigerenErkenningId = _scenario.ErkenningWerdGeregistreerd.ErkenningId;

        var hernieuwingsUrl = "ftp://example.com";

        var command = _fixture.Create<WijzigErkenningCommand>() with
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
                    WijzigErkenningCommand>(
                    command,
                    commandMetadata)
            );
        });

        exception.Message.Should().Be(ExceptionMessages.InvalidWebsiteStart);
    }
}
