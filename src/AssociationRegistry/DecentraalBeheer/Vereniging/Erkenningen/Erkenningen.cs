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

    public Erkenning GetById(int erkenningId)
    {
        var erkenning = this.SingleOrDefault(x => x.ErkenningId == erkenningId);
        Throw<ErkenningIsNietGekend>.If(erkenning == null, erkenningId.ToString());

        return erkenning!;
    }

    public Erkenning VoegToe(TeRegistrerenErkenning erkenning, IpdcProduct ipdcProduct, GegevensInitiator initiator)
    {
        var teRegistrerenErkenning = Erkenning.Create(NextId, erkenning, ipdcProduct, initiator);

        Throw<ErkenningBestaatAl>.If(HeeftConflictMet(teRegistrerenErkenning));

        return teRegistrerenErkenning;
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

    public void KanGewijzigdeErkenningToevoegen(Erkenning erkenning)
    {
        var huidigeErkenningen = this.Without(erkenning.ErkenningId);

        var heeftConflictMetHuidigeErkenning = huidigeErkenningen.Any(bestaande =>
            bestaande.HeeftConflictMet(erkenning)
        );

        Throw<ErkenningBestaatAl>.If(heeftConflictMetHuidigeErkenning);
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
