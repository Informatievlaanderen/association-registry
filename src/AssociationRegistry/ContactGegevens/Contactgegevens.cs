namespace AssociationRegistry.ContactGegevens;

using System.Collections.ObjectModel;

public class Contactgegevens : ReadOnlyCollection<Contactgegeven>
{
    public int NextId
        => Count + 1;

    public static Contactgegevens Empty
        => new(Enumerable.Empty<Contactgegeven>().ToList());

    private Contactgegevens(IList<Contactgegeven> list) : base(list)
    {
    }

    public bool HasPrimairForType(Contactgegeven.ContactgegevenType type)
        => this.Any(contactgegeven => contactgegeven.Type == type && contactgegeven.IsPrimair);

    public Contactgegevens Append(Contactgegeven contactgegeven)
        => new(((IEnumerable<Contactgegeven>)this).Append(contactgegeven).ToList());

    public new bool Contains(Contactgegeven contactgegeven)
        => this.Any(c => c.Type == contactgegeven.Type && c.Waarde == contactgegeven.Waarde);
}
