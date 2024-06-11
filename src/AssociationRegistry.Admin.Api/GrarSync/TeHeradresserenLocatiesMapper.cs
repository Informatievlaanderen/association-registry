namespace AssociationRegistry.Admin.Api.GrarSync;

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

        var teHeradresserenLocaties = locaties.Select(l => new TeHeradresserenLocatiesMessage(l.VCode, new List<(int, string)>
        {
            (l.LocatieId, l.AdresId)
        }));

        return teHeradresserenLocaties;
    }
}

public interface ILocatieFinder
{
    public Task<IEnumerable<LocatieLookupDocument>> FindLocaties(string adresId);
}

public record TeHeradresserenLocatiesMessage(string VCode, List<(int, string)> LocatiesMetAdres);
