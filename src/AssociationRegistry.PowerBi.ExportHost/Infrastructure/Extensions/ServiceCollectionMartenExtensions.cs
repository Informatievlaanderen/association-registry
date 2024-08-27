namespace AssociationRegistry.PowerBi.ExportHost.Infrastructure.Extensions;

using Admin.Schema.Detail;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx.CodeGeneration;
using Marten;
using Marten.Events;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using Weasel.Core;

public static class ServiceCollectionMartenExtensions
{
    public static StoreOptions GetStoreOptions(PostgreSqlOptionsSection postgreSqlOptions)
    {
        var opts = new StoreOptions();
        opts.Connection(postgreSqlOptions.GetConnectionString());
        opts.Serializer(CreateMartenSerializer());
        opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.Events.MetadataConfig.EnableAll();
        opts.AutoCreateSchemaObjects = AutoCreate.None;

        opts.RegisterDocumentType<LocatieLookupDocument>();

        opts.Schema.For<LocatieLookupDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        return opts;
    }

    private static JsonNetSerializer CreateMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Customize(
            s =>
            {
                s.DateParseHandling = DateParseHandling.None;
            });

        return jsonNetSerializer;
    }
}
