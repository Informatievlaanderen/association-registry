namespace AssociationRegistry.Public.Api.Infrastructure.Extensions;

using Constants;
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
        services.AddMarten(
            _ =>
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

                opts.Serializer(CreateCustomMartenSerializer());

                return opts;
            });

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
