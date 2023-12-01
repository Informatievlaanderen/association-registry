namespace AssociationRegistry.EventStore;

using Events;
using Framework;

public class EventEncryptor
{
    public IEvent Downcast(IEvent @event)
        => @event switch
        {
            VertegenwoordigerWerdToegevoegd vertegenwoordigerWerdToegevoegd => new VertegenwoordigerWerdToegevoegdEncrypted(
                vertegenwoordigerWerdToegevoegd.VertegenwoordigerId, vertegenwoordigerWerdToegevoegd.Insz,
                vertegenwoordigerWerdToegevoegd.IsPrimair, vertegenwoordigerWerdToegevoegd.Roepnaam, vertegenwoordigerWerdToegevoegd.Rol,
                vertegenwoordigerWerdToegevoegd.Voornaam + "-whoeptidoe",
                vertegenwoordigerWerdToegevoegd.Achternaam,
                vertegenwoordigerWerdToegevoegd.Email,
                vertegenwoordigerWerdToegevoegd.Telefoon, vertegenwoordigerWerdToegevoegd.Mobiel,
                vertegenwoordigerWerdToegevoegd.SocialMedia),
            _ => @event,
        };
}
