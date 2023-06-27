namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class Locaties : ReadOnlyCollection<Locatie>
{
    private const int InitialId = 1;
    public int NextId { get; }

    public static Locaties Empty
        => new(Array.Empty<Locatie>(), InitialId);

    private Locaties(IEnumerable<Locatie> locaties, int nextId) : base(locaties.ToArray())
    {
        NextId = nextId;
    }

    public static Locaties Hydrate(IEnumerable<Locatie> locaties)
    {
        locaties = locaties.ToArray();

        return new(locaties, locaties.Max(x => x.LocatieId) + 1);
    }

    public Locatie[] VoegToe(params Locatie[] toeTeVoegenLocaties)
    {
        var locaties = this;
        var toegevoegdeLocaties = Array.Empty<Locatie>();

        foreach (var toeTeVoegenLocatie in toeTeVoegenLocaties)
        {
            var locatieMetId = toeTeVoegenLocatie with { LocatieId = locaties.NextId };

            locaties.ThrowIfCannotAppend(locatieMetId);
            locaties = new Locaties(locaties.Append(locatieMetId), locaties.NextId + 1);

            toegevoegdeLocaties = toegevoegdeLocaties.Append(locatieMetId).ToArray();
        }

        return toegevoegdeLocaties;
    }

    public Locatie VoegToe(Locatie toeTeVoegenLocatie)
    {
        ThrowIfCannotAppend(toeTeVoegenLocatie);

        return toeTeVoegenLocatie with { LocatieId = NextId };
    }

    private bool HasPrimairelocatie
        => Items.Any(l => l.IsPrimair);

    private bool HasCorrespondentieLocatie
        => Items.Any(l => l.Locatietype == Locatietype.Correspondentie);

    private void ThrowIfCannotAppend(Locatie locatie)
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
}
