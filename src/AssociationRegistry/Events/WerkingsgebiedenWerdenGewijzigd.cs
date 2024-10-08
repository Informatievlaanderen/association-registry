namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WerkingsgebiedenWerdenGewijzigd(
    Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{
    public static WerkingsgebiedenWerdenGewijzigd With(
        IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(werkingsgebieden
              .Select(Registratiedata.Werkingsgebied.With)
              .ToArray());
}
