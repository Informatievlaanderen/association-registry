namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record LidmaatschapWerdToegevoegd(Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{
    public static LidmaatschapWerdToegevoegd With(Lidmaatschap lidmaatschap)
        => new(
            Registratiedata.Lidmaatschap.With(lidmaatschap)
        );
}
