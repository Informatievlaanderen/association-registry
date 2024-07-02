namespace AssociationRegistry.Test.Admin.AddressSync;

using Marten;
using Marten.Events;
using Weasel.Core;

public class TestDocumentStoreFactory
{
    public static async Task<DocumentStore> Create(string schema)
    {
        var documentStore = DocumentStore.For(options =>
        {
            options.Connection($"host=localhost:5432;" +
                               "database=verenigingsregister;" +
                               $"password=root;" +
                               $"username=root");

            options.Events.StreamIdentity = StreamIdentity.AsString;

            options.DatabaseSchemaName = schema;
            options.Events.DatabaseSchemaName = schema;
            options.AutoCreateSchemaObjects = AutoCreate.All;
        });

        await documentStore.Advanced.ResetAllData();
        return documentStore;
    }
}
