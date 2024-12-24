namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record AanvaardingDubbeleVerenigingWerdGecorrigeerd(string VCode, string VCodeDubbeleVereniging) : IEvent
{
    public static AanvaardingDubbeleVerenigingWerdGecorrigeerd With(VCode vCode, VCode vCodeDubbeleVereniging)
        => new(vCode, vCodeDubbeleVereniging);
}


