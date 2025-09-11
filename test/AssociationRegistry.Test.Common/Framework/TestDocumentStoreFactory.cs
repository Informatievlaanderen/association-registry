namespace AssociationRegistry.Test.Common.Framework;

using Admin.Api.Infrastructure.Json;
using Database;
using Formats;
using Hosts.Configuration;
using JasperFx;
using JasperFx.Events;
using Marten;
using Marten.Events;
using Marten.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Weasel.Core;

public static class TestDocumentStoreFactory
{
    public static async Task<DocumentStore> CreateAsync(string schema)
    {
        // Create schema from template for faster test initialization
        var configuration = ConfigurationHelper.GetConfiguration();
        DatabaseTemplateHelper.CreateSchemaFromTemplate(
            configuration, 
            schema, 
            "golden_master_template", 
            NullLogger.Instance);

        var documentStore = DocumentStore.For(options =>
        {
            options.Connection("host=127.0.0.1:5432;" +
                               "database=golden_master_template;" +
                               "password=root;" +
                               "username=root");

            options.Events.StreamIdentity = StreamIdentity.AsString;

            options.DatabaseSchemaName = schema;
            options.Events.DatabaseSchemaName = schema;
            options.AutoCreateSchemaObjects = AutoCreate.None; // Schema already exists from template
            options.Events.MetadataConfig.EnableAll();
            options.Serializer(CreateCustomMartenSerializer());
            options.UseNewtonsoftForSerialization(configure: settings =>
            {
                settings.DateParseHandling = DateParseHandling.None;
                settings.Converters.Add(new NullableDateOnlyJsonConvertor(WellknownFormats.DateOnly));
                settings.Converters.Add(new DateOnlyJsonConvertor(WellknownFormats.DateOnly));
            });
        });

        // No need to reset data since we created a fresh schema from template
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
