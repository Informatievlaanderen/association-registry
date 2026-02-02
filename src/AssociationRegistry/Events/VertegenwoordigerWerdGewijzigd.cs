namespace AssociationRegistry.Events;

[Obsolete("These are the upcasted events, only use this in projections and State")]
public record VertegenwoordigerWerdGewijzigd(
    int VertegenwoordigerId,
    bool IsPrimair,
    string Roepnaam,
    string Rol,
    string Voornaam,
    string Achternaam,
    string Email,
    string Telefoon,
    string Mobiel,
    string SocialMedia
) : IEvent
{
    protected virtual bool PrintMembers(System.Text.StringBuilder builder)
    {
        builder.Append($"VertegenwoordigerId = {VertegenwoordigerId}, ");
        builder.Append($"IsPrimair = {IsPrimair}, ");
        return true;
    }
}

public record VertegenwoordigerWerdGewijzigdZonderPersoonsgegevens(Guid RefId, int VertegenwoordigerId, bool IsPrimair)
    : IEvent { }
