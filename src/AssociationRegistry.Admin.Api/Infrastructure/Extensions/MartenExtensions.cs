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
using System.Linq;
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

                opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);

                opts.Events.Upcast(new DecryptionUpcaster(serviceProvider.GetRequiredService<IDocumentStore>(), new EventEncryptor()));
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

public class DecryptionUpcaster : AsyncOnlyEventUpcaster<IEvent<VertegenwoordigerWerdToegevoegdEncrypted>, VertegenwoordigerWerdToegevoegd>
{
    private readonly IDocumentStore _store;
    private readonly EventEncryptor _encryptor;

    public DecryptionUpcaster(IDocumentStore store, EventEncryptor encryptor)
    {
        _store = store;
        _encryptor = encryptor;
    }

    protected override async Task<VertegenwoordigerWerdToegevoegd> UpcastAsync(
        IEvent<VertegenwoordigerWerdToegevoegdEncrypted> oldEvent,
        CancellationToken ct)
    {
        await using var session = _store.QuerySession();

        var (_, _, encryptionKey) = await session.Query<EncryptionRecord>()
                                                 .Where(x => x.VCode == oldEvent.StreamKey &&
                                                             x.VertegenwoordigerId == oldEvent.Data.VertegenwoordigerId)
                                                 .SingleAsync(token: ct);

        return new VertegenwoordigerWerdToegevoegd(
            oldEvent.Data.VertegenwoordigerId, oldEvent.Data.Insz,
            oldEvent.Data.IsPrimair, oldEvent.Data.Roepnaam, oldEvent.Data.Rol,
            oldEvent.Data.Voornaam.Replace(encryptionKey, newValue: ""),
            oldEvent.Data.Achternaam,
            oldEvent.Data.Email,
            oldEvent.Data.Telefoon, oldEvent.Data.Mobiel,
            oldEvent.Data.SocialMedia);
    }
}

