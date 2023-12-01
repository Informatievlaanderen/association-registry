namespace AssociationRegistry.EventStore;

using Events;
using Framework;
using Marten;
using System.Security.Cryptography;
using System.Text;

public class EventEncryptor
{
    public IEvent Downcast(IEvent @event, IDocumentSession session, string vCode)
    {
        switch (@event)
        {
            case VertegenwoordigerWerdToegevoegd vertegenwoordigerWerdToegevoegd:

                var encryptionKey = Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: "");
                session.Insert(new EncryptionRecord(vCode, vertegenwoordigerWerdToegevoegd.VertegenwoordigerId, encryptionKey));

                return new VertegenwoordigerWerdToegevoegdEncrypted(
                    vCode, vertegenwoordigerWerdToegevoegd.VertegenwoordigerId,
                    vertegenwoordigerWerdToegevoegd.Insz,
                    vertegenwoordigerWerdToegevoegd.IsPrimair,
                    vertegenwoordigerWerdToegevoegd.Roepnaam,
                    vertegenwoordigerWerdToegevoegd.Rol,
                    EncryptString(vertegenwoordigerWerdToegevoegd.Voornaam, encryptionKey),
                    EncryptString(vertegenwoordigerWerdToegevoegd.Achternaam, encryptionKey),
                    vertegenwoordigerWerdToegevoegd.Email,
                    vertegenwoordigerWerdToegevoegd.Telefoon,
                    vertegenwoordigerWerdToegevoegd.Mobiel,
                    vertegenwoordigerWerdToegevoegd.SocialMedia);

            default:
                return @event;
        }
    }

    public static string EncryptString(string plainText, string key)
    {
        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16]; // Initialization vector (IV)

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
}

public record EncryptionRecord(string VCode, int VertegenwoordigerId, string EncryptionKey);
