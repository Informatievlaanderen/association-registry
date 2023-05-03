namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class Vertegenwoordigers : ReadOnlyCollection<Vertegenwoordiger>
{
    private const int InitialId = 1;

    private Vertegenwoordiger? Primair
        => Items.SingleOrDefault(i => i.IsPrimair);

    private Vertegenwoordigers(Vertegenwoordiger[] vertegenwoordigers, int nextId) : base(vertegenwoordigers)
    {
        NextId = nextId;
    }

    public int NextId { get; }

    public static Vertegenwoordigers Empty
        => new(Array.Empty<Vertegenwoordiger>(), InitialId);

    public static Vertegenwoordigers FromArray(Vertegenwoordiger[] vertegenwoordigerArray)
    {
        Throw<DuplicateInszProvided>.If(HasDuplicateInsz(vertegenwoordigerArray));
        Throw<MultiplePrimaryVertegenwoordigers>.If(HasMultiplePrimaryVertegenwoordigers(vertegenwoordigerArray));

        return vertegenwoordigerArray.Aggregate(
            Empty,
            (current, vertegenwoordiger) =>
                current.Append(vertegenwoordiger with { VertegenwoordigerId = current.NextId }));
    }

    public Vertegenwoordigers Append(Vertegenwoordiger vertegenwoordiger)
    {
        var nextId = Math.Max(vertegenwoordiger.VertegenwoordigerId + 1, NextId);
        return new Vertegenwoordigers(Items.Append(vertegenwoordiger).ToArray(), nextId);
    }

    public Vertegenwoordigers Remove(int vertegenwoordigerId)
        => new(Items.Where(v => v.VertegenwoordigerId != vertegenwoordigerId).ToArray(), NextId);


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

    public Vertegenwoordigers Replace(Vertegenwoordiger vertegenwoordiger)
        => Remove(vertegenwoordiger.VertegenwoordigerId).Append(vertegenwoordiger);
}
