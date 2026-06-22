namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Kbo;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Erkenning_Already_Exists_With_Same_OvoCode_And_ProductNummer_And_Periode_Overlaps
{
    private readonly RegistreerErkenningContext<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario> _ctx =
        new(new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario());

    [Fact]
    public async ValueTask Then_An_ErkenningAlreadyExists_Exception_Is_Thrown()
    {
        var erkenning = _ctx.RegistreerErkenningCommand.Erkenning with
        {
            ErkenningsPeriode = ErkenningsPeriode.Create(
                _ctx.Scenario.ErkenningWerdGeregistreerd.Startdatum,
                _ctx.Scenario.ErkenningWerdGeregistreerd.Einddatum
            ),
        };

        var command = _ctx.RegistreerErkenningCommand with
        {
            Erkenning = erkenning,
        };

        var ipdcProduct = _ctx.CreateIpdcProduct(nummer: _ctx.Scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer);
        var initiator = _ctx.CreateInitiator(ovoCode: _ctx.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);
        var metadata = _ctx.CreateMetadata(initiator: _ctx.Scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () =>
        {
            await _ctx.Handle(command, ipdcProduct, initiator, metadata);
        });

        exception.Message.Should().Be(ExceptionMessages.ErkenningBestaatAl);
    }
}
