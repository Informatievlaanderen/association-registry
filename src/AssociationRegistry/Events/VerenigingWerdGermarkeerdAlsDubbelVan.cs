namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingWerdGermarkeerdAlsDubbelVan(string VCode, string IsDubbelVan) : IEvent
{
    public static VerenigingWerdGermarkeerdAlsDubbelVan With(VCode vCode, VCode isDubbelVan)
        => new(vCode, isDubbelVan);
}
