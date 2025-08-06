namespace AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;

using Grar.NutsLau;
using Marten;

public interface IGeotagsService
{
    Task<GeotagsCollection> CalculateGeotags(IEnumerable<Locatie> locaties, IEnumerable<Werkingsgebied> werkingsgebieden);
    Task<GeotagsCollection> CalculateGeotags(string[] postcodes, string[] werkingsgebiedenCodes);
}

public class GeotagsService : IGeotagsService
{
    private readonly IDocumentSession _session;

    public GeotagsService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<GeotagsCollection> CalculateGeotags(IEnumerable<Locatie> locaties, IEnumerable<Werkingsgebied> werkingsgebieden)
    {
        var postcodes = GetPostcodesFromLocaties(locaties);
        var werkingsgebiedenCodes = GetWerkingsgebiedenCodeFromWerkingsgebieden(werkingsgebieden);

        return await CalculateGeotags(postcodes, werkingsgebiedenCodes);
    }

    public async Task<GeotagsCollection> CalculateGeotags(string[] postcodes, string[] werkingsgebiedenCodes)
    {
        var postalNutsLauInfos = _session.Query<PostalNutsLauInfo>()
                                         .Where(x => postcodes.Contains(x.Postcode)
                                                  || werkingsgebiedenCodes.Contains(x.Nuts3Lau)
                                                  || werkingsgebiedenCodes.Contains(x.Nuts2));

        var geoTags = Map(await postalNutsLauInfos.ToListAsync(), postcodes);

        var distinctGeoTags = geoTags
                             .Concat(postcodes.Select(x => new Geotag(x)))
                             .Distinct();

        return GeotagsCollection.Hydrate(distinctGeoTags.ToArray());
    }

    private Geotag[] Map(IReadOnlyList<PostalNutsLauInfo> postalNutsLauInfos, string[] postcodes)
    {
        var geotags = new List<Geotag>();

        foreach (var postalNutsLauInfo in postalNutsLauInfos)
        {
            var werkingsgebied = postalNutsLauInfo.Nuts3 + postalNutsLauInfo.Lau;
            var werkingsgebiedProvincie = postalNutsLauInfo.Nuts3[..4];
            var postcode = postalNutsLauInfo.Postcode;
            geotags.Add(new Geotag(werkingsgebied));
            geotags.Add(new Geotag(werkingsgebiedProvincie));
            geotags.Add(new Geotag(postcode));
        }

        return geotags.ToArray();
    }

    private static string[] GetPostcodesFromLocaties(IEnumerable<Locatie> locaties)
    {
        return locaties
              .Where(x => x.Adres?.Postcode != null)
              .Select(x => x.Adres!.Postcode)
              .ToArray();
    }

    private static string[] GetWerkingsgebiedenCodeFromWerkingsgebieden(IEnumerable<Werkingsgebied> werkingsgebieden)
    {
        return werkingsgebieden.Select(x => x.Code).ToArray();
    }
}
