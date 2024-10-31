namespace AssociationRegistry.Vereniging;

using Acties.VoegLidmaatschapToe;
using Exceptions;
using Framework;
using System.Collections.ObjectModel;

public class Lidmaatschappen : ReadOnlyCollection<Lidmaatschap>
{
    public LidmaatschapId NextId { get; }

    public static Lidmaatschappen Empty
        => new(Array.Empty<Lidmaatschap>(), LidmaatschapId.InitialId);


    private Lidmaatschappen(IEnumerable<Lidmaatschap> lidmaatschappen, LidmaatschapId nextId)
        : base(lidmaatschappen.ToArray())
    {
        NextId = nextId;
    }

    public Lidmaatschappen Hydrate(IEnumerable<Lidmaatschap> lidmaatschappen)
    {
        lidmaatschappen = lidmaatschappen.ToArray();

        if (!lidmaatschappen.Any())
            return new Lidmaatschappen(Empty, LidmaatschapId.Max(LidmaatschapId.InitialId, NextId));

        return new Lidmaatschappen(lidmaatschappen, CalculateNextId(lidmaatschappen));
    }

    private LidmaatschapId CalculateNextId(IEnumerable<Lidmaatschap> lidmaatschappen)
    {
        return new LidmaatschapId(Math.Max(lidmaatschappen.Max(x => x.LidmaatschapId), NextId)).Next;
    }

    public Lidmaatschap VoegToe(VoegLidmaatschapToeCommand.ToeTeVoegenLidmaatschap lidmaatschap)
    {
        var toeTeVoegenLidmaatschap = Lidmaatschap.Create(
            new LidmaatschapId(NextId),
            lidmaatschap);

        ThrowIfCannotAppendOrUpdate(toeTeVoegenLidmaatschap);
        return toeTeVoegenLidmaatschap;
    }

    public Lidmaatschap Verwijder(LidmaatschapId lidmaatschapId)
    {
        MustContain(lidmaatschapId);

        return this[lidmaatschapId];
    }

    private void MustContain(LidmaatschapId lidmaatschapId)
    {
        Throw<LidmaatschapIsNietGekend>.If(!HasKey(lidmaatschapId), lidmaatschapId.ToString());
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
