namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Xunit;

public class Given_A_Koepel_Met_Geaccepteerde_Privacyverklaring
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public Given_A_Koepel_Met_Geaccepteerde_Privacyverklaring()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _scenario.additionalEvents.AddRange([
            new VerenigingWerdGeidentificeerdAlsKoepel(_scenario.VCode.Value),
            new KoepelPrivacyverklaringWerdGeaccepteerd(_scenario.VCode.Value),
        ]);
    }

    [Fact]
    public async ValueTask With_GeenKoepel_Then_It_Saves_VerenigingWerdGeidentificeerdAlsGeenKoepel_And_KoepelPrivacyverklaringWerdIngetrokken()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.GeenKoepel);

        ctx.ShouldHaveSaved(
            new VerenigingWerdGeidentificeerdAlsGeenKoepel(_scenario.VCode.Value),
            new KoepelPrivacyverklaringWerdIngetrokken(_scenario.VCode.Value)
        );
    }

    [Fact]
    public async ValueTask With_NietBepaald_Then_It_Saves_KoepelstatusWerdTerugGezetNaarNietBepaald_And_KoepelPrivacyverklaringWerdIngetrokken()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.NietBepaald);

        ctx.ShouldHaveSaved(
            new KoepelstatusWerdTerugGezetNaarNietBepaald(_scenario.VCode.Value),
            new KoepelPrivacyverklaringWerdIngetrokken(_scenario.VCode.Value)
        );
    }

    [Fact]
    public async ValueTask With_PrivacyVerklaringGeaccepteerd_False_Then_It_Saves_KoepelPrivacyverklaringWerdIngetrokken()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(privacyVerklaringGeaccepteerd: false);

        ctx.ShouldHaveSaved(new KoepelPrivacyverklaringWerdIngetrokken(_scenario.VCode.Value));
    }

    [Fact]
    public async ValueTask With_Koepel_Then_It_Saves_Nothing_And_Keeps_The_Privacyverklaring()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.Koepel);

        ctx.ShouldNotHaveSavedAnyEvents();
    }

    [Fact]
    public async ValueTask With_The_Same_Koepelgegevens_Then_It_Saves_Nothing()
    {
        var ctx = await WijzigKoepelgegevensContext
            .Given(_scenario)
            .When(koepelstatus: Koepelstatus.Koepel, privacyVerklaringGeaccepteerd: true);

        ctx.ShouldNotHaveSavedAnyEvents();
    }
}
