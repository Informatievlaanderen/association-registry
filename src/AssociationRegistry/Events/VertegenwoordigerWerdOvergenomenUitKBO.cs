namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record VertegenwoordigerWerdOvergenomenUitKBO(
    int VertegenwoordigerId,
    string Insz,
    string Voornaam,
    string Achternaam) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;
}

public record VertegenwoordigerWerdOvergenomenUitKBOZonderPersoonsgegevens(
    Guid RefId,
    int VertegenwoordigerId) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.KBO;
}
