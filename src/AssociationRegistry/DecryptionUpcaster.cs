namespace AssociationRegistry;

using Events;
using EventStore;
using Marten;
using Marten.Services.Json.Transformations;
using System.Security.Cryptography;
using System.Text;

public class DecryptionUpcaster : AsyncOnlyEventUpcaster<VertegenwoordigerWerdToegevoegdEncrypted, VertegenwoordigerWerdToegevoegd>
{
    private readonly Func<IDocumentStore> _store;
    private readonly EventEncryptor _encryptor;

    public DecryptionUpcaster(Func<IDocumentStore> store, EventEncryptor encryptor)
    {
        _store = store;
        _encryptor = encryptor;
    }

    protected override async Task<VertegenwoordigerWerdToegevoegd> UpcastAsync(
        VertegenwoordigerWerdToegevoegdEncrypted oldEvent,
        CancellationToken ct)
    {
        await using var session = _store().QuerySession();

        var x = await session.Query<EncryptionRecord>()
                             .Where(x => x.VCode == oldEvent.VCode &&
                                         x.VertegenwoordigerId == oldEvent.VertegenwoordigerId)
                             .SingleOrDefaultAsync(token: ct);

        return new VertegenwoordigerWerdToegevoegd(
            oldEvent.VCode,
            oldEvent.VertegenwoordigerId, oldEvent.Insz,
            oldEvent.IsPrimair, oldEvent.Roepnaam, oldEvent.Rol,
            DecryptString(oldEvent.Voornaam, x?.EncryptionKey),
            DecryptString(oldEvent.Achternaam, x?.EncryptionKey),
            oldEvent.Email,
            oldEvent.Telefoon, oldEvent.Mobiel,
            oldEvent.SocialMedia);
    }

    public static string DecryptString(string cipherText, string? key)
    {
        if (key is null)
            return "<Anoniem>";

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16]; // Initialization vector (IV) - should be the same as used in encryption

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}

public class LocatieDecryptionUpcaster : AsyncOnlyEventUpcaster<LocatieWerdToegevoegdEncrypted, LocatieWerdToegevoegd>
{
    private readonly Func<IDocumentStore> _store;
    private readonly EventEncryptor _encryptor;

    public LocatieDecryptionUpcaster(Func<IDocumentStore> store, EventEncryptor encryptor)
    {
        _store = store;
        _encryptor = encryptor;
    }

    protected override async Task<LocatieWerdToegevoegd> UpcastAsync(
        LocatieWerdToegevoegdEncrypted oldEvent,
        CancellationToken ct)
    {
        await using var session = _store().QuerySession();

        var x = await session.Query<LocatieEncryptionRecord>()
                             .Where(x => x.EncryptionKeyId == oldEvent.EncryptionKeyId)
                             .SingleOrDefaultAsync(token: ct);

        return new LocatieWerdToegevoegd(oldEvent.Locatie with
        {
            Naam = DecryptString(oldEvent.Locatie.Naam, x.EncryptionKey),
            Adres = oldEvent.Locatie.Adres is null
                ? null
                : oldEvent.Locatie.Adres with
                {
                    Straatnaam = DecryptString(oldEvent.Locatie.Adres.Straatnaam, x.EncryptionKey),
                },
        });
    }

    public static string DecryptString(string cipherText, string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return "<Anoniem>";

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = new byte[16]; // Initialization vector (IV) - should be the same as used in encryption

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (var srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
