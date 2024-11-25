namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WerkingsgebiedenWerdenBepaald(string VCode, Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{
    public static WerkingsgebiedenWerdenBepaald With(
        VCode vCode,
        IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(vCode, werkingsgebieden
              .Select(Registratiedata.Werkingsgebied.With)
              .ToArray());
}



