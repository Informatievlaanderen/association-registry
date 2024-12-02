namespace AssociationRegistry.Grar.GrarUpdates.LocatieFinder;

using Acties.HeradresseerLocaties;
using Acties.OntkoppelAdres;
using AssociationRegistry.Grar.Models;
using Fusies.TeHeradresserenLocaties;
using Fusies.TeOntkoppelenLocaties;
using System.Collections.ObjectModel;

public class LocatieIdsPerVCodeCollection : ReadOnlyCollection<LocatieIdsPerVCode>
{
    private LocatieIdsPerVCodeCollection(IEnumerable<LocatieIdsPerVCode> locatieIdsPerVCodes): base(locatieIdsPerVCodes.ToList())
    {
    }

    public static new LocatieIdsPerVCodeCollection Empty = new([]);

    public static LocatieIdsPerVCodeCollection FromLocatiesPerVCode(Dictionary<string, int[]> locatieIdsGroupedByVCode)
        => new(locatieIdsGroupedByVCode.Select(s => new LocatieIdsPerVCode(s.Key, locatieIdsGroupedByVCode[s.Key])));

    public IEnumerable<HeradresseerLocatiesMessage> Map(int destinationAdresId)
    {
        return this.Select(x => new HeradresseerLocatiesMessage(
                                                       x.VCode,
                                                       x.LocatieIds.Select(locatieId => new TeHeradresserenLocatie(locatieId, destinationAdresId.ToString()))
                                                        .ToList(),
                                                       ""));
    }

    public IEnumerable<TeOntkoppelenLocatiesMessage> Map()
    {
        return this.Select(x => new TeOntkoppelenLocatiesMessage(
                                                       x.VCode,
                                                       x.LocatieIds));
    }
}
