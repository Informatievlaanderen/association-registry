namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingAanvaardeDubbeleVereniging(string VCode, string VCodeDubbeleVereniging) : IEvent
{
    public static VerenigingAanvaardeDubbeleVereniging With(VCode vCode, VCode dubbeleVereniging)
        => new(vCode, dubbeleVereniging);
}


