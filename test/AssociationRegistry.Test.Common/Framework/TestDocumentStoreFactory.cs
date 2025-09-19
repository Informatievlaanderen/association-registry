namespace AssociationRegistry.Test.Common.Framework;

using Admin.Api.Infrastructure.Json;
using Formats;
using JasperFx;
using JasperFx.Events;
using Marten;
using Marten.Events;
using Marten.Services;
using Newtonsoft.Json;
using Weasel.Core;

public static class TestDocumentStoreFactory
{
    public static async Task<DocumentStore> CreateAsync(string schema, bool applyDbChanges = false)
    {
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection("host=127.0.0.1:5432;" +
                               "database=verenigingsregister;" +
                               "password=root;" +
                               "username=root");

            options.Events.StreamIdentity = StreamIdentity.AsString;

            options.DatabaseSchemaName = schema;
            options.Events.DatabaseSchemaName = schema;
            options.AutoCreateSchemaObjects = AutoCreate.All;
            options.Events.MetadataConfig.EnableAll();
            options.Serializer(CreateCustomMartenSerializer());
            options.UseNewtonsoftForSerialization(configure: settings =>
            {
                settings.DateParseHandling = DateParseHandling.None;
                settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });
        });

        if(applyDbChanges)
            documentStore.Storage.ApplyAllConfiguredChangesToDatabaseAsync().GetAwaiter().GetResult();

        await documentStore.Advanced.ResetAllData();

        return documentStore;
    }

    public static JsonNetSerializer CreateCustomMartenSerializer()
    {
        var jsonNetSerializer = new JsonNetSerializer();

        jsonNetSerializer.Configure(
            s =>
            {
            });

        return jsonNetSerializer;
    }
}
