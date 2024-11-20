namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WerkingsgebiedenWerdenBepaald(
    Registratiedata.Werkingsgebied[] Werkingsgebieden) : IEvent
{
    public static WerkingsgebiedenWerdenBepaald With(
        IEnumerable<Werkingsgebied> werkingsgebieden)
        => new(werkingsgebieden
              .Select(Registratiedata.Werkingsgebied.With)
              .ToArray());
}
