namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using Constants;
using GrarSync;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.CodeGeneration;
using Json;
using Magda.Models;
using Marten;
using Marten.Events;
using Marten.Services;
using Metrics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Schema.Detail;
using Schema.Historiek;
using VCodeGeneration;
using Vereniging;
using Weasel.Core;
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

                                          opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
                                          opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();
                                          opts.RegisterDocumentType<LocatieLookupDocument>();
                                          opts.RegisterDocumentType<AddressKafkaConsumerOffset>();

                                          opts.Schema.For<LocatieLookupDocument>()
                                              .UseNumericRevisions(true)
                                              .UseOptimisticConcurrency(false);

                                          opts.RegisterDocumentType<VerenigingState>();

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
                                 .IntegrateWithWolverine()
                                 .UseLightweightSessions();

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

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
