namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record LidmaatschapWerdToegevoegd(string VCode, Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{
    public static LidmaatschapWerdToegevoegd With(VCode vCode ,Lidmaatschap lidmaatschap)
        => new(
            vCode,
            Registratiedata.Lidmaatschap.With(lidmaatschap)
        );
}
