namespace AssociationRegistry.Admin.Api.GrarSync;

using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Grar.HeradresseerLocaties;
using Grar.Models;

public class TeHeradresserenLocatiesMapper
{
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesMapper(ILocatieFinder locatieFinder)
    {
        _locatieFinder = locatieFinder;
    }

    public async Task<IEnumerable<TeHeradresserenLocatiesMessage>> ForAddress(
        string sourceAdresId,
        string destinationAdresId,
        string idempotencyKey)
    {
        var locaties = await _locatieFinder.FindLocaties(sourceAdresId);

        if (!locaties.Any())
            return Array.Empty<TeHeradresserenLocatiesMessage>();

        var teHeradresserenLocaties = locaties
                                     .GroupBy(doc => doc.VCode)
                                     .Select(g => new TeHeradresserenLocatiesMessage(
                                                 g.Key,
                                                 g.Select(doc => new LocatieIdWithAdresId(doc.LocatieId, destinationAdresId)).ToList(),
                                                 idempotencyKey));

        return teHeradresserenLocaties;
    }

    public async Task<IEnumerable<TeHeradresserenLocatiesMessage>> ForAddress(
        IReadOnlyList<AddressHouseNumberReaddressedData> readdressedHouseNumbers,
        string idempotenceKey)
    {
        var sourceAndDestinationIds = new List<(int, int)>();

        foreach (var readdressedHouseNumber in readdressedHouseNumbers)
        {
            foreach (var readdressedBoxNumber in readdressedHouseNumber.ReaddressedBoxNumbers)
            {
                sourceAndDestinationIds.Add((readdressedBoxNumber.SourceAddressPersistentLocalId,
                                             readdressedBoxNumber.DestinationAddressPersistentLocalId));
            }

            sourceAndDestinationIds.Add((readdressedHouseNumber.ReaddressedHouseNumber.SourceAddressPersistentLocalId,
                                         readdressedHouseNumber.ReaddressedHouseNumber.DestinationAddressPersistentLocalId));
        }

        var teHeradresserenLocatiesMessages = new List<TeHeradresserenLocatiesMessage>();

        foreach (var (sourceAdresId, destinationAdresId) in sourceAndDestinationIds)
        {
            teHeradresserenLocatiesMessages.AddRange(await ForAddress(sourceAdresId.ToString(), destinationAdresId.ToString(),
                                                                      idempotenceKey));
        }

        return teHeradresserenLocatiesMessages.GroupBy(gb => gb.VCode)
                                                                         .SelectMany(g => g).ToList();
    }
}
