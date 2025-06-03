namespace AssociationRegistry.Vereniging.Geotags;

using Events;
using System.Collections.ObjectModel;

public class GeotagsCollection : ReadOnlyCollection<Geotag>
{
    private GeotagsCollection(IList<Geotag> list) : base(list)
    {
    }

    public static GeotagsCollection CreateInstance(IList<Geotag> list)
    {
        return new GeotagsCollection(list.DistinctBy(x => x.Identificatie).ToList());
    }

    public override bool Equals(object? obj)
    {
        if (obj is not GeotagsCollection other)
            return false;

        return this.ToHashSet().SetEquals(other);
    }

    public override int GetHashCode()
    {
        // Order-independent and duplicate-insensitive hash code
        return this.ToHashSet()
                   .Select(tag => tag.GetHashCode())
                   .Aggregate(0, (acc, h) => acc ^ h);
    }

    public static bool operator ==(GeotagsCollection? left, GeotagsCollection? right)
        => Equals(left, right);

    public static bool operator !=(GeotagsCollection? left, GeotagsCollection? right)
        => !Equals(left, right);

    public static GeotagsCollection Hydrate(IEnumerable<Registratiedata.Geotag> geotags)
        => new(geotags.Select(x => new Geotag(x.Identificiatie)).ToList());

    public static GeotagsCollection Hydrate(IEnumerable<Geotag> geotags)
        => new(geotags.ToList());

    public static GeotagsCollection Empty
        => new(Array.Empty<Geotag>());
}
