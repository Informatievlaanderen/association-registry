namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using ConfigurationBindings;
using Constants;
using JasperFx.CodeGeneration;
using Json;
using Magda.Models;
using Marten;
using Marten.Events;
using Marten.Services;
using Metrics;
using Newtonsoft.Json;
using Schema.Detail;
using Schema.Historiek;
using Schema.KboSync;
using VCodeGeneration;
using Vereniging;
using Wolverine.Marten;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions)
    {
        var martenConfiguration = services
                                 .AddMarten(
                                      serviceProvider =>
                                      {
                                          var opts = new StoreOptions();
                                          opts.Connection(postgreSqlOptions.GetConnectionString());
                                          opts.Events.StreamIdentity = StreamIdentity.AsString;
                                          opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));
                                          opts.Serializer(CreateCustomMartenSerializer());
                                          opts.Events.MetadataConfig.EnableAll();

                                          opts.Listeners.Add(
                                              new HighWatermarkListener(serviceProvider.GetRequiredService<Instrumentation>()));

                                          opts.RegisterDocumentType<VerenigingState>();

                                          opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
                                          opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();
                                          opts.RegisterDocumentType<BeheerKboSyncHistoriekGebeurtenisDocument>();
                                          opts.RegisterDocumentType<LocatieLookupDocument>();

                                          opts.Schema.For<LocatieLookupDocument>().Identity(document => document.Id)
                                              .UseOptimisticConcurrency(false)
                                              .UseNumericRevisions(true);
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

                                          return opts;
                                      })
                                 .IntegrateWithWolverine()
                                 .UseLightweightSessions()
                                 .ApplyAllDatabaseChangesOnStartup();

        return services;
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username};";

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
