namespace AssociationRegistry.Grar.NutsLau;

using Marten;

public interface IPostcodesInSameGemeenteService
{
    /// <summary>
    ///     Returns the given postcodes plus all other postcodes belonging to the same gemeente (same Nuts3 + Lau).
    /// </summary>
    Task<string[]> WithPostcodesOfSameGemeente(string[] postcodes);
}

public class PostcodesInSameGemeenteService : IPostcodesInSameGemeenteService
{
    private readonly IQuerySession _session;

    public PostcodesInSameGemeenteService(IQuerySession session)
    {
        _session = session;
    }

    public async Task<string[]> WithPostcodesOfSameGemeente(string[] postcodes)
    {
        if (postcodes.Length == 0)
            return postcodes;

        var infos = await _session.Query<PostalNutsLauInfo>().Where(x => postcodes.Contains(x.Postcode)).ToListAsync();

        var nuts3Lau = infos.Select(x => x.Nuts3Lau).Distinct().ToArray();

        if (nuts3Lau.Length == 0)
            return postcodes.Distinct().ToArray();

        var sameGemeente = await _session
            .Query<PostalNutsLauInfo>()
            .Where(x => nuts3Lau.Contains(x.Nuts3Lau))
            .ToListAsync();

        return postcodes.Concat(sameGemeente.Select(x => x.Postcode)).Distinct().ToArray();
    }
}
