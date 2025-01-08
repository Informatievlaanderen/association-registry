namespace AssociationRegistry.Events;


using Vereniging;

public record VerenigingAanvaarddeCorrectieDubbeleVereniging(string VCode, string VCodeDubbeleVereniging) : IEvent
{

}
