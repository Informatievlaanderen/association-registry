namespace AssociationRegistry.EventStore;

using Events;
using Framework;
using Marten;

public class EventEncryptor
{
    public IEvent Downcast(IEvent @event, IDocumentSession session, string vCode)
    {
        switch (@event)
        {
            case VertegenwoordigerWerdToegevoegd vertegenwoordigerWerdToegevoegd:

                var encryptionKey = Guid.NewGuid().ToString();
                session.Insert(new EncryptionRecord(vCode, vertegenwoordigerWerdToegevoegd.VertegenwoordigerId, encryptionKey));

                return new VertegenwoordigerWerdToegevoegdEncrypted(vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                                                                    vertegenwoordigerWerdToegevoegd.Insz,
                                                                    vertegenwoordigerWerdToegevoegd.IsPrimair,
                                                                    vertegenwoordigerWerdToegevoegd.Roepnaam,
                                                                    vertegenwoordigerWerdToegevoegd.Rol,
                                                                    vertegenwoordigerWerdToegevoegd.Voornaam + encryptionKey,
                                                                    vertegenwoordigerWerdToegevoegd.Achternaam,
                                                                    vertegenwoordigerWerdToegevoegd.Email,
                                                                    vertegenwoordigerWerdToegevoegd.Telefoon,
                                                                    vertegenwoordigerWerdToegevoegd.Mobiel,
                                                                    vertegenwoordigerWerdToegevoegd.SocialMedia);

            default:
                return @event;
        }
    }
}

public record EncryptionRecord(string VCode, int VertegenwoordigerId, string EncryptionKey);
