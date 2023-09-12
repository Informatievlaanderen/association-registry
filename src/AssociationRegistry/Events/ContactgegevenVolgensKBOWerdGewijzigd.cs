namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;
using Vereniging.Bronnen;

public record ContactgegevenVolgensKBOWerdGewijzigd(
    int ContactgegevenId,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;

    public static ContactgegevenVolgensKBOWerdGewijzigd With(Contactgegeven contactgegeven)
        => new(contactgegeven.ContactgegevenId, contactgegeven.Beschrijving, contactgegeven.IsPrimair);
}
