namespace AssociationRegistry.ContactGegevens;

using System.Collections.ObjectModel;
using Exceptions;
using Framework;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    public int NextId
        => Count + 1;

    public static Contactgegevens Empty
        => new(Enumerable.Empty<Contactgegeven>().ToList());

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

    private Contactgegevens(IList<Contactgegeven> list) : base(list)
    {
    }

    public bool HasPrimairForType(ContactgegevenType type)
        => this.Any(contactgegeven => contactgegeven.Type == type && contactgegeven.IsPrimair);

    public Contactgegevens Append(Contactgegeven contactgegeven)
    {
        return new(((IEnumerable<Contactgegeven>)this).Append(contactgegeven with { ContactgegevenId = NextId }).ToList());
    }

    public new bool Contains(Contactgegeven contactgegeven)
        => this.Any(c => c.Type == contactgegeven.Type && c.Waarde == contactgegeven.Waarde);
}
