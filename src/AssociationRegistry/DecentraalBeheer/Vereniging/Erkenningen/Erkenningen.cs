namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using System.Collections.ObjectModel;
using Events;
using Exceptions;
using Framework;

public class Erkenningen : ReadOnlyCollection<Erkenning>
{
    private const int InitialId = 1;
    public int NextId { get; }
    public static Erkenningen Empty => new(Array.Empty<Erkenning>(), InitialId);

    private Erkenningen(IEnumerable<Erkenning> erkenningen, int nextId)
        : base(erkenningen.ToArray())
    {
        NextId = nextId;
    }

    public Erkenning GetById(int erkenningId) => this.Single(x => x.ErkenningId == erkenningId);

    private new Erkenning this[int erkenningId] => this.Single(x => x.ErkenningId == erkenningId);

    public Erkenning VoegToe(TeRegistrerenErkenning erkenning, IpdcProduct ipdcProduct, GegevensInitiator initiator)
    {
        var teRegistrerenErkenning = Erkenning.Create(NextId, erkenning, ipdcProduct, initiator);

        ThrowIfCannotAppendOrUpdate(teRegistrerenErkenning);

        return teRegistrerenErkenning;
    }

    private void ThrowIfCannotAppendOrUpdate(Erkenning teRegisterenErkenning)
    {
        Throw<ErkenningBestaatAl>.If(HeeftConflictMet(teRegisterenErkenning));
    }

    private bool HeeftConflictMet(Erkenning teRegistrerenErkenning)
    {
        return this.Any(bestaande => bestaande.HeeftConflictMet(teRegistrerenErkenning));
    }

    public Erkenningen Hydrate(IEnumerable<Erkenning> erkenningen)
    {
        erkenningen = erkenningen.ToArray();

        if (!erkenningen.Any())
            return new Erkenningen(Empty, Math.Max(InitialId, NextId));

        return new Erkenningen(erkenningen, Math.Max(erkenningen.Max(x => x.ErkenningId) + 1, NextId));
    }

    public void Schors(TeSchorsenErkenning teSchorsenErkenning, string initiator)
    {
        var erkenning = MustExists(teSchorsenErkenning.ErkenningId);

        Throw<ErkenningIsAlReedsGeschorst>.If(erkenning.Status == ErkenningStatus.Geschorst);
        Throw<ErkenningRedenSchorsingIsVerplicht>.If(string.IsNullOrEmpty(teSchorsenErkenning.RedenSchorsing));
        Throw<VerlopenErkenningKanNietGeschorstWorden>.If(erkenning.Status == ErkenningStatus.Verlopen);
        Throw<GiIsNIetBevoegd>.If(erkenning!.GeregistreerdDoor.OvoCode != initiator);
    }

    private Erkenning MustExists(int erkenningId)
    {
        var erkenning = this.SingleOrDefault(x => x.ErkenningId == erkenningId);

        Throw<ErkenningIsNietGekend>.If(erkenning == null, erkenningId.ToString());

        return erkenning!;
    }

    public Erkenning HefSchorsingErkenningOp(int erkenningId, string initiator)
    {
        var erkenning = this.SingleOrDefault(x => x.ErkenningId == erkenningId);

        Throw<ErkenningIsNietGekend>.If(erkenning == null, erkenningId.ToString());
        Throw<ErkenningIsNietGeschorst>.If(erkenning.Status != ErkenningStatus.Geschorst);
        Throw<GiIsNIetBevoegd>.If(erkenning!.GeregistreerdDoor.OvoCode != initiator);

        var today = DateOnly.FromDateTime(DateTime.Today);

        return erkenning with
        {
            Status = ErkenningStatus.Bepaal(erkenning.ErkenningsPeriode, today),
        };
    }

    public bool CorrigeerSchorsing(TeCorrigerenSchorsingErkenning teCorrigerenSchorsingErkenning, string initiator)
    {
        var erkenning = MustExists(teCorrigerenSchorsingErkenning.ErkenningId);

        Throw<ErkenningIsNietGeschorst>.If(erkenning.Status != ErkenningStatus.Geschorst);
        Throw<ErkenningRedenSchorsingIsVerplicht>.If(
            string.IsNullOrEmpty(teCorrigerenSchorsingErkenning.RedenSchorsing)
        );
        Throw<GiIsNIetBevoegd>.If(erkenning!.GeregistreerdDoor.OvoCode != initiator);

        var heeftWijzigingen = erkenning.RedenSchorsing != teCorrigerenSchorsingErkenning.RedenSchorsing;

        return heeftWijzigingen;
    }

    public void VerwijderErkenning(int erkenningId, string initiator)
    {
        var erkenning = this.SingleOrDefault(x => x.ErkenningId == erkenningId);

        Throw<ErkenningIsNietGekend>.If(erkenning == null, erkenningId.ToString());
        Throw<GiIsNIetBevoegd>.If(erkenning!.GeregistreerdDoor.OvoCode != initiator);
        Throw<ErkenningIsGeschorst>.If(erkenning.Status == ErkenningStatus.Geschorst);
        Throw<VerlopenErkenningKanNietVerwijderdWorden>.If(erkenning.Status == ErkenningStatus.Verlopen);
    }
}

public static class ErkenningenEnumerableExtensions
{
    public static IEnumerable<Erkenning> AppendFromEventData(
        this IEnumerable<Erkenning> erkenningen,
        ErkenningWerdGeregistreerd eventData
    ) =>
        erkenningen.Append(
            Erkenning.Hydrate(
                eventData.ErkenningId,
                eventData.GeregistreerdDoor,
                eventData.IpdcProduct,
                eventData.Startdatum,
                eventData.Einddatum,
                eventData.Hernieuwingsdatum,
                eventData.HernieuwingsUrl,
                string.Empty,
                eventData.Status
            )
        );

    public static IEnumerable<Erkenning> Without(this IEnumerable<Erkenning> source, int id)
    {
        return source.Where(c => c.ErkenningId != id);
    }
}
