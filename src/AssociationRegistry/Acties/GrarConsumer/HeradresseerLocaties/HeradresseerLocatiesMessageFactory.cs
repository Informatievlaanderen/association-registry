namespace AssociationRegistry.Acties.GrarConsumer.HeradresseerLocaties;

using Grar.GrarUpdates.Hernummering;
using Grar.GrarUpdates.LocatieFinder;
using JasperFx.Core.Reflection;

public class HeradresseerLocatiesMessageFactory
{
    private readonly LocatiesPerVCodeCollection _locatiesPerVCode;

    private HeradresseerLocatiesMessageFactory(LocatiesPerVCodeCollection locatiesPerVCode)
    {
        _locatiesPerVCode = locatiesPerVCode;
    }

    public static HeradresseerLocatiesMessageFactory VoorLocaties(LocatiesPerVCodeCollection locatiesPerVCode)
        => new(locatiesPerVCode);

    public IReadOnlyCollection<HeradresseerLocatiesMessage> MetNaarAdres(int destinationAdresId, string idempotencyKey)
    {
            return _locatiesPerVCode.Select(x => new HeradresseerLocatiesMessage(
                                   x.VCode,
                                   x.Locaties.Select(locatie => new TeHeradresserenLocatie(locatie.LocatieId, destinationAdresId.ToString()))
                                    .ToList(),
                                   idempotencyKey)).ToList().AsReadOnly();
    }
}
