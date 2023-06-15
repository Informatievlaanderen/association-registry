namespace AssociationRegistry.Admin.Api.Infrastructure.Extensions;

using ConfigurationBindings;
using Constants;
using JasperFx.CodeGeneration;
using Json;
using Marten;
using Marten.Events;
using Marten.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ProjectionHost.Projections.Search;
using Schema.Detail;
using Schema.Historiek;
using VCodeGeneration;
using Vereniging;

public static class MartenExtentions
{
    public static IServiceCollection AddMarten(this IServiceCollection services, PostgreSqlOptionsSection postgreSqlOptions, IConfiguration configuration)
    {
        services
            .AddTransient<IElasticRepository, ElasticRepository>();
        var martenConfiguration = services.AddMarten(
            sp =>
            {
                var opts = new StoreOptions();
                opts.Connection(postgreSqlOptions.GetConnectionString());
                opts.Events.StreamIdentity = StreamIdentity.AsString;
                opts.Storage.Add(new VCodeSequence(opts, VCode.StartingVCode));
                opts.Serializer(CreateCustomMartenSerializer());
                opts.Events.MetadataConfig.EnableAll();

                opts.GeneratedCodeMode = TypeLoadMode.Auto;
                opts.RegisterDocumentType<BeheerVerenigingDetailDocument>();
                opts.RegisterDocumentType<BeheerVerenigingHistoriekDocument>();
                return opts;
            });

        martenConfiguration.ApplyAllDatabaseChangesOnStartup();

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
