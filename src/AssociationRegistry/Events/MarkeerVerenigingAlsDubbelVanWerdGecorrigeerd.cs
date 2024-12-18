namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record MarkeerVerenigingAlsDubbelVanWerdGecorrigeerd(string VCode) : IEvent
{
    public static MarkeerVerenigingAlsDubbelVanWerdGecorrigeerd With(VCode vCode)
        => new(vCode);
}


