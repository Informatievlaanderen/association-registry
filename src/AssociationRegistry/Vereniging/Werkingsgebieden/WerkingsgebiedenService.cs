namespace AssociationRegistry.Vereniging;

using Exceptions;
using Grar.NutsLau;
using Marten;

public interface IWerkingsgebiedenService
{
    Werkingsgebied Create(string code);
    IReadOnlyList<Werkingsgebied> AllWithNVT();
}

public class WerkingsgebiedenService : IWerkingsgebiedenService
{
    private readonly IDocumentSession _session;

    public WerkingsgebiedenService(IDocumentSession session)
    {
        _session = session;
    }

    public IReadOnlyList<Werkingsgebied> AllWithNVT()
    {
        var postalNutsLauInfos = _session.Query<PostalNutsLauInfo>().ToList();

        var nutsLauWerkingsgebieden = postalNutsLauInfos
           .Select(x => Werkingsgebied.Hydrate($"{x.Nuts}{x.Lau}", x.Gemeentenaam));

        var all = Werkingsgebied.ProvincieWerkingsgebieden
                                .Concat(nutsLauWerkingsgebieden)
                                .Append(Werkingsgebied.NietVanToepassing)
                                .Distinct()
                                .ToList();

        return all;
    }

    public Werkingsgebied Create(string code)
    {
        var all = AllWithNVT();

        var match = all.SingleOrDefault(w =>
                                            string.Equals(w.Code, code, StringComparison.InvariantCultureIgnoreCase));

        return match ?? throw new WerkingsgebiedCodeIsNietGekend(code);
    }
}
