namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class Vertegenwoordigers : ReadOnlyCollection<Vertegenwoordiger>
{
    private const int InitialId = 1;

    public int NextId { get; }

    private Vertegenwoordiger? Primair
        => Items.SingleOrDefault(i => i.IsPrimair);

    private Vertegenwoordigers(IEnumerable<Vertegenwoordiger> vertegenwoordigers, int nextId) : base(vertegenwoordigers.ToArray())
    {
        NextId = nextId;
    }

    public static Vertegenwoordigers Empty
        => new(Array.Empty<Vertegenwoordiger>(), InitialId);

    public Vertegenwoordigers Hydrate(IEnumerable<Vertegenwoordiger> vertegenwoordigers)
    {
        vertegenwoordigers = vertegenwoordigers.ToArray();

        if (!vertegenwoordigers.Any())
            return new(Empty, Math.Max(InitialId, NextId));

        return new(vertegenwoordigers, Math.Max(vertegenwoordigers.Max(x => x.VertegenwoordigerId) + 1, NextId));
    }

    public Vertegenwoordiger[] VoegToe(params Vertegenwoordiger[] toeTeVoegenVertegenwoordigers)
    {
        var vertegenwoordigers = this;
        var toegevoegdeVertegenwoordigers = Array.Empty<Vertegenwoordiger>();

        foreach (var toeTeVoegenVertegenwoordiger in toeTeVoegenVertegenwoordigers)
        {
            var vertegenwoordigerMetId = toeTeVoegenVertegenwoordiger with { VertegenwoordigerId = vertegenwoordigers.NextId };

            vertegenwoordigers.ThrowIfCannotAppend(vertegenwoordigerMetId);
            vertegenwoordigers = new Vertegenwoordigers(vertegenwoordigers.Append(vertegenwoordigerMetId), vertegenwoordigers.NextId + 1);

            toegevoegdeVertegenwoordigers = toegevoegdeVertegenwoordigers.Append(vertegenwoordigerMetId).ToArray();
        }

        return toegevoegdeVertegenwoordigers;
    }

    public Vertegenwoordiger Verwijder(int vertegenwoordigerId)
    {
        MustContain(vertegenwoordigerId);

        return this[vertegenwoordigerId];
    }

    private void ThrowIfCannotAppend(Vertegenwoordiger vertegenwoordiger)
    {
        var vertegenwoordigers = this.Append(vertegenwoordiger).ToArray();

        Throw<DuplicateInszProvided>.If(HasDuplicateInsz(vertegenwoordigers));
        Throw<MultiplePrimaryVertegenwoordigers>.If(HasMultiplePrimaryVertegenwoordigers(vertegenwoordigers));
    }


    public new Vertegenwoordiger this[int vertegenwoordigerId]
        => this.Single(v => v.VertegenwoordigerId == vertegenwoordigerId);


    private static bool HasMultiplePrimaryVertegenwoordigers(Vertegenwoordiger[] vertegenwoordigersArray)
        => vertegenwoordigersArray.Count(vertegenwoordiger => vertegenwoordiger.IsPrimair) > 1;

    private static bool HasDuplicateInsz(Vertegenwoordiger[] vertegenwoordigers)
        => vertegenwoordigers.DistinctBy(x => x.Insz).Count() != vertegenwoordigers.Length;

    public void MustNotHaveDuplicateOf(Vertegenwoordiger vertegenwoordiger)
        => Throw<DuplicateInszProvided>.If(
            HasDuplicateInsz(Items.Append(vertegenwoordiger).ToArray()));

    public void MustNotHaveMultiplePrimary(Vertegenwoordiger vertegenwoordiger)
    {
        Throw<MultiplePrimaryVertegenwoordigers>.If(
            Primair is not null && // there is a primair vertegenwoordiger
            Primair.VertegenwoordigerId != vertegenwoordiger.VertegenwoordigerId && // it is not the same
            vertegenwoordiger.IsPrimair); // we want to make it primair
    }

    public void MustContain(int vertegenwoordigerId)
    {
        Throw<UnknownVertegenwoordiger>.IfNot(Items.Any(vertegenwoordiger => vertegenwoordiger.VertegenwoordigerId == vertegenwoordigerId));
    }
}

public static class VertegenwoordigerEnumerableExtensions
{
    public static IEnumerable<Vertegenwoordiger> Without(this IEnumerable<Vertegenwoordiger> source, Vertegenwoordiger vertegenwoordiger)
    {
        return source.Where(c => c.VertegenwoordigerId != vertegenwoordiger.VertegenwoordigerId);
    }

    public static IEnumerable<Vertegenwoordiger> Without(this IEnumerable<Vertegenwoordiger> source, int vertegenwoordigerId)
    {
        return source.Where(c => c.VertegenwoordigerId != vertegenwoordigerId);
    }
}
