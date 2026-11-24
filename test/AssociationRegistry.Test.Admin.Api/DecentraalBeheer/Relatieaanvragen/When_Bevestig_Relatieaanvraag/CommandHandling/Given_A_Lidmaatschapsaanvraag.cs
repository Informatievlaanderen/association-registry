namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Bevestig_Relatieaanvraag.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;
using static Verenigingen;

/// <summary>
///     Confirming adds a lidmaatschap from today, in the same transaction as the confirmation, if the rules of a
///     lidmaatschapsaanvraag still hold.
/// </summary>
public class Given_A_Lidmaatschapsaanvraag
{
    private static readonly DateOnly Today = BevestigRelatieaanvraagContext.Today;

    private static BevestigRelatieaanvraagContext Context(
        VerenigingState vereniging,
        VCode? aangevraagdDoor = null,
        OpenRelatieaanvraag[]? otherOpenRelatieaanvragen = null
    ) =>
        new(
            Koepel(KoepelA),
            vereniging,
            RelatieType.Lidmaatschap,
            aangevraagdDoor ?? KoepelA,
            otherOpenRelatieaanvragen
        );

    [Fact]
    public async ValueTask When_The_Vereniging_Confirms_Then_It_Becomes_Lid_From_Today()
    {
        var ctx = Context(Vzer(VerenigingC));

        var result = await ctx.Bevestig(VerenigingC, Relatiegegevens.Create("LID-1", "Lid van de koepel"));

        ctx.Bevestigd.Should()
            .Be(new RelatieaanvraagWerdBevestigd(BevestigRelatieaanvraagContext.AanvraagId.Value, Today));

        ctx.VerenigingEvents.OfType<LidmaatschapWerdToegevoegd>()
            .Should()
            .ContainSingle()
            .Which.Lidmaatschap.Should()
            .BeEquivalentTo(
                new
                {
                    AndereVereniging = KoepelA.Value,
                    DatumVan = (DateOnly?)Today,
                    DatumTot = (DateOnly?)null,
                    Identificatie = "LID-1",
                    Beschrijving = "Lid van de koepel",
                }
            );

        result.AanvraagId.Should().Be(BevestigRelatieaanvraagContext.AanvraagId);
    }

    [Fact]
    public async ValueTask When_The_Koepel_Confirms_A_Relatieaanvraag_Of_The_Vereniging_Then_It_Becomes_Lid()
    {
        var ctx = Context(Vzer(VerenigingC), aangevraagdDoor: VerenigingC);

        await ctx.Bevestig(KoepelA);

        ctx.VerenigingEvents.OfType<LidmaatschapWerdToegevoegd>().Should().ContainSingle();
    }

    [Fact]
    public async ValueTask With_Subtype_Leeg_Then_It_Becomes_FV()
    {
        var ctx = Context(Vzer(VerenigingC));

        await ctx.Bevestig(VerenigingC);

        ctx.VerenigingEvents.OfType<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging>().Should().ContainSingle();
    }

    [Fact]
    public async ValueTask With_Subtype_NB_Then_It_Becomes_FV()
    {
        var nietBepaald = Vzer(VerenigingC)
            .Apply(new VerenigingssubtypeWerdTerugGezetNaarNietBepaald(VerenigingC.Value));

        var ctx = Context(nietBepaald);

        await ctx.Bevestig(VerenigingC);

        ctx.VerenigingEvents.OfType<VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging>().Should().ContainSingle();
    }

    [Fact]
    public async ValueTask With_Subtype_FV_Then_Only_The_Lidmaatschap_Is_Added()
    {
        var feitelijkeVereniging = Vzer(VerenigingC)
            .Apply(new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(VerenigingC.Value));

        var ctx = Context(feitelijkeVereniging);

        await ctx.Bevestig(VerenigingC);

        ctx.VerenigingEvents.Should().ContainSingle().Which.Should().BeOfType<LidmaatschapWerdToegevoegd>();
    }

    [Fact]
    public async ValueTask With_A_Vereniging_Met_Rechtspersoonlijkheid_Then_Only_The_Lidmaatschap_Is_Added()
    {
        var ctx = Context(KboVereniging(VerenigingC));

        await ctx.Bevestig(VerenigingC);

        ctx.VerenigingEvents.Should().ContainSingle().Which.Should().BeOfType<LidmaatschapWerdToegevoegd>();
    }

    [Fact]
    public async ValueTask With_An_Ended_Lidmaatschap_Bij_The_Koepel_Then_A_New_Lidmaatschap_Is_Added()
    {
        var withEndedLidmaatschap = Vzer(VerenigingC)
            .MetLidmaatschap(KoepelA, van: new DateOnly(2020, 1, 1), tot: Today.AddDays(-1));

        var ctx = Context(withEndedLidmaatschap);

        await ctx.Bevestig(VerenigingC);

        ctx.VerenigingEvents.OfType<LidmaatschapWerdToegevoegd>().Should().ContainSingle();
    }

    [Fact]
    public async ValueTask When_A_Lidmaatschap_Was_Added_Since_Then_It_Throws_ErBestaatAlEenActieveFeitelijkeRelatie()
    {
        var ctx = Context(Vzer(VerenigingC).MetLidmaatschap(KoepelA));

        await Assert.ThrowsAsync<ErBestaatAlEenActieveFeitelijkeRelatie>(async () => await ctx.Bevestig(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_The_Vereniging_Became_Subvereniging_Since_Then_It_Throws_SubverenigingKanGeenLidWorden()
    {
        var ctx = Context(Vzer(VerenigingC).AlsSubverenigingVan(KoepelB));

        await Assert.ThrowsAsync<SubverenigingKanGeenLidWorden>(async () => await ctx.Bevestig(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_An_Open_Subverenigingsaanvraag_Elsewhere_Then_It_Throws_VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden()
    {
        var subverenigingsaanvraag = new OpenRelatieaanvraag(RelatieaanvraagId.Create(2), RelatieType.Subvereniging);
        var ctx = Context(Vzer(VerenigingC), otherOpenRelatieaanvragen: [subverenigingsaanvraag]);

        await Assert.ThrowsAsync<VerenigingMetOpenSubverenigingsaanvraagKanGeenLidWorden>(async () =>
            await ctx.Bevestig(VerenigingC)
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_Other_Open_Lidmaatschapsaanvragen_Then_It_Is_Bevestigd()
    {
        var otherLidmaatschapsaanvraag = new OpenRelatieaanvraag(RelatieaanvraagId.Create(2), RelatieType.Lidmaatschap);
        var ctx = Context(Vzer(VerenigingC), otherOpenRelatieaanvragen: [otherLidmaatschapsaanvraag]);

        await ctx.Bevestig(VerenigingC);

        ctx.Bevestigd.Should().NotBeNull();
    }
}
