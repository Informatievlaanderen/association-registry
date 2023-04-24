namespace AssociationRegistry.Vereniging;

using System.Collections.ObjectModel;
using Framework;
using Exceptions;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    private const int InitialId = 1;
    public int NextId { get; }


    private Contactgegevens(Contactgegeven[] contactgegevens, int nextId) : base(contactgegevens)
    {
        NextId = nextId;
    }

    public static Contactgegevens Empty
        => new(Array.Empty<Contactgegeven>(), InitialId);

    public static Contactgegevens FromArray(Contactgegeven[] contactgegevenArray)
    {
        var contactgegevens = Empty;
        foreach (var contactgegeven in contactgegevenArray)
        {
            Throw<DuplicateContactgegeven>.If(contactgegevens.ContainsMetZelfdeWaarden(contactgegeven), contactgegeven.Type);
            Throw<MultiplePrimaryContactgegevens>.If(contactgegeven.IsPrimair && contactgegevens.HasPrimairForType(contactgegeven.Type), contactgegeven.Type);
            contactgegevens = contactgegevens.Append(contactgegeven with { ContactgegevenId = contactgegevens.NextId });
        }

        return contactgegevens;
    }

    public Contactgegevens Append(Contactgegeven contactgegeven)
    {
        var nextId = Math.Max(contactgegeven.ContactgegevenId + 1, NextId);
        return new(Items.Append(contactgegeven).ToArray(), nextId);
    }

    public Contactgegevens Remove(int contectgegevenId)
        => new(Items.Where(c => c.ContactgegevenId != contectgegevenId).ToArray(), NextId);

    public Contactgegevens Replace(Contactgegeven contactgegeven)
        => Remove(contactgegeven.ContactgegevenId).Append(contactgegeven);

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

    public static bool HasPrimairForType(this IEnumerable<Contactgegeven> source, ContactgegevenType type)
        => source.Any(contactgegeven => contactgegeven.Type == type && contactgegeven.IsPrimair);

    public static bool WouldGiveMultiplePrimaryOfType(this IEnumerable<Contactgegeven> source, Contactgegeven contactgegevenToEvaluate)
        => source.Without(contactgegevenToEvaluate).Any(contactgegeven => contactgegeven.Type == contactgegevenToEvaluate.Type && contactgegeven.IsPrimair);
}
