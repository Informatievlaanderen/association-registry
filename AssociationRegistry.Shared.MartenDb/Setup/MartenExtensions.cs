namespace AssociationRegistry.Shared.MartenDb.Setup;

using Formats;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serialization.JsonConverters;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions,
        MartenOptionsSection martenOptions,
        IConfiguration configuration)
    {
        var martenConfiguration = services
                                 .AddSingleton(postgreSqlOptions)
                                 .AddMarten(
                                      serviceProvider =>
                                      {
                                          var opts = new StoreOptions();
                                          bool isDevelopment = serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment();
                                          opts.Connection(postgreSqlOptions.GetConnectionString());

                                          if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
                                          {
                                              opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
                                              opts.DatabaseSchemaName = postgreSqlOptions.Schema;
                                          }

                                          opts.Events.StreamIdentity = StreamIdentity.AsString;
                                          opts.UseNewtonsoftForSerialization(configure: settings =>
                                          {
                                              settings.DateParseHandling = DateParseHandling.None;
                                              settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                                              settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                                          });
                                          opts.Events.MetadataConfig.EnableAll();
                                          opts.AddPostgresProjections();

                                          opts.Projections.DaemonLockId = 2;

                                          opts.RegisterDocumentType<VerenigingenPerInszDocument>();
                                          opts.RegisterDocumentType<VerenigingDocument>();

                                          if (isDevelopment)
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

        if (martenOptions.ApplyAllDatabaseChangesEnabled)
            martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        if (martenOptions.ProjectionDaemonEnabled)
            martenConfiguration.AddAsyncDaemon(DaemonMode.HotCold);

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate = AutoCreate.None;
            x.Production.SourceCodeWritingEnabled = false;
        });

        return services;
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";
}

public record MartenOptionsSection
{
    public MartenOptionsSection(IConfiguration configuration)
    {
        ApplyAllDatabaseChangesEnabled = configuration["ApplyAllDatabaseChangesDisabled"]?.ToLowerInvariant() != "true";
        ProjectionDaemonEnabled = configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true";
    }

    public bool ProjectionDaemonEnabled { get; set; }
    public bool ApplyAllDatabaseChangesEnabled { get; set; }
}

