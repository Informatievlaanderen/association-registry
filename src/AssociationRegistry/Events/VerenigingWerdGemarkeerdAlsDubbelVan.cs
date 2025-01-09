namespace AssociationRegistry.Events;


using Vereniging;

public record VerenigingWerdGemarkeerdAlsDubbelVan(string VCode, string VCodeAuthentiekeVereniging) : IEvent
{

}
