namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;
using System.Collections.ObjectModel;

public class Lidmaatschappen : ReadOnlyCollection<Lidmaatschap>
{

    private const int InitialId = 1;
    public int NextId { get; }

    public static Lidmaatschappen Empty
        => new(Array.Empty<Lidmaatschap>(), InitialId);


    private Lidmaatschappen(IEnumerable<Lidmaatschap> lidmaatschappen, int nextId)
        : base(lidmaatschappen.ToArray())
    {
        NextId = nextId;
    }

    public Lidmaatschappen Hydrate(IEnumerable<Lidmaatschap> lidmaatschappen)
    {
        lidmaatschappen = lidmaatschappen.ToArray();

        if (!lidmaatschappen.Any())
            return new Lidmaatschappen(Empty, Math.Max(InitialId, NextId));

        return new Lidmaatschappen(lidmaatschappen, Math.Max(lidmaatschappen.Max(x => x.LidmaatschapId) + 1, NextId));
    }

    public Lidmaatschap[] VoegToe(params Lidmaatschap[] toeTeVoegenLidmaatschappen)
    {
        var lidmaatschappen = this;
        var toegevoegdeLidmaatschappen = Array.Empty<Lidmaatschap>();

        foreach (var toeTeVoegenLidmaatschap in toeTeVoegenLidmaatschappen)
        {
            var lidmaatschapMetId = lidmaatschappen.VoegToe(toeTeVoegenLidmaatschap);

            lidmaatschappen = new Lidmaatschappen(lidmaatschappen.Append(lidmaatschapMetId), lidmaatschappen.NextId + 1);

            toegevoegdeLidmaatschappen = toegevoegdeLidmaatschappen.Append(lidmaatschapMetId).ToArray();
        }

        return toegevoegdeLidmaatschappen;
    }

    public Lidmaatschap VoegToe(Lidmaatschap toeTeVoegenLidmaatschap)
    {
        ThrowIfCannotAppendOrUpdate(toeTeVoegenLidmaatschap);

        return toeTeVoegenLidmaatschap with { LidmaatschapId = NextId };
    }

    private void ThrowIfCannotAppendOrUpdate(Lidmaatschap lidmaatschap)
    {
        MustNotOverlapForSameAndereVereniging(lidmaatschap);
    }

    private void MustNotOverlapForSameAndereVereniging(Lidmaatschap lidmaatschap)
    {
        Throw<LidmaatschapIsOverlappend>.If(
            Items
               .Where(x => lidmaatschap.AndereVereniging == x.AndereVereniging)
               .Any(x => lidmaatschap.Geldigheidsperiode.OverlapsWith(x.Geldigheidsperiode)));
    }

    public new Lidmaatschap this[int lidmaatschapId]
        => this.Single(l => l.LidmaatschapId == lidmaatschapId);


    public bool HasKey(int lidmaatschapId)
        => this.Any(lidmaatschap => lidmaatschap.LidmaatschapId == lidmaatschapId);
}
