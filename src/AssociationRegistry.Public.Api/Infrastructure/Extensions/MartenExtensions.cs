namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Constants;
using Hosts.Configuration;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using Json;
using Marten;
using MartenDb.Logging;
using MartenDb.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostgreSqlOptionsSection = Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        PostgreSqlOptionsSection postgreSqlOptions,
        IConfiguration configuration)
    {
        var martenConfiguration = services.AddMarten(serviceProvider =>
                 {
                     var connectionString1 = GetPostgresConnectionString(postgreSqlOptions);

                     var opts = new StoreOptions();

                     opts.Connection(connectionString1);

                     if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
                     {
                         opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
                         opts.DatabaseSchemaName = postgreSqlOptions.Schema;
                     }

                     opts.SetUpOpenTelemetry();
                     opts.Logger(new SecureMartenLogger(serviceProvider.GetRequiredService<ILogger<SecureMartenLogger>>()));
                     opts.Events.StreamIdentity = StreamIdentity.AsString;

                     opts.Events.MetadataConfig.EnableAll();

                     opts.UseNewtonsoftForSerialization(configure: settings =>
                     {
                         settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                         settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
                     });

                     return opts;
                 }).UseLightweightSessions();


        if(FeatureFlags.IsTestingMode())
            martenConfiguration.ApplyAllDatabaseChangesOnStartup();
        else
            martenConfiguration.AssertDatabaseMatchesConfigurationOnStartup();

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;
            x.Development.ResourceAutoCreate =
                FeatureFlags.IsTestingMode()
                    ? AutoCreate.CreateOrUpdate
                    : AutoCreate.None;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate =
                FeatureFlags.IsTestingMode()
                    ? AutoCreate.CreateOrUpdate
                    : AutoCreate.None;
            x.Production.SourceCodeWritingEnabled = false;
        });

        return services;
    }

    private static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
        => $"host={postgreSqlOptions.Host};" +
           $"database={postgreSqlOptions.Database};" +
           $"password={postgreSqlOptions.Password};" +
           $"username={postgreSqlOptions.Username}";
}
