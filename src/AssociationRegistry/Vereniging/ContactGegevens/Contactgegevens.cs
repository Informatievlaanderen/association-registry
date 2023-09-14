namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Bronnen;
using Framework;
using Exceptions;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    private const int InitialId = 1;
    private int NextId { get; }

    public static Contactgegevens Empty
        => new(Array.Empty<Contactgegeven>(), InitialId);

    private Contactgegevens(IEnumerable<Contactgegeven> contactgegevens, int nextId) : base(contactgegevens.ToArray())
    {
        NextId = nextId;
    }

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

        if (teWijzigenContactgegeven.WouldBeEquivalent(null, beschrijving, isPrimair, out var gewijzigdContactgegeven))
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

    private new Contactgegeven this[int contactgegevenId]
        => this.Single(x => x.ContactgegevenId == contactgegevenId);

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
            contactgegeven.Type);
    }

    private void MustNotHavePrimairOfTheSameTypeAs(Contactgegeven updatedContactgegeven)
    {
        Throw<MeerderePrimaireContactgegevensZijnNietToegestaan>.If(
            updatedContactgegeven.IsPrimair &&
            this.Without(updatedContactgegeven)
                .HasPrimairForType(updatedContactgegeven.Type),
            updatedContactgegeven.Type);
    }
}

public static class ContactgegevenEnumerableExtensions
{
    public static IEnumerable<Contactgegeven> Without(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegeven)
        => source.Without(contactgegeven.ContactgegevenId);

    public static IEnumerable<Contactgegeven> Without(this IEnumerable<Contactgegeven> source, int contactgegevenId)
        => source.Where(c => c.ContactgegevenId != contactgegevenId);

    public static bool HasPrimairForType(this IEnumerable<Contactgegeven> source, ContactgegevenType type)
        => source.Any(contactgegeven => contactgegeven.Type == type && contactgegeven.IsPrimair);

    public static bool ContainsMetZelfdeWaarden(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegeven)
        => source.Any(contactgegeven.IsEquivalentTo);

    public static bool WouldGiveMultiplePrimaryOfType(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegevenToEvaluate)
        => source.Without(contactgegevenToEvaluate)
                 .Any(contactgegeven => contactgegeven.Type == contactgegevenToEvaluate.Type && contactgegeven.IsPrimair);
}
