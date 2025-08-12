namespace AssociationRegistry.Grar.Models.PostalInfo;

using System.Collections.ObjectModel;

public class Postnamen: ReadOnlyCollection<Postnaam>, IReadOnlyCollection<Postnaam>
{
    public Postnamen(IList<Postnaam> list) : base(list)
    {
    }

    public bool HasSinglePostnaam => Count == 1;


    public Postnaam? FindSingleWithGemeentenaam(string origineleGemeentenaamClean)
    {
        return this.SingleOrDefault(
            sod => sod.Value.Equals(origineleGemeentenaamClean, StringComparison.InvariantCultureIgnoreCase));
    }

    public Postnaam? FindSingleOrDefault()
        => HasSinglePostnaam ? this.SingleOrDefault() : null;

    public static Postnamen FromValues(params string[] values)
        => new(values.Select(Postnaam.FromValue).ToList());

    public static Postnamen Empty => new ([]);
}
