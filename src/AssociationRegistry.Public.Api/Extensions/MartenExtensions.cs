namespace AssociationRegistry.Public.Api.Extensions;

using ConfigurationBindings;
using Constants;
using Infrastructure.Json;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class MartenExtensions
{
    public static IServiceCollection AddMarten(this IServiceCollection services, PostgreSqlOptionsSection postgreSqlOptions, IConfiguration configuration)
    {
        var martenConfiguration = services.AddMarten(
            serviceProvider =>
            {
                var connectionString1 = GetPostgresConnectionString(postgreSqlOptions);

                var opts = new StoreOptions();

                opts.Connection(connectionString1);

                opts.Events.StreamIdentity = StreamIdentity.AsString;

                opts.Events.MetadataConfig.EnableAll();

                opts.Serializer(CreateCustomMartenSerializer());
                return opts;
            });
        if (configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);

        return services;
    }

    private static string GetPostgresConnectionString(PostgreSqlOptionsSection postgreSqlOptions)
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
                s.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                s.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });
        return jsonNetSerializer;
    }
}
