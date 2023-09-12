namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;
using Vereniging;

public record ContactgegevenVolgensKBOWerdGewijzigd(
    int ContactgegevenId,
    string Beschrijving,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => AssociationRegistry.Vereniging.Bronnen.Bron.KBO;

    public static ContactgegevenVolgensKBOWerdGewijzigd With(Contactgegeven contactgegeven)
        => new(contactgegeven.ContactgegevenId, contactgegeven.Beschrijving, contactgegeven.IsPrimair);
}
