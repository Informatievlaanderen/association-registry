namespace AssociationRegistry.Vereniging;

using Grar.NutsLau;
using System.Collections.ObjectModel;

public class MogelijkeWerkingsgebieden : ReadOnlyCollection<Werkingsgebied>
{
    public MogelijkeWerkingsgebieden(IList<Werkingsgebied> list) : base(list)
    {
    }

    public static MogelijkeWerkingsgebieden FromPostalNutsLauInfo(IEnumerable<PostalNutsLauInfo> postalNutsLauInfos)
    {
        var nutsLauWerkingsgebieden = postalNutsLauInfos
           .Select(x => Werkingsgebied.Hydrate(x.Nuts3, x.Lau, x.Gemeentenaam));

        var all = WellKnownWerkingsgebieden.Provincies
                                           .Concat(nutsLauWerkingsgebieden)
                                           .Append(Werkingsgebied.NietVanToepassing)
                                           .Distinct()
                                           .ToList();

        return new MogelijkeWerkingsgebieden(all);
    }
}
