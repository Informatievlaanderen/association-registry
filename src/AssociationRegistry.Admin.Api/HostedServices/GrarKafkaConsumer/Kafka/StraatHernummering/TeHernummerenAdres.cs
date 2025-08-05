namespace AssociationRegistry.Admin.Api.HostedServices.GrarKafkaConsumer.Kafka.StraatHernummering;

using AssociationRegistry.Grar.GrarUpdates.Hernummering;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;

public static class TeHernummerenStraatFactory
{
    public static TeHernummerenStraat From(StreetNameWasReaddressed streetNameWasReaddressed)
    {
        var result = new List<TeHernummerenAdres>();

        foreach (var readdressed in streetNameWasReaddressed.ReaddressedHouseNumbers)
        {
            result.AddRange(readdressed.ReaddressedBoxNumbers.Select(From));
            result.Add(From(readdressed.ReaddressedHouseNumber));
        }

        return new(result);
    }

    private static TeHernummerenAdres From(ReaddressedAddressData readdressedAddressData)
        => new(
            readdressedAddressData.SourceAddressPersistentLocalId,
            readdressedAddressData.DestinationAddressPersistentLocalId);
}

