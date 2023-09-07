namespace AssociationRegistry.Events;

using Framework;
using System.Runtime.Serialization;

public record VertegenwoordigerWerdOvergenomenUitKBO(
    int VertegenwoordigerId,
    string Insz,
    string Voornaam,
    string Achternaam) : IEvent
{
    [IgnoreDataMember]
    public string Bron
        => Vereniging.Bronnen.Bron.KBO;
}
