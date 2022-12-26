namespace AssociationRegistry.Admin.Api.Extensions;

using AssociationRegistry.Admin.Api.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Constants;
using AssociationRegistry.Admin.Api.Infrastructure.Json;
using AssociationRegistry.Admin.Api.Verenigingen.VCodes;
using AssociationRegistry.VCodes;
using Infrastructure;
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
        var martenConfiguration = services.AddMarten(
            opts =>
            {
                opts.Connection(GetPostgresConnectionString(postgreSqlOptions));
                opts.Events.StreamIdentity = StreamIdentity.AsString;
                opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));
                opts.Serializer(CreateCustomMartenSerializer());
                opts.Events.MetadataConfig.EnableAll();
                opts.AddPostgresProjections();
            });

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

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
                s.DateParseHandling = DateParseHandling.None;
                s.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                s.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });
        return jsonNetSerializer;
    }
}
