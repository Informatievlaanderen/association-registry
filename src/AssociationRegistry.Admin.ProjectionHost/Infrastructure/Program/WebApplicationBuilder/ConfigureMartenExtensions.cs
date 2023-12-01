namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure.Program.WebApplicationBuilder;

using ConfigurationBindings;
using Constants;
using Events;
using EventStore;
using JasperFx.CodeGeneration;
using Json;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Services;
using Marten.Services.Json.Transformations;
using Newtonsoft.Json;
using Projections.Detail;
using Projections.Historiek;
using Projections.Search;
using Schema.Detail;
using Schema.Historiek;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Wolverine;
using ConfigurationManager = ConfigurationManager;

public static class ConfigureMartenExtensions
{
    public static IServiceCollection ConfigureProjectionsWithMarten(
        this IServiceCollection source,
        ConfigurationManager configurationManager)
    {
        source
           .AddTransient<IElasticRepository, ElasticRepository>();

        var martenConfiguration = AddMarten(source, configurationManager);

        if (configurationManager["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);

        return source;
    }

    private static MartenServiceCollectionExtensions.MartenConfigurationExpression AddMarten(
        IServiceCollection services,
        ConfigurationManager configurationManager)
    {
        static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
            => $"host={postgreSqlOptions.Host};" +
               $"database={postgreSqlOptions.Database};" +
               $"password={postgreSqlOptions.Password};" +
               $"username={postgreSqlOptions.Username}";

        static JsonNetSerializer CreateCustomMartenSerializer()
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

        var martenConfigurationExpression = services.AddMarten(
            serviceProvider =>
            {
                var postgreSqlOptions = configurationManager.GetSection(PostgreSqlOptionsSection.Name)
                                                            .Get<PostgreSqlOptionsSection>() ??
                                        throw new ConfigurationErrorsException("Missing a valid postgres configuration");

                var connectionString = GetPostgresConnectionString(postgreSqlOptions);

                var opts = new StoreOptions();

                opts.Connection(connectionString);

                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Events.MetadataConfig.EnableAll();

                opts.Projections.OnException(_ => true).Stop();

                opts.Projections.Add<BeheerVerenigingHistoriekProjection>(ProjectionLifecycle.Async);
                opts.Projections.Add<BeheerVerenigingDetailProjection>(ProjectionLifecycle.Async);

                opts.Projections.Add(
                    new MartenSubscription(
                        new MartenEventsConsumer(
                            serviceProvider.GetRequiredService<IMessageBus>()
                        )
                    ),
                    ProjectionLifecycle.Async,
                    projectionName: "BeheerVerenigingZoekenDocument");

                opts.Serializer(CreateCustomMartenSerializer());

                opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
                opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();

                opts.Schema.For<EncryptionRecord>().Identity(x => x.EncryptionKey);

                opts.Events.Upcast(new DecryptionUpcaster(serviceProvider.GetRequiredService<IDocumentStore>, new EventEncryptor()));

                if (serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Dynamic;
                }
                else
                {
                    opts.GeneratedCodeMode = TypeLoadMode.Static;
                    opts.SourceCodeWritingEnabled = false;
                }

                return opts;
            });

        return martenConfigurationExpression;
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
