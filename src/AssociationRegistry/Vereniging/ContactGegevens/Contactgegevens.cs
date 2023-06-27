namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Framework;
using Exceptions;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    private const int InitialId = 1;
    public int NextId { get; }

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
            return new(Empty, Math.Max(InitialId, NextId));

        return new(contactgegevens, Math.Max(contactgegevens.Max(x => x.ContactgegevenId) + 1, NextId));
    }

    public Contactgegeven[] VoegToe(params Contactgegeven[] toeTeVoegenContactgegevens)
    {
        var contactgegevens = this;
        var toegevoegdeContactgegevens = Array.Empty<Contactgegeven>();

        foreach (var toeTeVoegenContactgegeven in toeTeVoegenContactgegevens)
        {
            var contactgegevenMetId = toeTeVoegenContactgegeven with { ContactgegevenId = contactgegevens.NextId };

            contactgegevens.ThrowIfCannotAppend(contactgegevenMetId);
            contactgegevens = new Contactgegevens(contactgegevens.Append(contactgegevenMetId), contactgegevens.NextId + 1);

            toegevoegdeContactgegevens = toegevoegdeContactgegevens.Append(contactgegevenMetId).ToArray();
        }

        return toegevoegdeContactgegevens;
    }

    public Contactgegeven VoegToe(Contactgegeven toeTeVoegenContactgegeven)
    {
        ThrowIfCannotAppend(toeTeVoegenContactgegeven);

        return toeTeVoegenContactgegeven with { ContactgegevenId = NextId };
    }

    public Contactgegeven Verwijder(int contactgegevenId)
    {
        MustContain(contactgegevenId);

        return this[contactgegevenId];
    }

    private bool HasPrimairContactgegeven
        => Items.Any(l => l.IsPrimair);

    private void ThrowIfCannotAppend(Contactgegeven contactgegeven)
    {
        Throw<DuplicateContactgegeven>.If(ContainsMetZelfdeWaarden(contactgegeven), contactgegeven.Type);
        Throw<MultiplePrimaryContactgegevens>.If(contactgegeven.IsPrimair && this.HasPrimairForType(contactgegeven.Type), contactgegeven.Type);
    }

    public bool ContainsMetZelfdeWaarden(Contactgegeven contactgegeven)
        => this.Any(contactgegeven.MetZelfdeWaarden);

    public new Contactgegeven this[int contactgegevenId]
        => this.Single(x => x.ContactgegevenId == contactgegevenId);

    public bool HasKey(int contactgegevenId)
        => this.Any(contactgegeven => contactgegeven.ContactgegevenId == contactgegevenId);

    public bool ContainsOther(Contactgegeven contactgegeven)
        => Items
            .Without(contactgegeven)
            .Any(contactgegeven.MetZelfdeWaarden);

    public void MustContain(int contactgegevenId)
    {
        Throw<OnbekendContactgegeven>.If(!HasKey(contactgegevenId), contactgegevenId.ToString());
    }

    public void MustNotHaveDuplicates(Contactgegeven updatedContactgegeven)
    {
        Throw<DuplicateContactgegeven>.If(ContainsOther(updatedContactgegeven), updatedContactgegeven.Type);
    }

    public void MustNotHaveMultiplePrimaryOfTheSameType(Contactgegeven updatedContactgegeven)
    {
        Throw<MultiplePrimaryContactgegevens>.If(updatedContactgegeven.IsPrimair && this.WouldGiveMultiplePrimaryOfType(updatedContactgegeven), updatedContactgegeven.Type);
    }
}

public static class ContactgegevenEnumerableExtensions
{
    public static IEnumerable<Contactgegeven> Without(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegeven)
    {
        return source.Where(c => c.ContactgegevenId != contactgegeven.ContactgegevenId);
    }

    public static IEnumerable<Contactgegeven> Without(this IEnumerable<Contactgegeven> source, int contactgegevenId)
    {
        return source.Where(c => c.ContactgegevenId != contactgegevenId);
    }

    public static bool HasPrimairForType(this IEnumerable<Contactgegeven> source, ContactgegevenType type)
        => source.Any(contactgegeven => contactgegeven.Type == type && contactgegeven.IsPrimair);

    public static bool WouldGiveMultiplePrimaryOfType(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegevenToEvaluate)
        => source.Without(contactgegevenToEvaluate).Any(contactgegeven => contactgegeven.Type == contactgegevenToEvaluate.Type && contactgegeven.IsPrimair);
}
