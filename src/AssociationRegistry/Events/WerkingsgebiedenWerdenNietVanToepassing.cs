namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record WerkingsgebiedenWerdenNietVanToepassing(string VCode) : IEvent
{
    public static WerkingsgebiedenWerdenNietVanToepassing With(VCode vCode) => new(vCode);
}
