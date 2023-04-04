namespace AssociationRegistry.ContactGegevens;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    private const int InitialId = 1;
    public int NextId { get; }

    public static Contactgegevens Empty
        => new(Enumerable.Empty<Contactgegeven>().ToList(), InitialId);

    public static Contactgegevens FromArray(Contactgegeven[] contactgegevenArray)
    {
        var contactgegevens = Empty;
        foreach (var contactgegeven in contactgegevenArray)
        {
            Throw<DuplicateContactgegeven>.If(contactgegevens.Contains(contactgegeven), Enum.GetName(contactgegeven.Type));
            Throw<MultiplePrimaryContactgegevens>.If(contactgegeven.IsPrimair && contactgegevens.HasPrimairForType(contactgegeven.Type), Enum.GetName(contactgegeven.Type));
            contactgegevens = contactgegevens.Append(contactgegeven);
        }

        return contactgegevens;
    }

    private Contactgegevens(IList<Contactgegeven> list, int nextId) : base(list)
    {
        NextId = nextId;
    }

    public bool HasPrimairForType(ContactgegevenType type)
        => this.Any(contactgegeven => contactgegeven.Type == type && contactgegeven.IsPrimair);

    public Contactgegevens Append(Contactgegeven contactgegeven)
        => new(((IEnumerable<Contactgegeven>)this).Append(contactgegeven with { ContactgegevenId = NextId }).ToList(), NextId + 1);

    public new bool Contains(Contactgegeven contactgegeven)
        => this.Any(c => c.Type == contactgegeven.Type && c.Waarde == contactgegeven.Waarde);

    public new Contactgegeven this[int contactgegevenId]
        => this.Single(x => x.ContactgegevenId == contactgegevenId);

    public bool HasKey(int contactgegevenId)
        => this.Any(contactgegeven => contactgegeven.ContactgegevenId == contactgegevenId);
}
