namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WerkingsgebiedenWerdenGewijzigd(string VCode, Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{
    public static WerkingsgebiedenWerdenGewijzigd With(VCode vCode, IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(vCode, werkingsgebieden
              .Select(Registratiedata.Werkingsgebied.With)
              .ToArray());
}
