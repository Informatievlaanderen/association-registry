namespace AssociationRegistry.Vereniging.Geotags;

using Grar.NutsLau;
using Marten;

public interface IGeotagsService
{
    Task<GeoTag[]> CalculateGeotags(Locatie[] postcodes);
    Task<GeoTag[]> CalculateGeotags(string[] postcodes);

}

public class GeotagsService : IGeotagsService
{
    private readonly IDocumentSession _session;

    public GeotagsService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<GeoTag[]> CalculateGeotags(Locatie[] locaties)
    {
        var postcodes = GetPostcodesFromLocaties(locaties);

        return await CalculateGeotags(postcodes);
    }

    public async Task<GeoTag[]> CalculateGeotags(string[] postcodes)
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

    private static string[] GetPostcodesFromLocaties(Locatie[] locaties)
    {
        return locaties
              .Where(x => x.Adres?.Postcode != null)
              .Select(x => x.Adres!.Postcode)
              .ToArray();
    }
}
