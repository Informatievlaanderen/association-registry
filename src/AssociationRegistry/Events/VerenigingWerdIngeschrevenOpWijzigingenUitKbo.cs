namespace AssociationRegistry.Events;

using Framework;

public record VerenigingWerdIngeschrevenOpWijzigingenUitKbo(
    string KboNummer) : IEvent
{
}
