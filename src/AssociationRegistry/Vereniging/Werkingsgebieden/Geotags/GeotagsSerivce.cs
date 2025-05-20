namespace AssociationRegistry.Vereniging.Geotags;

using Grar.NutsLau;
using Marten;

public interface IGeotagsSerivce
{
    Task<GeoTag[]> CalculateGeotagsByPostcode(string[] postcodes);
}

public class GeotagsSerivce : IGeotagsSerivce
{
    private readonly IDocumentSession _session;

    public GeotagsSerivce(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<GeoTag[]> CalculateGeotagsByPostcode(string[] postcodes)
    {
        var postalNutsLauInfos = _session.Query<PostalNutsLauInfo>()
                                         .Where(x => postcodes.Contains(x.Postcode));

        return Map(await postalNutsLauInfos.ToListAsync());
    }


    private GeoTag[] Map(IReadOnlyList<PostalNutsLauInfo> postalNutsLauInfos)
    {
        var geotags = new List<GeoTag>();
        foreach (var postalNutsLauInfo in postalNutsLauInfos)
        {
            var werkingsgebied = postalNutsLauInfo.Nuts + postalNutsLauInfo.Lau;
            var werkingsgebiedProvincie = postalNutsLauInfo.Nuts[..4];
            var postcode = postalNutsLauInfo.Postcode;
            geotags.Add(new GeoTag(werkingsgebied));
            geotags.Add(new GeoTag(werkingsgebiedProvincie));
            geotags.Add(new GeoTag(postcode));
        }

        return geotags.ToArray();
    }
}
