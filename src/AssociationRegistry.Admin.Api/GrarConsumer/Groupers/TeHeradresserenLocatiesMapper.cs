namespace AssociationRegistry.Admin.Api.GrarConsumer.Groupers;

using Grar.Models;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Finders;
using Grar.GrarUpdates;
using Grar.GrarUpdates.TeHeradresserenLocaties;
using Grar.LocatieFinder;

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
        var locations = await _locatieFinder.FindLocatieLookupDocuments(sourceAddressIds);


        var result = locations.GroupBy(v => v.VCode)
                              .Select(g => new TeHeradresserenLocatiesMessage(
                                          g.Key,
                                          g.Select(locatieIdWithVCode =>
                                                       new TeHeradresserenLocatie(locatieIdWithVCode.LocatieId,
                                                                                  GetDestinationAddressIdFromSourceAddressId(
                                                                                      locatieIdWithVCode.AdresId, readdressedAddressData)))
                                           .ToList(),
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
