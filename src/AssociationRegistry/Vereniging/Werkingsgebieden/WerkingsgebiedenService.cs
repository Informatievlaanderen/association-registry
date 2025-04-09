namespace AssociationRegistry.Vereniging;

using Exceptions;
using Grar.NutsLau;
using Marten;

public interface IWerkingsgebiedenService
{
    Werkingsgebied Create(string code);
    void CacheWerkingsgebieden();
}

public class WerkingsgebiedenService : IWerkingsgebiedenService
{
    private readonly IDocumentSession _session;
    private static List<Werkingsgebied> _werkingsgebieden = new();

    public WerkingsgebiedenService(IDocumentSession session)
    {
        _session = session;
        CacheWerkingsgebieden();
    }

    public IReadOnlyList<Werkingsgebied> All => _werkingsgebieden;
    public IReadOnlyList<Werkingsgebied> AllWithNVT => _werkingsgebieden.Append(Werkingsgebied.NietVanToepassing).ToList();

    public Werkingsgebied Create(string code)
    {
        var match = _werkingsgebieden
           .SingleOrDefault(w => string.Equals(w.Code, code, StringComparison.InvariantCultureIgnoreCase));

        return match ?? throw new WerkingsgebiedCodeIsNietGekend(code);
    }

    public void CacheWerkingsgebieden()
    {
        var postalNutsLauInfos = _session.Query<PostalNutsLauInfo>().ToList();
        var werkingsgebieden = postalNutsLauInfos.Select(x => Werkingsgebied.Hydrate($"{x.Nuts}{x.Lau}", x.Gemeentenaam));

        _werkingsgebieden =
            Werkingsgebied.ProvincieWerkingsgebieden
                          .Concat(werkingsgebieden)
                          .Distinct()
                          .ToList();
    }
}
