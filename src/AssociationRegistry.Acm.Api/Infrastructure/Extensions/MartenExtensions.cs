namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using Constants;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using JasperFx.Events.Daemon;
using Json;
using Marten;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Schema.VerenigingenPerInsz;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions,
        IConfiguration configuration)
    {
        var martenConfiguration = services
                                 .AddSingleton(postgreSqlOptions)
                                 .AddMarten(
                                      serviceProvider =>
                                      {
                                          var opts = new StoreOptions();
                                          ConfigureStoreOptions(opts, postgreSqlOptions, serviceProvider.GetRequiredService<IHostEnvironment>().IsDevelopment());
                                          return opts;
                                      });

        if (configuration["ApplyAllDatabaseChangesDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        if (configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.HotCold);

        return services;
    }

    public static void ConfigureStoreOptions(StoreOptions opts, PostgreSqlOptionsSection postgreSqlOptions, bool isDevelopment)
    {
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
    }

    public static string GetConnectionString(this PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";
}
