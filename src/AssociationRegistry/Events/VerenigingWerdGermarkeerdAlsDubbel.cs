namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingWerdGermarkeerdAlsDubbel(string VCode, string IsDubbelVan) : IEvent
{
    public static VerenigingWerdGermarkeerdAlsDubbel With(VCode vCode, VCode isDubbelVan)
        => new(vCode, isDubbelVan);
}
