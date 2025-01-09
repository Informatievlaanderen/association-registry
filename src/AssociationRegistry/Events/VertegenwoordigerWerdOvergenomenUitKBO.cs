namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

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
