namespace AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;

using System.Collections.ObjectModel;
using Events;
using Exceptions;
using Framework;

public class Erkenningen : ReadOnlyCollection<Erkenning>
{
    public const int InitialId = 1;
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

    public Erkenningen Hydrate(IEnumerable<Erkenning> erkenningen)
    {
        erkenningen = erkenningen.ToArray();

        if (!erkenningen.Any())
            return new Erkenningen(Empty, Math.Max(InitialId, NextId));

        return new Erkenningen(erkenningen, Math.Max(erkenningen.Max(x => x.ErkenningId) + 1, NextId));
    }

    public bool KanErkenningWijzigenMetCombinatie(Erkenning erkenning) =>
        this.Without(erkenning).WithSameOvoCodeAndIpdcProduct(erkenning).HasNoOverlapWith(erkenning);

    public bool KanErkenningToevoegenMetCombinatie(Erkenning erkenning) =>
        this.WithSameOvoCodeAndIpdcProduct(erkenning).HasNoOverlapWith(erkenning);
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

    public static IEnumerable<Erkenning> WithSameOvoCodeAndIpdcProduct(
        this IEnumerable<Erkenning> erkenningen,
        Erkenning erkenning
    )
    {
        return erkenningen
            .Where(x => x.IpdcProduct.Equals(erkenning.IpdcProduct))
            .Where(x => x.GeregistreerdDoor.Equals(erkenning.GeregistreerdDoor));
    }

    public static bool HasNoOverlapWith(this IEnumerable<Erkenning> erkenningen, Erkenning erkenning)
    {
        return !erkenningen.Any(x => x.ErkenningsPeriode.OverlapsWith(erkenning.ErkenningsPeriode));
    }

    public static IEnumerable<Erkenning> Without(this IEnumerable<Erkenning> source, int id)
    {
        return source.Where(c => c.ErkenningId != id);
    }

    public static IEnumerable<Erkenning> Without(this IEnumerable<Erkenning> source, Erkenning erkenning)
    {
        return source.Where(c => c.ErkenningId != erkenning.ErkenningId);
    }
}
