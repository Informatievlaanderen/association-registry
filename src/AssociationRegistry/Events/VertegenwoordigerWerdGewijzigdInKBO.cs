namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("These are the upcasted events, you might be looking for <EventName>+ZonderPersoonsgegevens")]
public record VertegenwoordigerWerdGewijzigdInKBO(
    int VertegenwoordigerId,
    string Insz,
    string Voornaam,
    string Achternaam) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;
}

public record VertegenwoordigerWerdGewijzigdInKBOZonderPersoonsgegevens(
    Guid RefId,
    int VertegenwoordigerId) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;
}
