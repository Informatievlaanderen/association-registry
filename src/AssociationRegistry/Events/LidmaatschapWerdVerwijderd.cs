namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record LidmaatschapWerdVerwijderd(string VCode,
                                         Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{
    public static LidmaatschapWerdVerwijderd With(VCode vCode, Lidmaatschap lidmaatschap)
        => new(vCode, Registratiedata.Lidmaatschap.With(lidmaatschap));
}
