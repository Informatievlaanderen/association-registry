namespace AssociationRegistry.Acm.Api.Infrastructure.Extensions;

using ConfigurationBindings;
using Constants;
using Json;
using Marten;
using Marten.Events;
using Marten.Events.Daemon.Resiliency;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public static class MartenExtentions
{
    public static IServiceCollection AddMarten(this IServiceCollection services, PostgreSqlOptionsSection postgreSqlOptions, IConfiguration configuration)
    {
        var martenConfiguration = services
            .AddSingleton(postgreSqlOptions)
            .AddMarten(
                opts =>
                {
                    opts.Connection(postgreSqlOptions.GetConnectionString());
                    opts.Events.StreamIdentity = StreamIdentity.AsString;
                    opts.Serializer(CreateCustomMartenSerializer());
                    opts.Events.MetadataConfig.EnableAll();
                    opts.AddPostgresProjections();
                });

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

        if (configuration["ProjectionDaemonDisabled"]?.ToLowerInvariant() != "true")
            martenConfiguration.AddAsyncDaemon(DaemonMode.Solo);

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
