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

public class Given_Erkenning_Already_Exists
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly ErkenningWerdGeregistreerdScenario _scenario;

    public Given_Erkenning_Already_Exists()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new ErkenningWerdGeregistreerdScenario();
        var aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_ErkenningAlreadyExists_Exception_Is_Thrown()
    {
        var command = _fixture.Create<RegistreerErkenningCommand>() with { VCode = _scenario.VCode };

        var ipdcProductNummer = _fixture.Create<IpdcProduct>() with
        {
            Nummer = _scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer,
        };

        var commandMetadata = _fixture.Create<CommandMetadata>() with
        {
            Initiator = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        var initiator = _fixture.Create<GegevensInitiator>() with
        {
            OvoCode = _scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode,
        };

        var exception = await Assert.ThrowsAsync<ErkenningBestaatAl>(async () =>
        {
            await _commandHandler.Handle(
                new CommandEnvelope<RegistreerErkenningCommand>(command, commandMetadata),
                ipdcProductNummer,
                initiator
            );
        });

        exception.Message.Should().Be(ExceptionMessages.ErkenningBestaatAl);
    }
}
