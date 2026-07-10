namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Erkenningen.When_Registreer_Erkenning.Commandhandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen.Exceptions;
using AutoFixture;
using Common.AutoFixture;
using Common.Scenarios.CommandHandling;
using Events;
using FluentAssertions;
using Resources;
using Xunit;

public class Given_Erkenning_Already_Exists_With_Same_OvoCode_And_ProductNummer_And_Periode_Overlaps
{
    public static IEnumerable<object[]> ErkenningScenarios
    {
        get
        {
            var fixture = new Fixture().CustomizeDomain();

            var scenario = new ErkenningScenarioBuilder(fixture).WithActieveErkenning().Build();

            return new[] { new object[] { scenario.Vzer }, new object[] { scenario.Vmr } };
        }
    }

    [Theory]
    [MemberData(nameof(ErkenningScenarios))]
    public async ValueTask Then_An_ErkenningAlreadyExists_Exception_Is_Thrown(CommandhandlerScenarioBase scenario)
    {
        var existingErkenning = scenario.Events().OfType<ErkenningWerdGeregistreerd>().Single();

        var ctx = RegistreerErkenningContext<CommandhandlerScenarioBase>
            .Given(scenario)
            .WithCommand(cmd =>
                cmd with
                {
                    Erkenning = cmd.Erkenning with
                    {
                        ErkenningsPeriode = ErkenningsPeriode.Create(
                            existingErkenning.Startdatum,
                            existingErkenning.Einddatum
                        ),
                    },
                }
            )
            .WithIpdcProduct(nummer: existingErkenning.IpdcProduct.Nummer)
            .WithInitiator(ovoCode: existingErkenning.GeregistreerdDoor.OvoCode)
            .WithMetadata(initiator: existingErkenning.GeregistreerdDoor.OvoCode);

        var exception = await Assert.ThrowsAsync<ErkenningCombinatieBestaatAl>(async () => await ctx.WhenHandled());

        ctx.AggregateSessionMock.ShouldNotHaveAnySaves();
        exception.Message.Should().Be(ExceptionMessages.ErkenningBestaatAl);
    }
}
