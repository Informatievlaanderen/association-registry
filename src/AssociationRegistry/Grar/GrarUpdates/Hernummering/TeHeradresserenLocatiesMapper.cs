namespace AssociationRegistry.Grar.GrarUpdates.Hernummering;

using AssociationRegistry.Acties.HeradresseerLocaties;
using AssociationRegistry.Grar.GrarUpdates.LocatieFinder;

public class TeHeradresserenLocatiesMapper
{
    private readonly ILocatieFinder _locatieFinder;

    public TeHeradresserenLocatiesMapper(ILocatieFinder locatieFinder)
    {
        _locatieFinder = locatieFinder;
    }

    public async Task<IEnumerable<HeradresseerLocatiesMessage>> ForAddress(
        TeHernummerenStraat teHernummerenStraat,
        string idempotenceKey)
    {
        var sourceAddressIds = teHernummerenStraat.Select(s => s.VanAdresId.ToString()).ToArray();
        var locatiesPerVCode = await _locatieFinder.FindLocaties(sourceAddressIds);

        return locatiesPerVCode.Select(x => new HeradresseerLocatiesMessage(x.VCode, x.Locaties.Select(locatie =>
                                                                                new TeHeradresserenLocatie(locatie.LocatieId,
                                                                                    teHernummerenStraat.NaarAdresIdVoor(locatie.AdresId))).ToList(), idempotenceKey));
    }
}
