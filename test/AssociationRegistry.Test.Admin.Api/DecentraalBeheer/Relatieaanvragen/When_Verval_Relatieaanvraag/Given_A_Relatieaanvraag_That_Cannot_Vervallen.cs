namespace AssociationRegistry.Test.Admin.Api.DecentraalBeheer.Relatieaanvragen.When_Verval_Relatieaanvraag;

using AssociationRegistry.DecentraalBeheer.Relatieaanvragen;
using AssociationRegistry.DecentraalBeheer.Relatieaanvragen.Exceptions;
using AssociationRegistry.Events;
using Xunit;
using static Verenigingen;

/// <summary>
///     Verval is only for a relatieaanvraag that KanVervallenOp today: the handler checks that first and skips the others.
///     Called anyway, Verval throws, like VerloopErkenning.
/// </summary>
public class Given_A_Relatieaanvraag_That_Cannot_Vervallen
{
    private static readonly DateOnly AanvraagDatum = new(2026, 8, 20);
    private static readonly DateOnly VervalDatum = AanvraagDatum.AddDays(90);
    private static readonly RelatieaanvraagId AanvraagId = RelatieaanvraagId.Create(1);

    [Fact]
    public void On_The_Vervaldatum_Then_Verval_Throws_RelatieaanvraagKanNietVervallen() =>
        Assert.Throws<RelatieaanvraagKanNietVervallen>(() => Ingediend().Verval(VervalDatum));

    [Fact]
    public void When_Already_Bevestigd_Then_Verval_Throws_RelatieaanvraagKanNietVervallen() =>
        Assert.Throws<RelatieaanvraagKanNietVervallen>(() =>
            Ingediend()
                .Apply(new RelatieaanvraagWerdBevestigd(AanvraagId.Value, AanvraagDatum))
                .Verval(VervalDatum.AddDays(1))
        );

    private static RelatieaanvraagState Ingediend() =>
        new RelatieaanvraagState().Apply(
            new RelatieaanvraagWerdIngediend(
                AanvraagId.Value,
                KoepelA.Value,
                VerenigingC.Value,
                KoepelA.Value,
                RelatieType.Lidmaatschap.Value,
                "Aanvraag",
                AanvraagDatum,
                VervalDatum
            )
        );
}
