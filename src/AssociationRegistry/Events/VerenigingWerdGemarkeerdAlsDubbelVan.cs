namespace AssociationRegistry.Events;

public record VerenigingWerdGemarkeerdAlsDubbelVan(string VCode, string VCodeAuthentiekeVereniging) : IEvent
{

}
