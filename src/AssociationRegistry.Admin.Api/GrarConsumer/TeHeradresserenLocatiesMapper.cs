namespace AssociationRegistry.Admin.Api.GrarConsumer;

using Grar.HeradresseerLocaties;
using Grar.Models;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;

public class TeHeradresserenLocatiesMapper
{
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesMapper(ILocatieFinder locatieFinder)
    {
        _locatieFinder = locatieFinder;
    }

    public async Task<IEnumerable<TeHeradresserenLocatiesMessage>> ForAddress(
        IReadOnlyList<AddressHouseNumberReaddressedData> readdressedHouseNumbers,
        string idempotenceKey)
    {
        var readdressedAddressData = GetReaddressedAddressData(readdressedHouseNumbers);
        var sourceAddressIds = readdressedAddressData.Select(s => s.SourceAddressPersistentLocalId.ToString()).ToArray();
        var locations = await _locatieFinder.FindLocaties(sourceAddressIds);

        var grouping = locations.GroupBy(l => l.VCode);

        var result = grouping.Select(g => new TeHeradresserenLocatiesMessage(
                                         g.Key,
                                         g.Select(s =>
                                                      new LocatieIdWithAdresId(s.LocatieId,
                                                                               GetDestinationAddressIdFromSourceAddressId(
                                                                                   s.AdresId, readdressedAddressData))).ToList(),
                                         idempotenceKey));



        return result;
    }

    private string GetDestinationAddressIdFromSourceAddressId(string sourceAddressId, IReadOnlyList<ReaddressedAddressData> data)
        => data.Single(s => s.SourceAddressPersistentLocalId.ToString() == sourceAddressId).DestinationAddressPersistentLocalId.ToString();

    private IReadOnlyList<ReaddressedAddressData> GetReaddressedAddressData(IReadOnlyList<AddressHouseNumberReaddressedData> data)
    {
        var result = new List<ReaddressedAddressData>();

        foreach (var readdressed in data)
        {
            result.AddRange(readdressed.ReaddressedBoxNumbers);
            result.Add(readdressed.ReaddressedHouseNumber);
        }

        return result;
    }
}
