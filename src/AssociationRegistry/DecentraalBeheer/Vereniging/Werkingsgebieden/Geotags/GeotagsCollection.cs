namespace AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;

using Events;
using System.Collections.ObjectModel;

public class GeotagsCollection : ReadOnlyCollection<Geotag>
{
    private readonly bool _initialised;
    private GeotagsCollection(IList<Geotag> list, bool initialised = true) : base(list)
    {
        _initialised = initialised;
    }

    public static GeotagsCollection CreateInstance(IList<Geotag> list)
    {
        return new GeotagsCollection(list.DistinctBy(x => x.Identificatie).ToList());
    }

    public static GeotagsCollection Null => new([], false);

    public override bool Equals(object? obj)
    {
        if (obj is not GeotagsCollection other)
            return false;

        return _initialised == other._initialised && this.ToHashSet().SetEquals(other);
    }

    public override int GetHashCode()
    {
        // Order-independent and duplicate-insensitive hash code
        var itemsHash = this.ToHashSet()
                            .Select(tag => tag.GetHashCode())
                            .Aggregate(0, (acc, h) => acc ^ h);
        var initHash = _initialised.GetHashCode(); // true => 1, false => 0

        return HashCode.Combine(itemsHash, initHash);
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
