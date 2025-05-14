namespace AssociationRegistry.Events;

public record VerenigingAanvaarddeDubbeleVereniging(string VCode, string VCodeDubbeleVereniging) : IEvent
{

}
