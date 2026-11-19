namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels.Exceptions;
using AssociationRegistry.Events;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.FeitelijkeVereniging;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingZonderEigenRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_A_Vereniging_Zonder_Rechtspersoonlijkheid
{
    public static TheoryData<string> Verenigingstypes =>
        new() { Verenigingstype.VZER.Code, Verenigingstype.FeitelijkeVereniging.Code };

    [Theory]
    [MemberData(nameof(Verenigingstypes))]
    public async ValueTask With_Koepel_Then_It_Throws_EnkelVerenigingMetRechtspersoonlijkheidKanKoepelZijn(
        string verenigingstype
    )
    {
        var ctx = WijzigKoepelgegevensContext.Given(CreateScenario(verenigingstype));

        var exception = await Assert.ThrowsAsync<EnkelVerenigingMetRechtspersoonlijkheidKanKoepelZijn>(async () =>
            await ctx.When(koepelstatus: Koepelstatus.Koepel)
        );

        exception.Message.Should().Be(ExceptionMessages.EnkelVerenigingMetRechtspersoonlijkheidKanKoepelZijn);
        ctx.ShouldNotHaveSavedAnyEvents();
    }

    [Theory]
    [MemberData(nameof(Verenigingstypes))]
    public async ValueTask With_GeenKoepel_Then_It_Saves_VerenigingWerdGeidentificeerdAlsGeenKoepel(
        string verenigingstype
    )
    {
        var scenario = CreateScenario(verenigingstype);

        var ctx = await WijzigKoepelgegevensContext.Given(scenario).When(koepelstatus: Koepelstatus.GeenKoepel);

        ctx.ShouldHaveSaved(new VerenigingWerdGeidentificeerdAlsGeenKoepel(scenario.VCode.Value));
    }

    private static CommandhandlerScenarioBase CreateScenario(string verenigingstype) =>
        verenigingstype == Verenigingstype.VZER.Code
            ? new VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerdScenario()
            : new FeitelijkeVerenigingWerdGeregistreerdScenario();
}
