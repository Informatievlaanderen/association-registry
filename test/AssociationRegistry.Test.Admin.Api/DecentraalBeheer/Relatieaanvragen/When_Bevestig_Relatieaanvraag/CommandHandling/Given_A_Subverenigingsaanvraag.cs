namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Bevestig_Relatieaanvraag.CommandHandling;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;
using FluentAssertions;
using Xunit;
using static Verenigingen;

/// <summary>
///     Confirming makes the vereniging subvereniging of the koepel, in the same transaction as the confirmation, if the
///     rules of a subverenigingsaanvraag still hold.
/// </summary>
public class Given_A_Subverenigingsaanvraag
{
    private static BevestigRelatieaanvraagContext Context(
        VerenigingState vereniging,
        OpenRelatieaanvraag[]? otherOpenRelatieaanvragen = null,
        VerenigingState? koepel = null
    ) =>
        new(
            koepel ?? Koepel(KoepelA),
            vereniging,
            RelatieType.Subvereniging,
            aangevraagdDoor: KoepelA,
            otherOpenRelatieaanvragen
        );

    [Fact]
    public async ValueTask When_The_Vereniging_Confirms_Then_It_Becomes_Subvereniging_Of_The_Koepel()
    {
        var koepel = Koepel(KoepelA);
        var ctx = Context(Vzer(VerenigingC), koepel: koepel);

        await ctx.Bevestig(VerenigingC, Relatiegegevens.Create("SUB-1", "Afdeling"));

        ctx.VerenigingEvents.Should()
            .ContainSingle()
            .Which.Should()
            .Be(
                new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                    VerenigingC.Value,
                    new Registratiedata.SubverenigingVan(KoepelA.Value, koepel.Naam, "SUB-1", "Afdeling")
                )
            );
    }

    [Fact]
    public async ValueTask When_The_Vereniging_Became_FV_Since_Then_It_Throws_SubverenigingsaanvraagIsNietToegestaan()
    {
        var feitelijkeVereniging = Vzer(VerenigingC)
            .Apply(new VerenigingssubtypeWerdVerfijndNaarFeitelijkeVereniging(VerenigingC.Value));

        var ctx = Context(feitelijkeVereniging);

        await Assert.ThrowsAsync<SubverenigingsaanvraagIsNietToegestaan>(async () => await ctx.Bevestig(VerenigingC));

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask When_A_Lidmaatschap_Elsewhere_Was_Added_Since_Then_It_Throws_VerenigingMetLidmaatschapKanGeenSubverenigingWorden()
    {
        var ctx = Context(Vzer(VerenigingC).MetLidmaatschap(KoepelB));

        await Assert.ThrowsAsync<VerenigingMetLidmaatschapKanGeenSubverenigingWorden>(async () =>
            await ctx.Bevestig(VerenigingC)
        );

        ctx.ShouldNotHaveSaved();
    }

    [Fact]
    public async ValueTask With_An_Open_Lidmaatschapsaanvraag_Elsewhere_Then_It_Throws_VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden()
    {
        var lidmaatschapsaanvraag = new OpenRelatieaanvraag(RelatieaanvraagId.Create(2), RelatieType.Lidmaatschap);
        var ctx = Context(Vzer(VerenigingC), otherOpenRelatieaanvragen: [lidmaatschapsaanvraag]);

        await Assert.ThrowsAsync<VerenigingMetOpenLidmaatschapsaanvraagKanGeenSubverenigingWorden>(async () =>
            await ctx.Bevestig(VerenigingC)
        );

        ctx.ShouldNotHaveSaved();
    }
}
