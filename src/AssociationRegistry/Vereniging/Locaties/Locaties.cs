namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using DuplicateVerenigingDetection;
using Events;
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

    public Locaties Hydrate(IEnumerable<Locatie> locaties)
    {
        locaties = locaties.ToArray();
        if (!locaties.Any())
            return new(Empty, Math.Max(InitialId, NextId));
        return new(locaties, Math.Max(locaties.Max(x => x.LocatieId) + 1, NextId));
    }

    public Locatie[] VoegToe(params Locatie[] toeTeVoegenLocaties)
    {
        var locaties = this;
        var toegevoegdeLocaties = Array.Empty<Locatie>();

        foreach (var toeTeVoegenLocatie in toeTeVoegenLocaties)
        {
            var locatieMetId = locaties.VoegToe(toeTeVoegenLocatie);

            locaties = new Locaties(locaties.Append(locatieMetId), locaties.NextId + 1);

            toegevoegdeLocaties = toegevoegdeLocaties.Append(locatieMetId).ToArray();
        }

        return toegevoegdeLocaties;
    }

    public Locatie VoegToe(Locatie toeTeVoegenLocatie)
    {
        ThrowIfCannotAppendOrUpdate(toeTeVoegenLocatie);

        return toeTeVoegenLocatie with { LocatieId = NextId };
    }

    public Locatie? Wijzig(int locatieId, string? naam, Locatietype? locatietype, bool? isPrimair, AdresId? adresId, Adres? adres)
    {
        MustContain(locatieId);


        var teWijzigenLocatie = this[locatieId];
        if (teWijzigenLocatie.WouldBeEquivalent(naam, locatietype, isPrimair, adresId,adres, out var gewijzigdeLocatie))
            return null;

        ThrowIfCannotAppendOrUpdate(gewijzigdeLocatie);

        return gewijzigdeLocatie;
    }

    public Locatie Verwijder(int locatieId)
    {
        MustContain(locatieId);

        return this[locatieId];
    }

    public new Locatie this[int locatieId]
        => this.Single(l => l.LocatieId == locatieId);

    private void MustContain(int locatieId)
    {
        Throw<UnknownLocatie>.If(!HasKey(locatieId), locatieId.ToString());
    }

    private bool HasKey(int locatieId)
        => this.Any(locatie => locatie.LocatieId == locatieId);

    private void ThrowIfCannotAppendOrUpdate(Locatie locatie)
    {
        MustNotHaveDuplicateOf(locatie);
        MustNotHaveMultiplePrimaireLocaties(locatie);
        MustNotHaveMultipleCorrespondentieLocaties(locatie);
    }

    private void MustNotHaveMultipleCorrespondentieLocaties(Locatie locatie)
        => Throw<MultipleCorrespondentieLocaties>.If(
            locatie.Locatietype == Locatietype.Correspondentie &&
            this.Without(locatie).HasCorrespondentieLocatie());

    private void MustNotHaveMultiplePrimaireLocaties(Locatie locatie)
        => Throw<MultiplePrimaireLocaties>.If(
            locatie.IsPrimair &&
            this.Without(locatie).HasPrimairelocatie());

    private void MustNotHaveDuplicateOf(Locatie locatie)
        => Throw<DuplicateLocatie>.If(this.Without(locatie).Contains(locatie));
}

public static class LocatieEnumerbleExtentions
{
    public static IEnumerable<Locatie> Without(this IEnumerable<Locatie> locaties, Locatie locatie)
        => locaties.Without(locatie.LocatieId);

    public static IEnumerable<Locatie> Without(this IEnumerable<Locatie> locaties, int locatieId)
        => locaties.Where(l => l.LocatieId != locatieId);

    public static bool HasPrimairelocatie(this IEnumerable<Locatie> locaties)
        => locaties.Any(l => l.IsPrimair);

    public static bool HasCorrespondentieLocatie(this IEnumerable<Locatie> locaties)
        => locaties.Any(l => l.Locatietype == Locatietype.Correspondentie);
}
