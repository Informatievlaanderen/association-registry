namespace AssociationRegistry.Admin.Api.Extentions;

using ConfigurationBindings;
using Constants;
using Infrastructure.Json;
using Verenigingen.VCodes;
using Marten;
using Marten.Events;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;
using VCodes;

public static class MartenExtentions
{
    public static IServiceCollection AddMarten(this IServiceCollection services, PostgreSqlOptionsSection postgreSqlOptions)
    {
        services.AddMarten(
                opts =>
                {
                    opts.Connection(GetPostgresConnectionString(postgreSqlOptions));
                    opts.Events.StreamIdentity = StreamIdentity.AsString;
                    opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));
                    opts.Serializer(CreateCustomMartenSerializer());
                })
            .ApplyAllDatabaseChangesOnStartup();

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
