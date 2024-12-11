namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record VerenigingWerdToegevoegdAlsDubbel(string VCode, string VCodeDubbeleVereniging) : IEvent
{
    public static VerenigingWerdToegevoegdAlsDubbel With(VCode vCode, VCode dubbeleVereniging)
        => new(vCode, dubbeleVereniging);
}
