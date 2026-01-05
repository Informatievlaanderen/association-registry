namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

public record BankrekeningnummerWerdToegevoegd(
    int Id,
    string IBAN,
    string GebruiktVoor,
    string Titularis) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

