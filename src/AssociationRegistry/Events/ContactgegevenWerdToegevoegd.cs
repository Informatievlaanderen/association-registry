namespace AssociationRegistry.Events;

using System.Runtime.Serialization;
using Framework;
using Vereniging;

public record ContactgegevenWerdToegevoegd(
    int ContactgegevenId,
    string Type,
    string Waarde,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => AssociationRegistry.Vereniging.Bronnen.Bron.Initiator;
    public static ContactgegevenWerdToegevoegd With(Contactgegeven contactgegeven)
        => new(
            contactgegeven.ContactgegevenId,
            contactgegeven.Type,
            contactgegeven.Waarde,
            contactgegeven.Beschrijving,
            contactgegeven.IsPrimair);
}
