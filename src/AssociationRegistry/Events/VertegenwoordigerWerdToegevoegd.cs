namespace AssociationRegistry.Events;


using System.Runtime.Serialization;
using Vereniging.Bronnen;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record VertegenwoordigerWerdToegevoegd(
    int VertegenwoordigerId,
    string Insz,
    bool IsPrimair,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

public record VertegenwoordigerWerdToegevoegdZonderPersoonsgegevens(
    Guid RefId,
    int VertegenwoordigerId,
    bool IsPrimair) : IEvent
{
    [IgnoreDataMember]
    public Bron Bron
        => Bron.Initiator;
}

