namespace AssociationRegistry.Events;

using Framework;
using Vereniging;

public record DoelgroepWerdGewijzigd(Registratiedata.Doelgroep Doelgroep) : IEvent
{
    public static DoelgroepWerdGewijzigd With(Doelgroep doelgroep)
        => new(Registratiedata.Doelgroep.With(doelgroep));
}
