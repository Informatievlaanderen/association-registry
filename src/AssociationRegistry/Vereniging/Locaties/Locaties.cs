namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class Locaties : ReadOnlyCollection<Locatie>
{
    private const int InitialId = 1;
    public int NextId { get; }

    private Locaties(Locatie[] locaties, int nextId) : base(locaties)
    {
        NextId = nextId;
    }

    public bool HasPrimairelocatie
        => Items.Any(l => l.IsPrimair);

    public bool HasCorrespondentieLocatie
        => Items.Any(l => l.Locatietype == Locatietype.Correspondentie);

    public static Locaties Empty
        => new(Array.Empty<Locatie>(), InitialId);

    public static Locaties FromArray(Locatie[] locatiesArray)
    {
        return locatiesArray.Aggregate(
            Empty,
            (current, locatie) =>
                current.Append(locatie with { LocatieId = current.NextId }));
    }

    public Locaties Append(Locatie locatie)
    {
        ThrowIfCannotAppend(locatie);

        var nextId = Math.Max(locatie.LocatieId + 1, NextId);
        return new Locaties(Items.Append(locatie).ToArray(), nextId);
    }

    public void ThrowIfCannotAppend(Locatie locatie)
    {
        MustNotHaveDuplicateOf(locatie);
        MustNotHaveMultiplePrimaireLocaties(locatie);
        MustNotHaveMultipleCorrespondentieLocaties(locatie);
    }

    private void MustNotHaveMultipleCorrespondentieLocaties(Locatie locatie)
    {
        Throw<DuplicateCorrespondentielocatieProvided>.If(locatie.Locatietype == Locatietype.Correspondentie && HasCorrespondentieLocatie);
    }

    private void MustNotHaveMultiplePrimaireLocaties(Locatie locatie)
    {
        Throw<DuplicatePrimaireLocatieProvided>.If(locatie.IsPrimair && HasPrimairelocatie);
    }

    private void MustNotHaveDuplicateOf(Locatie locatie)
    {
        Throw<DuplicateLocatieProvided>.If(Items.Contains(locatie));
    }

    public Locaties Remove(int locatieId)
        => new(Items.Where(l => l.LocatieId != locatieId).ToArray(), NextId);

    public new Locatie this[int locatieId]
        => this.Single(l => l.LocatieId == locatieId);

    public static Locaties Hydrate(Locatie[] locaties, int nextId)
        => new(locaties, nextId);
}
