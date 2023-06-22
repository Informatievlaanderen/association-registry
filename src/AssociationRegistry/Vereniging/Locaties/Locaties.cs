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
        Throw<DuplicateLocatieProvided>.If(Items.Contains(locatie));
        Throw<DuplicatePrimaireLocatieProvided>.If(locatie.IsPrimair && HasPrimairelocatie);
        Throw<DuplicateCorrespondentielocatieProvided>.If(locatie.Locatietype == Locatietype.Correspondentie && HasCorrespondentieLocatie);

        var nextId = Math.Max(locatie.LocatieId + 1, NextId);
        return new Locaties(Items.Append(locatie).ToArray(), nextId);
    }

    public Locaties Remove(int locatieId)
        => new(Items.Where(l => l.LocatieId != locatieId).ToArray(), NextId);

    public new Locatie this[int locatieId]
        => this.Single(l => l.LocatieId == locatieId);
}
