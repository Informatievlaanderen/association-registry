namespace AssociationRegistry.Events;


using Vereniging;

public record VerenigingAanvaarddeDubbeleVereniging(string VCode, string VCodeDubbeleVereniging) : IEvent
{

}
