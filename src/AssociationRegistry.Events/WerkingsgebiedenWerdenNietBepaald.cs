namespace AssociationRegistry.Events;


using Vereniging;

public record WerkingsgebiedenWerdenNietBepaald(string VCode) : IEvent
{
}
