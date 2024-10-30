namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record LidmaatschapWerdToegevoegd(Registratiedata.Lidmaatschap Lidmaatschap) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;

    public static LidmaatschapWerdToegevoegd With(Lidmaatschap lidmaatschap)
        => new(
            Registratiedata.Lidmaatschap.With(lidmaatschap)
        );
}
