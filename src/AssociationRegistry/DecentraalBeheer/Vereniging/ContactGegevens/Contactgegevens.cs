namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Framework;
using AssociationRegistry.Vereniging.Bronnen;
using Exceptions;
using System.Collections.ObjectModel;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    private const int InitialId = 1;

    private Contactgegevens(IEnumerable<Contactgegeven> contactgegevens, int nextId) : base(contactgegevens.ToArray())
    {
        NextId = nextId;
    }

    private int NextId { get; }

    public static Contactgegevens Empty
        => new(Array.Empty<Contactgegeven>(), InitialId);

    private new Contactgegeven this[int contactgegevenId]
        => this.Single(x => x.ContactgegevenId == contactgegevenId);

    public Contactgegevens Hydrate(IEnumerable<Contactgegeven> contactgegevens)
    {
        contactgegevens = contactgegevens.ToArray();

        if (!contactgegevens.Any())
            return new Contactgegevens(Empty, Math.Max(InitialId, NextId));

        return new Contactgegevens(contactgegevens, Math.Max(contactgegevens.Max(x => x.ContactgegevenId) + 1, NextId));
    }

    public Contactgegeven[] VoegToe(params Contactgegeven[] toeTeVoegenContactgegevens)
    {
        var contactgegevens = this;
        var toegevoegdeContactgegevens = Array.Empty<Contactgegeven>();

        foreach (var toeTeVoegenContactgegeven in toeTeVoegenContactgegevens)
        {
            var contactgegevenMetId = contactgegevens.VoegToe(toeTeVoegenContactgegeven);

            contactgegevens = new Contactgegevens(contactgegevens.Append(contactgegevenMetId), contactgegevens.NextId + 1);

            toegevoegdeContactgegevens = toegevoegdeContactgegevens.Append(contactgegevenMetId).ToArray();
        }

        return toegevoegdeContactgegevens;
    }

    public Contactgegeven VoegToe(Contactgegeven toeTeVoegenContactgegeven)
    {
        ThrowIfCannotAppendOrUpdate(toeTeVoegenContactgegeven);

        return toeTeVoegenContactgegeven with { ContactgegevenId = NextId };
    }

    public Contactgegeven? Wijzig(int contactgegevenId, string? waarde, string? beschrijving, bool? isPrimair)
    {
        MustContain(contactgegevenId);

        var teWijzigenContactgegeven = this[contactgegevenId];
        Throw<ContactgegevenUitKboKanNietGewijzigdWorden>.If(teWijzigenContactgegeven.Bron == Bron.KBO);

        if (teWijzigenContactgegeven.WouldBeEquivalent(waarde, beschrijving, isPrimair, out var gewijzigdContactgegeven))
            return null;

        ThrowIfCannotAppendOrUpdate(gewijzigdContactgegeven);

        return gewijzigdContactgegeven;
    }

    public Contactgegeven? Wijzig(int contactgegevenId, string? beschrijving, bool? isPrimair)
    {
        MustContain(contactgegevenId);

        var teWijzigenContactgegeven = this[contactgegevenId];

        if (teWijzigenContactgegeven.WouldBeEquivalent(waarde: null, beschrijving, isPrimair, out var gewijzigdContactgegeven))
            return null;

        ThrowIfCannotAppendOrUpdate(gewijzigdContactgegeven);

        return gewijzigdContactgegeven;
    }

    public Contactgegeven Verwijder(int contactgegevenId)
    {
        MustContain(contactgegevenId);

        var contactgegeven = this[contactgegevenId];
        Throw<ContactgegevenUitKboKanNietVerwijderdWorden>.If(contactgegeven.Bron == Bron.KBO);

        return contactgegeven;
    }

    private void ThrowIfCannotAppendOrUpdate(Contactgegeven contactgegeven)
    {
        MustNotHaveDuplicateOf(contactgegeven);
        MustNotHavePrimairOfTheSameTypeAs(contactgegeven);
    }

    public bool ContainsMetZelfdeWaarden(Contactgegeven contactgegeven)
        => this.Any(contactgegeven.IsEquivalentTo);

    public Contactgegeven? MetZelfdeWaarden(Contactgegeven contactgegeven)
        => this.SingleOrDefault(contactgegeven.IsEquivalentTo);

    private bool HasKey(int contactgegevenId)
        => this.Any(contactgegeven => contactgegeven.ContactgegevenId == contactgegevenId);

    private void MustContain(int contactgegevenId)
    {
        Throw<ContactgegevenIsNietGekend>.If(!HasKey(contactgegevenId), contactgegevenId.ToString());
    }

    private void MustNotHaveDuplicateOf(Contactgegeven contactgegeven)
    {
        Throw<ContactgegevenIsDuplicaat>.If(
            this.Without(contactgegeven)
                .ContainsMetZelfdeWaarden(contactgegeven),
            contactgegeven.Contactgegeventype);
    }

    private void MustNotHavePrimairOfTheSameTypeAs(Contactgegeven updatedContactgegeven)
    {
        Throw<MeerderePrimaireContactgegevensZijnNietToegestaan>.If(
            updatedContactgegeven.IsPrimair &&
            this.Without(updatedContactgegeven)
                .HasPrimairForType(updatedContactgegeven.Contactgegeventype),
            updatedContactgegeven.Contactgegeventype);
    }

    public Contactgegeven? WijzigUitKbo(int id, string? waarde)
    {
        var contactgegeven = this[id];

        if (contactgegeven.WouldBeEquivalent(waarde: waarde, null, null, out var gewijzigdContactgegeven))
            return null;

        return gewijzigdContactgegeven;
    }

    public Contactgegeven? GetContactgegevenOfKboType(ContactgegeventypeVolgensKbo typeVolgensKbo)
    {
        return this.SingleOrDefault(c => c.TypeVolgensKbo == typeVolgensKbo);
    }
}

public static class ContactgegevenEnumerableExtensions
{
    public static IEnumerable<Contactgegeven> Without(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegeven)
        => source.Without(contactgegeven.ContactgegevenId);

    public static IEnumerable<Contactgegeven> Without(this IEnumerable<Contactgegeven> source, int contactgegevenId)
        => source.Where(c => c.ContactgegevenId != contactgegevenId);

    public static bool HasPrimairForType(this IEnumerable<Contactgegeven> source, Contactgegeventype type)
        => source.Any(contactgegeven => contactgegeven.Contactgegeventype == type && contactgegeven.IsPrimair);

    public static bool ContainsMetZelfdeWaarden(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegeven)
        => source.Any(contactgegeven.IsEquivalentTo);

    public static bool WouldGiveMultiplePrimaryOfType(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegevenToEvaluate)
        => source.Without(contactgegevenToEvaluate)
                 .Any(contactgegeven => contactgegeven.Contactgegeventype == contactgegevenToEvaluate.Contactgegeventype &&
                                        contactgegeven.IsPrimair);
}
