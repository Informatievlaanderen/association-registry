namespace AssociationRegistry.PowerBi.ExportHost.Infrastructure.Extensions;

using Admin.Schema.Detail;
using Hosts.Configuration.ConfigurationBindings;
using JasperFx;
using JasperFx.Events;
using Marten;
using Marten.Events;
using Marten.Services;
using Newtonsoft.Json;
using Weasel.Core;

public static class ServiceCollectionMartenExtensions
{
    public static StoreOptions GetStoreOptions(PostgreSqlOptionsSection postgreSqlOptions)
    {
        var opts = new StoreOptions();
        opts.Connection(postgreSqlOptions.GetConnectionString());
        opts.UseNewtonsoftForSerialization(configure: settings =>
        {
            settings.DateParseHandling = DateParseHandling.None;
            // s.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
            // s.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
        });
        opts.Events.StreamIdentity = StreamIdentity.AsString;

        opts.Events.MetadataConfig.EnableAll();
        opts.AutoCreateSchemaObjects = AutoCreate.None;

        opts.RegisterDocumentType<LocatieLookupDocument>();

        opts.Schema.For<LocatieLookupDocument>()
            .UseNumericRevisions(true)
            .UseOptimisticConcurrency(false);

        return opts;
    }
}
