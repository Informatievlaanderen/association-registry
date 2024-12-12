namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingWerdGemarkeerdAlsDubbelVan(string VCode, string VCodeAuthentiekeVereniging) : IEvent
{
    public static VerenigingWerdGemarkeerdAlsDubbelVan With(VCode vCode, VCode vCodeAuthentiekeVereniging)
        => new(vCode, vCodeAuthentiekeVereniging);
}
