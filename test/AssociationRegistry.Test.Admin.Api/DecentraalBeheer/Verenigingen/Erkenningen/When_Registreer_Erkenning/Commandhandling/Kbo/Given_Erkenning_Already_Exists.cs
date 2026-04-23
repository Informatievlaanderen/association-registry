namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Kbo;

using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Erkenningen.RegistreerErkenning;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AssociationRegistry.Framework;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using AssociationRegistry.Test.Common.StubsMocksFakes.VerenigingsRepositories;
using AutoFixture;
using FluentAssertions;
using Xunit;

public class Given_Erkenning_Already_Exists_With_Same_OvoCode_And_ProductNummer_And_Periode_Overlaps
{
    private readonly RegistreerErkenningCommandHandler _commandHandler;
    private readonly Fixture _fixture;
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario _scenario;

    public Given_Erkenning_Already_Exists_With_Same_OvoCode_And_ProductNummer_And_Periode_Overlaps()
    {
        _fixture = new Fixture().CustomizeAdminApi();

        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();
        var aggregateSessionMock = new AggregateSessionMock(_scenario.GetVerenigingState());

        _commandHandler = new RegistreerErkenningCommandHandler(aggregateSessionMock);
    }

    [Fact]
    public async ValueTask Then_An_ErkenningAlreadyExists_Exception_Is_Thrown()
    {
        var command = _fixture.Create<RegistreerErkenningCommand>() with
        {
            VCode = _scenario.VCode,
            Erkenning = _fixture.Create<TeRegistrerenErkenning>() with
            {
                ErkenningsPeriode = ErkenningsPeriode.Create(
                    _scenario.ErkenningWerdGeregistreerd.Startdatum,
                    _scenario.ErkenningWerdGeregistreerd.Einddatum
                ),
            },
        };

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
