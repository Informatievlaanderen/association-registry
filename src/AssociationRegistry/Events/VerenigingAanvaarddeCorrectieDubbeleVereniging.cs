namespace AssociationRegistry.Events;

public record VerenigingAanvaarddeCorrectieDubbeleVereniging(string VCode, string VCodeDubbeleVereniging) : IEvent
{

}
