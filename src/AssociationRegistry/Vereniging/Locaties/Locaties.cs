namespace AssociationRegistry.Vereniging;

using Exceptions;
using Framework;
using System.Collections.ObjectModel;

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
            return new Locaties(Empty, Math.Max(InitialId, NextId));

        return new Locaties(locaties, Math.Max(locaties.Max(x => x.LocatieId) + 1, NextId));
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
        var locatie = Get(locatieId);

        var gewijzigdeLocatie = locatie.Wijzig(naam, locatietype, isPrimair, adresId, adres);

        if (gewijzigdeLocatie.Equals(locatie))
            return null;

        ThrowIfCannotAppendOrUpdate(gewijzigdeLocatie);

        return gewijzigdeLocatie;
    }

    public Locatie? Wijzig(int locatieId, string? naam, bool? isPrimair)
        => throw new NotImplementedException();

    public Locatie? WijzigVanuitKbo(int locatieId, string? naam, Locatietype? locatietype, bool? isPrimair, AdresId? adresId, Adres? adres)
        => throw new NotImplementedException();

    public Locatie Get(int locatieId)
    {
        MustContain(locatieId);

        return this[locatieId];
    }

    public Locatie Verwijder(int locatieId)
    {
        MustContain(locatieId);
        var teVerwijderenLocatie = this[locatieId];

        Throw<MaatschappelijkeZetelCanNotBeRemoved>.If(teVerwijderenLocatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo);

        return teVerwijderenLocatie;
    }

    public new Locatie this[int locatieId]
        => this.Single(l => l.LocatieId == locatieId);

    private void MustContain(int locatieId)
    {
        Throw<UnknownLocatie>.If(!HasKey(locatieId), locatieId.ToString());
    }

    private bool HasKey(int locatieId)
        => this.Any(locatie => locatie.LocatieId == locatieId);

    public void ThrowIfCannotAppendOrUpdate(Locatie locatie)
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
            this.Without(locatie).HasPrimaireLocatie());

    private void MustNotHaveDuplicateOf(Locatie locatie)
        => Throw<DuplicateLocatie>.If(this.Without(locatie).ContainsEquivalent(locatie));
}
