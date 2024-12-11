namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingWerdGermarkeerdAlsDubbelVan(string VCode, string VCodeAuthentiekeVereniging) : IEvent
{
    public static VerenigingWerdGermarkeerdAlsDubbelVan With(VCode vCode, VCode vCodeAuthentiekeVereniging)
        => new(vCode, vCodeAuthentiekeVereniging);
}
