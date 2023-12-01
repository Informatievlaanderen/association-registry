namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using AssociationRegistry.Magda.Models;
using ConfigurationBindings;
using Constants;
using Events;
using EventStore;
using JasperFx.CodeGeneration;
using Json;
using Marten;
using Marten.Events;
using Marten.Services;
using Marten.Services.Json.Transformations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Schema.Detail;
using Schema.Historiek;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VCodeGeneration;
using Vereniging;
using Weasel.Core;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions,
        IConfiguration configuration)
    {
        var martenConfiguration = services.AddMarten(
            serviceProvider =>
            {
                var opts = new StoreOptions();
                opts.Connection(postgreSqlOptions.GetConnectionString());
                opts.Events.StreamIdentity = StreamIdentity.AsString;
                opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));
                opts.Serializer(CreateCustomMartenSerializer());
                opts.Events.MetadataConfig.EnableAll();

                opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
                opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();

                opts.RegisterDocumentType<VerenigingState>();

                opts.Events.AddEventType(typeof(VertegenwoordigerWerdToegevoegdEncrypted));

                opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);

                opts.Schema.For<EncryptionRecord>().Identity(x => x.EncryptionKey);

                opts.Events.Upcast(new DecryptionUpcaster(store: () => serviceProvider.GetRequiredService<IDocumentStore>(),
                                                          new EventEncryptor()));

                if (serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Dynamic;

                    // serviceProvider.GetRequiredService<IDatabaseInitializer>()
                    //                .InitializeDatabase(opts);
                }
                else
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Auto;
                    opts.SourceCodeWritingEnabled = false;
                }

                opts.AutoCreateSchemaObjects = AutoCreate.All;

                return opts;
            });

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        return services;
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";

    public static JsonNetSerializer CreateCustomMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Customize(
            s =>
            {
                s.DateParseHandling = DateParseHandling.None;
                s.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                s.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });

        return jsonNetSerializer;
    }
}

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
