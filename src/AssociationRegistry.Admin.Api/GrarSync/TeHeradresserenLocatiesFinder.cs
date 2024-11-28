namespace AssociationRegistry.Admin.Api.GrarSync;

using Grar.HeradresseerLocaties;

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
