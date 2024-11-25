namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WerkingsgebiedenWerdenNietBepaald(string VCode) : IEvent
{
    public static WerkingsgebiedenWerdenNietBepaald With(VCode vCode) => new(vCode);
}
