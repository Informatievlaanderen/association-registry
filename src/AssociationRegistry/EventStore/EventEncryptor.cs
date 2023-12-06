namespace AssociationRegistry.EventStore;

using Events;
using Formatters;
using Framework;
using Marten;
using Marten.Schema;
using System.Security.Cryptography;
using System.Text;

public class EventEncryptor
{
    public async Task<IEvent> Downcast(IEvent @event, IDocumentSession session, string vCode)
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

            case LocatieWerdToegevoegd locatieWerdToegevoegd:
                var adresString = locatieWerdToegevoegd.Locatie.Adres?.ToAdresString() ?? locatieWerdToegevoegd.Locatie.AdresId.ToString();

                var locatieEncryptionRecord = await session.Query<LocatieEncryptionRecord>()
                                                           .Where(x => x.Locatie == adresString)
                                                           .SingleOrDefaultAsync();

                if (locatieEncryptionRecord == null)
                {
                    locatieEncryptionRecord = new LocatieEncryptionRecord(Guid.NewGuid(),
                                                                          Guid.NewGuid().ToString().Replace(oldValue: "-", newValue: ""),
                                                                          adresString);

                    session.Insert(locatieEncryptionRecord);
                }

                return new LocatieWerdToegevoegdEncrypted(
                    locatieWerdToegevoegd.Locatie with
                    {
                        Naam = EncryptString(locatieWerdToegevoegd.Locatie.Naam, locatieEncryptionRecord.EncryptionKey),
                        Adres = locatieWerdToegevoegd.Locatie.Adres is null
                            ? null
                            : locatieWerdToegevoegd.Locatie.Adres with
                            {
                                Straatnaam = EncryptString(locatieWerdToegevoegd.Locatie.Adres.Straatnaam,
                                                           locatieEncryptionRecord.EncryptionKey),
                            },
                    },
                    locatieEncryptionRecord.EncryptionKeyId);

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

public record EncryptionRecord(string VCode, int VertegenwoordigerId, [property: Identity] string EncryptionKey);
public record LocatieEncryptionRecord([property: Identity] Guid EncryptionKeyId, string EncryptionKey, string Locatie);
