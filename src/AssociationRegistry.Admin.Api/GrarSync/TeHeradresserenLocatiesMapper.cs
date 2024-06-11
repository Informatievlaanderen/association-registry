namespace AssociationRegistry.Admin.Api.GrarSync;

using Be.Vlaanderen.Basisregisters.GrAr.Contracts.AddressRegistry;
using Schema.Detail;

public class TeHeradresserenLocatiesMapper
{
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesMapper(ILocatieFinder locatieFinder)
    {
        _locatieFinder = locatieFinder;
    }

    public async Task<IEnumerable<TeHeradresserenLocatiesMessage>> ForAddress(string adresId)
    {
        var locaties = await _locatieFinder.FindLocaties(adresId);

        if (!locaties.Any())
            return Array.Empty<TeHeradresserenLocatiesMessage>();

        var teHeradresserenLocaties = locaties
                                     .GroupBy(doc => doc.VCode)
                                     .Select(g => new TeHeradresserenLocatiesMessage(
                                                 g.Key,
                                                 g.Select(doc => (doc.LocatieId, doc.AdresId)).ToList()));

        return teHeradresserenLocaties;
    }

    public async Task<IEnumerable<TeHeradresserenLocatiesMessage>> ForAddress(IReadOnlyList<AddressHouseNumberReaddressedData> readdressedHouseNumbers)
    {
        return Array.Empty<TeHeradresserenLocatiesMessage>();
    }
}

public interface ILocatieFinder
{
    public Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string adresId);
}

public record TeHeradresserenLocatiesMessage(string VCode, List<(int, string)> LocatiesMetAdres);
