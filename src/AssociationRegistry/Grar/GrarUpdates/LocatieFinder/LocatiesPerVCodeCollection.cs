namespace AssociationRegistry.Grar.GrarUpdates.LocatieFinder;

using GrarConsumer.Messaging.HeradresseerLocaties;
using GrarConsumer.Messaging.OntkoppelAdres;
using Hernummering;
using System.Collections.ObjectModel;

public class LocatiesPerVCodeCollection : ReadOnlyCollection<LocatiesPerVCode>
{
    private LocatiesPerVCodeCollection(IEnumerable<LocatiesPerVCode> locatieIdsPerVCodes): base(locatieIdsPerVCodes.ToList())
    {
    }

    public static readonly new LocatiesPerVCodeCollection Empty = new([]);

    public static LocatiesPerVCodeCollection FromLocatiesPerVCode(Dictionary<string, LocatieLookupData[]> locatieLookupDataGroupedByVCode)
        => new(locatieLookupDataGroupedByVCode.Select(s => new LocatiesPerVCode(s.Key, locatieLookupDataGroupedByVCode[s.Key])));

    public IEnumerable<HeradresseerLocatiesMessage> Map(int destinationAdresId, string idempotencyKey)
    {
        return this.Select(x => new HeradresseerLocatiesMessage(
                                                       x.VCode,
                                                       x.Locaties.Select(locatie => new TeHeradresserenLocatie(locatie.LocatieId, destinationAdresId.ToString()))
                                                        .ToList(),
                                                       idempotencyKey));
    }

    public IEnumerable<OntkoppelLocatiesMessage> Map()
    {
        return this.Select(x => new OntkoppelLocatiesMessage(
                                                       x.VCode,
                                                       x.Locaties.Select(x => x.LocatieId).ToArray()));
    }
}
