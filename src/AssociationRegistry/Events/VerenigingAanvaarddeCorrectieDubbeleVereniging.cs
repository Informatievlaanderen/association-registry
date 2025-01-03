namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingAanvaarddeCorrectieDubbeleVereniging(string VCode, string VCodeDubbeleVereniging) : IEvent
{
    public static VerenigingAanvaarddeCorrectieDubbeleVereniging With(VCode vCode, VCode dubbeleVereniging)
        => new(vCode, dubbeleVereniging);
}
