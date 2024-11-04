namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record LidmaatschapWerdGewijzigd(string VCode, Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{
    public static LidmaatschapWerdGewijzigd With(VCode vCode, Lidmaatschap lidmaatschap)
        => new(
            vCode,
            Registratiedata.Lidmaatschap.With(lidmaatschap)
        );
}
