namespace AssociationRegistry.DecentraalBeheer.Vereniging;

using Exceptions;
using Grar.NutsLau;
using Marten;

public interface IWerkingsgebiedenService
{
    Task<Werkingsgebied> Create(string code, CancellationToken cancellationToken = default);
    Task<Werkingsgebied[]> CreateMany(IEnumerable<string> codes, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Werkingsgebied>> AllWithNVT(CancellationToken cancellationToken = default);
}

public class WerkingsgebiedenService : IWerkingsgebiedenService
{
    private readonly IDocumentSession _session;

    public WerkingsgebiedenService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IReadOnlyList<Werkingsgebied>> AllWithNVT(CancellationToken cancellationToken = default)
    {
        var postalNutsLauInfos = await _session.Query<PostalNutsLauInfo>().ToListAsync(cancellationToken);

        return MogelijkeWerkingsgebieden.FromPostalNutsLauInfo(postalNutsLauInfos);
    }

    public async Task<Werkingsgebied> Create(string code, CancellationToken cancellationToken = default)
    {
        var all = await AllWithNVT(cancellationToken);

        return Match(all, code);
    }

    public async Task<Werkingsgebied[]> CreateMany(
        IEnumerable<string> codes,
        CancellationToken cancellationToken = default
    )
    {
        var all = await AllWithNVT(cancellationToken);

        return codes.Select(code => Match(all, code)).ToArray();
    }

    private static Werkingsgebied Match(IReadOnlyList<Werkingsgebied> all, string code)
    {
        var match = all.SingleOrDefault(w => string.Equals(w.Code, code, StringComparison.InvariantCultureIgnoreCase));

        return match ?? throw new WerkingsgebiedCodeIsNietGekend(code);
    }
}
