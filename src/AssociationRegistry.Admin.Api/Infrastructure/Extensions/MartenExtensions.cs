namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Adapters.VCodeGeneration;
using Formats;
using GrarConsumer.Kafka;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using Json;
using Magda.Models;
using Marten;
using Marten.Events;
using Marten.Services;
using Metrics;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Schema.Detail;
using Schema.Historiek;
using Schema.PowerBiExport;
using Vereniging;
using Weasel.Core;
using Wolverine.Marten;
using IEvent = Events.IEvent;

public static class MartenExtensions
{
    private const string WolverineSchemaName = "public";

    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        IConfigurationRoot configuration,
        PostgreSqlOptionsSection postgreSqlOptions,
        bool isDevelopment)
    {
        var martenConfiguration = services
                                 .AddMarten(
                                      serviceProvider =>
                                      {
                                          var opts = new StoreOptions();

                                          if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
                                          {
                                              opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
                                              opts.DatabaseSchemaName = postgreSqlOptions.Schema;
                                          }

                                          opts.OpenTelemetry.TrackConnections = TrackLevel.Normal;
                                          opts.OpenTelemetry.TrackEventCounters();

                                          opts.Connection(postgreSqlOptions.GetConnectionString());
                                          opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));

                                          opts.UseNewtonsoftForSerialization(configure: settings =>
                                          {
                                              settings.DateParseHandling = DateParseHandling.None;
                                              settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                                              settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                                          });

                                          opts.Events.StreamIdentity = StreamIdentity.AsString;
                                          opts.Events.MetadataConfig.EnableAll();
                                          opts.Events.AppendMode = EventAppendMode.Quick;

                                          opts.Events.AddEventTypes(typeof(IEvent).Assembly
                                                                                  .GetTypes()
                                                                                  .Where(t => typeof(IEvent)
                                                                                                .IsAssignableFrom(t) && !t.IsAbstract &&
                                                                                             t.IsClass)
                                                                                  .ToList());

                                          opts.Listeners.Add(
                                              new HighWatermarkListener(serviceProvider.GetRequiredService<Instrumentation>()));

                                          opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
                                          opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();
                                          opts.RegisterDocumentType<PowerBiExportDocument>();
                                          opts.RegisterDocumentType<LocatieLookupDocument>();
                                          opts.RegisterDocumentType<LocatieZonderAdresMatchDocument>();
                                          opts.RegisterDocumentType<AddressKafkaConsumerOffset>();

                                          opts.Schema.For<LocatieLookupDocument>()
                                              .UseNumericRevisions(true)
                                              .UseOptimisticConcurrency(false);

                                          opts.Schema.For<LocatieZonderAdresMatchDocument>()
                                              .UseNumericRevisions(true)
                                              .UseOptimisticConcurrency(false);

                                          opts.Schema.For<PowerBiExportDocument>()
                                              .UseNumericRevisions(true)
                                              .UseOptimisticConcurrency(false);

                                          opts.RegisterDocumentType<VerenigingState>();

                                          opts.RegisterDocumentType<SettingOverride>();

                                          opts.Schema.For<MagdaCallReference>().Identity(x => x.Reference);

                                          if (serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment())
                                          {
                                              opts.GeneratedCodeMode = TypeLoadMode.Dynamic;
                                          }
                                          else
                                          {
                                              opts.GeneratedCodeMode = TypeLoadMode.Auto;
                                              opts.SourceCodeWritingEnabled = false;
                                          }

                                          opts.AutoCreateSchemaObjects = AutoCreate.All;

                                          return opts;
                                      })
                                 .IntegrateWithWolverine(integration =>
                                  {
                                      integration.TransportSchemaName = WolverineSchemaName;
                                      integration.MessageStorageSchemaName = WolverineSchemaName;
                                  })
                                 .UseLightweightSessions();

        if (configuration["ApplyAllDatabaseChangesDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        return services;
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username};";
}
