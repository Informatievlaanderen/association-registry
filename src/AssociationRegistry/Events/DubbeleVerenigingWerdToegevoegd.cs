namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record DubbeleVerenigingWerdToegevoegd(string VCode, string DubbeleVereniging) : IEvent
{
    public static DubbeleVerenigingWerdToegevoegd With(VCode vCode, VCode dubbeleVereniging)
        => new(vCode, dubbeleVereniging);
}
