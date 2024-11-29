namespace AssociationRegistry.Admin.Api.GrarConsumer.Finders;

using Grar.HeradresseerLocaties;
using Groupers;

public class TeHeradresserenLocatiesFinder : ITeHeradresserenLocatiesFinder
{
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesFinder(ILocatieFinder locatieFinder)
    {
        _locatieFinder = locatieFinder;
    }

    public async Task<TeHeradresserenLocatiesMessage[]> Find(int addressPersistentLocalId)
    {
        var locatiesMetVCodes = await _locatieFinder.FindLocaties(addressPersistentLocalId);

        return locatiesMetVCodes.Any()
            ? new LocatiesVolgensVCodeGrouper().Group(locatiesMetVCodes)
            : [];
    }
}
