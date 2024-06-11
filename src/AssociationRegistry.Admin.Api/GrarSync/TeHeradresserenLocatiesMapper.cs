namespace AssociationRegistry.Admin.Api.GrarSync;

using Acties.HeradresseerLocaties;
using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;

public class TeHeradresserenLocatiesMapper
{
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesMapper(ILocatieFinder locatieFinder)
    {
        _locatieFinder = locatieFinder;
    }

    public async Task<IEnumerable<TeHeradresserenLocatiesMessage>> ForAddress(string adresId, string idempotencyKey)
    {
        var locaties = await _locatieFinder.FindLocaties(adresId);

        if (!locaties.Any())
            return Array.Empty<TeHeradresserenLocatiesMessage>();

        var teHeradresserenLocaties = locaties
                                     .GroupBy(doc => doc.VCode)
                                     .Select(g => new TeHeradresserenLocatiesMessage(
                                                 g.Key,
                                                 g.Select(doc => (doc.LocatieId, doc.AdresId)).ToList(),
                                                 idempotencyKey));

        return teHeradresserenLocaties;
    }

    public async Task<IEnumerable<TeHeradresserenLocatiesMessage>> ForAddress(IReadOnlyList<AddressHouseNumberReaddressedData> readdressedHouseNumbers)
    {
        return Array.Empty<TeHeradresserenLocatiesMessage>();
    }
}
