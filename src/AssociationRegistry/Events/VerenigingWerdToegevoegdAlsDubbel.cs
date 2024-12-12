namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingAanvaarddeDubbeleVereniging(string VCode, string VCodeDubbeleVereniging) : IEvent
{
    public static VerenigingAanvaarddeDubbeleVereniging With(VCode vCode, VCode dubbeleVereniging)
        => new(vCode, dubbeleVereniging);
}


