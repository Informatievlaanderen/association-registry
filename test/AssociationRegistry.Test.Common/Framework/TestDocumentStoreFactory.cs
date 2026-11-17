namespace AssociationRegistry.Test.Common.Framework;

using Admin.Api.Infrastructure.Json;
using Formats;
using JasperFx;
using JasperFx.Events;
using Marten;
using Marten.Newtonsoft;
using Marten.Services;
using MartenDb.Setup;
using Newtonsoft.Json;

public static class TestDocumentStoreFactory
{
    public static async Task<DocumentStore> CreateAsync(string schema, Action<StoreOptions>? configure = null)
    {
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection(
                "host=127.0.0.1:5432;"
                    + "database=verenigingsregister;"
                    + "password=root;"
                    + "username=root;"
                    // Wait up to 60s (default 15s) to open a connection: test classes run unbounded in parallel,
                    // and on busy CI runners Postgres sometimes needs longer than 15s to accept new connections.
                    + "Timeout=60"
            );

            options.Events.StreamIdentity = StreamIdentity.AsString;

            options.DatabaseSchemaName = schema;
            options.Events.DatabaseSchemaName = schema;
            options.AutoCreateSchemaObjects = AutoCreate.All;
            options.Events.MetadataConfig.EnableAll();
            options.Serializer(CreateCustomMartenSerializer());
            options.KeepV8Defaults();

            options.UseNewtonsoftForSerialization(configure: settings =>
            {
                settings.DateParseHandling = DateParseHandling.None;
                settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });

            configure?.Invoke(options);
        });

        await documentStore.Advanced.ResetAllData();

        return documentStore;
    }

    public static JsonNetSerializer CreateCustomMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Configure(s => { });

        return jsonNetSerializer;
    }
}
