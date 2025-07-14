namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Constants;
using JasperFx;
using JasperFx.CodeGeneration;
using JasperFx.Events;
using Json;
using Marten;
using Marten.Events;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostgreSqlOptionsSection = Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(
        this IServiceCollection services,
        Hosts.Configuration.ConfigurationBindings.PostgreSqlOptionsSection postgreSqlOptions,
        IConfiguration configuration)
    {
        services.AddMarten(_ =>
        {
            var connectionString1 = GetPostgresConnectionString(postgreSqlOptions);

            var opts = new StoreOptions();

            opts.Connection(connectionString1);

            if (!string.IsNullOrEmpty(postgreSqlOptions.Schema))
            {
                opts.Events.DatabaseSchemaName = postgreSqlOptions.Schema;
                opts.DatabaseSchemaName = postgreSqlOptions.Schema;
            }

            opts.Events.StreamIdentity = StreamIdentity.AsString;

            opts.Events.MetadataConfig.EnableAll();

            opts.UseNewtonsoftForSerialization(configure: settings =>
            {
                settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });

            return opts;
        }).UseLightweightSessions();

        services.CritterStackDefaults(x =>
        {
            x.Development.GeneratedCodeMode = TypeLoadMode.Dynamic;

            x.Production.GeneratedCodeMode = TypeLoadMode.Static;
            x.Production.ResourceAutoCreate = AutoCreate.None;
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
