namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;
using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels.Exceptions;
using AssociationRegistry.Events;
using AssociationRegistry.Resources;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using FluentAssertions;
using Xunit;

public class Given_A_Vereniging_Met_Rechtspersoonlijkheid_Zonder_Koepelgegevens
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario = new();

    [Fact]
    public async ValueTask With_Koepel_Then_It_Saves_VerenigingWerdGeidentificeerdAlsKoepel()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.Koepel);

        ctx.ShouldHaveSaved(new VerenigingWerdGeidentificeerdAlsKoepel(_scenario.VCode.Value));
    }

    [Fact]
    public async ValueTask With_Koepel_And_PrivacyVerklaringGeaccepteerd_Then_It_Saves_Both_Events()
    {
        var ctx = await WijzigKoepelgegevensContext
            .Given(_scenario)
            .When(koepelstatus: Koepelstatus.Koepel, privacyVerklaringGeaccepteerd: true);

        ctx.ShouldHaveSaved(
            new VerenigingWerdGeidentificeerdAlsKoepel(_scenario.VCode.Value),
            new KoepelPrivacyverklaringWerdGeaccepteerd(_scenario.VCode.Value)
        );
    }

    [Fact]
    public async ValueTask With_GeenKoepel_Then_It_Saves_VerenigingWerdGeidentificeerdAlsGeenKoepel()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.GeenKoepel);

        ctx.ShouldHaveSaved(new VerenigingWerdGeidentificeerdAlsGeenKoepel(_scenario.VCode.Value));
    }

    [Fact]
    public async ValueTask With_NietBepaald_Then_It_Saves_Nothing()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.NietBepaald);

        ctx.ShouldNotHaveSavedAnyEvents();
    }

    [Fact]
    public async ValueTask With_PrivacyVerklaringGeaccepteerd_False_Then_It_Saves_Nothing()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(privacyVerklaringGeaccepteerd: false);

        ctx.ShouldNotHaveSavedAnyEvents();
    }

    [Fact]
    public async ValueTask With_Only_PrivacyVerklaringGeaccepteerd_Then_It_Throws_PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden()
    {
        var ctx = WijzigKoepelgegevensContext.Given(_scenario);

        var exception = await Assert.ThrowsAsync<PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden>(async () =>
            await ctx.When(privacyVerklaringGeaccepteerd: true)
        );

        exception.Message.Should().Be(ExceptionMessages.PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden);
        ctx.ShouldNotHaveSavedAnyEvents();
    }

    [Theory]
    [MemberData(nameof(GeenKoepelStatussen))]
    public async ValueTask With_GeenKoepel_Or_NietBepaald_And_PrivacyVerklaringGeaccepteerd_Then_It_Throws_PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden(
        string koepelstatus
    )
    {
        var ctx = WijzigKoepelgegevensContext.Given(_scenario);

        var exception = await Assert.ThrowsAsync<PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden>(async () =>
            await ctx.When(koepelstatus: Koepelstatus.Parse(koepelstatus), privacyVerklaringGeaccepteerd: true)
        );

        exception.Message.Should().Be(ExceptionMessages.PrivacyverklaringKanEnkelDoorKoepelGeaccepteerdWorden);
        ctx.ShouldNotHaveSavedAnyEvents();
    }

    public static TheoryData<string> GeenKoepelStatussen =>
        new() { Koepelstatus.GeenKoepel.Value, Koepelstatus.NietBepaald.Value };
}
