namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling.Vzer;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Erkenning_Already_Exists_With_Same_OvoCode_And_ProductNummer_And_Periode_Overlaps
{
    [Fact]
    public async ValueTask Then_An_ErkenningAlreadyExists_Exception_Is_Thrown()
    {
        var scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario();

        var test = RegistreerErkenningTest<VerenigingMetRechtspersoonlijkheidWerdGeregistreerdWithErkenningScenario>
            .Given(scenario)
            .WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        ErkenningsPeriode = ErkenningsPeriode.Create(
                            scenario.ErkenningWerdGeregistreerd.Startdatum,
                            scenario.ErkenningWerdGeregistreerd.Einddatum
                        ),
                    },
                }
            )
            .WithIpdcProduct(nummer: scenario.ErkenningWerdGeregistreerd.IpdcProduct.Nummer)
            .WithInitiator(ovoCode: scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode)
            .WithMetadata(initiator: scenario.ErkenningWerdGeregistreerd.GeregistreerdDoor.OvoCode);

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () => await test.WhenHandled());

        exception.Message.Should().Be(ExceptionMessages.ErkenningBestaatAl);
    }
}
