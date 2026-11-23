namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Dien_Relatieaanvraag_In.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.Events;
using Xunit;
using static Verenigingen;

/// <summary>The partijen (aanvrager, the same vereniging) are checked by their value object: see When_Creating_Partijen.</summary>
public class Given_A_Relatieaanvraag_That_Breaks_A_Rule
{
    [Fact]
    public async ValueTask With_A_Koepel_Zonder_Rechtspersoonlijkheid_Then_It_Throws_BeoogdeKoepelMoetKboVerenigingZijn()
    {
        var ctx = new DienRelatieaanvraagInContext(Vzer(KoepelA), Vzer(VerenigingC));

        await Assert.ThrowsAsync<BeoogdeKoepelMoetKboVerenigingZijn>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap, aangevraagdDoor: VerenigingC))
        );

        ctx.ShouldNotHaveSaved();
    }

    // Dezelfde boodschap, ongeacht wie indient.
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async ValueTask With_A_Koepel_That_Is_NietBepaald_Then_It_Throws_BeoogdeKoepelIsNietAlsKoepelGeidentificeerd(
        bool deKoepelDientIn
    )
    {
        var ctx = new DienRelatieaanvraagInContext(KboVereniging(KoepelA), Vzer(VerenigingC));

        await Assert.ThrowsAsync<BeoogdeKoepelIsNietAlsKoepelGeidentificeerd>(async () =>
            await ctx.Handle(
                ctx.Command(RelatieType.Lidmaatschap, aangevraagdDoor: deKoepelDientIn ? KoepelA : VerenigingC)
            )
        );

        ctx.ShouldNotHaveSaved();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async ValueTask With_A_Koepel_That_Is_GeenKoepel_Then_It_Throws_BeoogdeKoepelIsNietAlsKoepelGeidentificeerd(
        bool deKoepelDientIn
    )
    {
        var ctx = new DienRelatieaanvraagInContext(GeenKoepel(KoepelA), Vzer(VerenigingC));

        await Assert.ThrowsAsync<BeoogdeKoepelIsNietAlsKoepelGeidentificeerd>(async () =>
            await ctx.Handle(
                ctx.Command(RelatieType.Lidmaatschap, aangevraagdDoor: deKoepelDientIn ? KoepelA : VerenigingC)
            )
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Subvereniging_Met_Rechtspersoonlijkheid_Then_It_Throws_SubverenigingsaanvraagIsNietToegestaan()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), KboVereniging(VerenigingC));

        await Assert.ThrowsAsync<SubverenigingsaanvraagIsNietToegestaan>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Subvereniging_With_Subtype_FeitelijkeVereniging_Then_It_Throws_SubverenigingsaanvraagIsNietToegestaan()
    {
        var feitelijkeVereniging = Vzer(VerenigingC)
            .Apply(new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(VerenigingC.Value));

        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), feitelijkeVereniging);

        await Assert.ThrowsAsync<SubverenigingsaanvraagIsNietToegestaan>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Subvereniging_Of_Another_Koepel_Then_A_Subvereniging_Throws_SubverenigingsaanvraagIsNietToegestaan()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC).AlsSubverenigingVan(KoepelB));

        await Assert.ThrowsAsync<SubverenigingsaanvraagIsNietToegestaan>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Lidmaatschap_Of_Another_Koepel_Then_A_Subvereniging_Throws_VerenigingMetLidmaatschapKanGeenSubverenigingWorden()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC).MetLidmaatschap(KoepelB));

        await Assert.ThrowsAsync<VerenigingMetLidmaatschapKanGeenSubverenigingWorden>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Future_Lidmaatschap_Of_Another_Koepel_Then_A_Subvereniging_Throws_VerenigingMetLidmaatschapKanGeenSubverenigingWorden()
    {
        var toekomstigLidmaatschap = Vzer(VerenigingC)
            .MetLidmaatschap(KoepelB, van: DienRelatieaanvraagInContext.Vandaag.AddMonths(2));

        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), toekomstigLidmaatschap);

        await Assert.ThrowsAsync<VerenigingMetLidmaatschapKanGeenSubverenigingWorden>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Subvereniging))
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_A_Subvereniging_Of_Another_Koepel_Then_A_Lidmaatschap_Throws_SubverenigingKanGeenLidWorden()
    {
        var ctx = new DienRelatieaanvraagInContext(Koepel(KoepelA), Vzer(VerenigingC).AlsSubverenigingVan(KoepelB));

        await Assert.ThrowsAsync<SubverenigingKanGeenLidWorden>(async () =>
            await ctx.Handle(ctx.Command(RelatieType.Lidmaatschap))
        );

        ctx.ShouldNotHaveSaved();
    }
}
