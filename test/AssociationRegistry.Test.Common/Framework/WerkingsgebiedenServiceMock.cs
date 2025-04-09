namespace AssociationRegistry.Test.Common.Framework;

using Vereniging;
using Vereniging.Exceptions;

public class WerkingsgebiedenServiceMock : IWerkingsgebiedenService
{
    public static readonly Werkingsgebied[] All =
    [
        Werkingsgebied.Hydrate("BE10", "Brussels Hoofdstedelijk Gewest"),
        Werkingsgebied.Hydrate("BE21", "Provincie Antwerpen"),
        Werkingsgebied.Hydrate("BE22", "Provincie Limburg"),
        Werkingsgebied.Hydrate("BE23", "Provincie Oost-Vlaanderen"),
        Werkingsgebied.Hydrate("BE24", "Provincie Vlaams-Brabant"),
        Werkingsgebied.Hydrate("BE25", "Provincie West-Vlaanderen"),
        Werkingsgebied.Hydrate("BE21111021", "Hove (Antwerpen)"),
        Werkingsgebied.Hydrate("BE21111022", "Kalmthout"),
        Werkingsgebied.Hydrate("BE21111023", "Kapellen (Antwerpen)"),
        Werkingsgebied.Hydrate("BE21111024", "Kontich"),
        Werkingsgebied.Hydrate("BE21111025", "Lint"),
        Werkingsgebied.Hydrate("BE21111029", "Mortsel"),
        Werkingsgebied.Hydrate("BE21111030", "Niel"),
        Werkingsgebied.Hydrate("BE21111035", "Ranst"),
        Werkingsgebied.Hydrate("BE21111037", "Rumst"),
        Werkingsgebied.Hydrate("BE21111038", "Schelle"),
        Werkingsgebied.Hydrate("BE21111039", "Schilde"),
        Werkingsgebied.Hydrate("BE21111040", "Schoten"),
        Werkingsgebied.Hydrate("BE21111044", "Stabroek"),
        Werkingsgebied.Hydrate("BE21111050", "Wijnegem"),
        Werkingsgebied.Hydrate("BE21111052", "Wommelgem"),
        Werkingsgebied.Hydrate("BE21111053", "Wuustwezel"),
        Werkingsgebied.Hydrate("BE25535002", "Bredene"),
    ];

    public static Werkingsgebied[] AllWithNvt => All.Append(Werkingsgebied.NietVanToepassing).ToArray();

    public Werkingsgebied Create(string code)
    {
        var match = AllWithNvt
           .SingleOrDefault(w => string.Equals(w.Code, code, StringComparison.InvariantCultureIgnoreCase));

        return match ?? throw new WerkingsgebiedCodeIsNietGekend(code);
    }

    public IReadOnlyList<Werkingsgebied> AllWithNVT() => All.Append(Werkingsgebied.NietVanToepassing).ToArray();
}
