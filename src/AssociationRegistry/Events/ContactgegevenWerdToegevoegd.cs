namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record ContactgegevenWerdToegevoegd(
    int ContactgegevenId,
    string Type,
    string Waarde,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;

    public static ContactgegevenWerdToegevoegd With(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Contactgegeventype,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);
}
