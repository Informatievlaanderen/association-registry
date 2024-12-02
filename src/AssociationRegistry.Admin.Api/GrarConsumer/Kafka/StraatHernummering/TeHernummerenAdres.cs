namespace AssociationRegistry.Admin.Api.GrarConsumer.Kafka.StraatHernummering;

using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Grar.GrarUpdates.Hernummering.Groupers;

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

