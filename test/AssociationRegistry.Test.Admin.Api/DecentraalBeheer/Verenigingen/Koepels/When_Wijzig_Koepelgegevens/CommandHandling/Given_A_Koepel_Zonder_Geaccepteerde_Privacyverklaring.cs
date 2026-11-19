namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Verenigingen.Koepels.When_Wijzig_Koepelgegevens.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Vereniging.Koepels;
using AssociationRegistry.Events;
using AssociationRegistry.Test.Common.Scenarios.CommandHandling.VerenigingMetRechtspersoonlijkheid;
using Xunit;

public class Given_A_Koepel_Zonder_Geaccepteerde_Privacyverklaring
{
    private readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario _scenario;

    public Given_A_Koepel_Zonder_Geaccepteerde_Privacyverklaring()
    {
        _scenario = new VerenigingMetRechtspersoonlijkheidWerdGeregistreerdScenario();

        _scenario.additionalEvents.Add(new VerenigingWerdGeidentificeerdAlsKoepel(_scenario.VCode.Value));
    }

    [Fact]
    public async ValueTask With_PrivacyVerklaringGeaccepteerd_Then_It_Saves_KoepelPrivacyverklaringWerdGeaccepteerd()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(privacyVerklaringGeaccepteerd: true);

        ctx.ShouldHaveSaved(new KoepelPrivacyverklaringWerdGeaccepteerd(_scenario.VCode.Value));
    }

    [Fact]
    public async ValueTask With_GeenKoepel_Then_It_Only_Saves_VerenigingWerdGeidentificeerdAlsGeenKoepel()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.GeenKoepel);

        ctx.ShouldHaveSaved(new VerenigingWerdGeidentificeerdAlsGeenKoepel(_scenario.VCode.Value));
    }

    [Fact]
    public async ValueTask With_NietBepaald_Then_It_Only_Saves_KoepelstatusWerdTerugGezetNaarNietBepaald()
    {
        var ctx = await WijzigKoepelgegevensContext.Given(_scenario).When(koepelstatus: Koepelstatus.NietBepaald);

        ctx.ShouldHaveSaved(new KoepelstatusWerdTerugGezetNaarNietBepaald(_scenario.VCode.Value));
    }
}
