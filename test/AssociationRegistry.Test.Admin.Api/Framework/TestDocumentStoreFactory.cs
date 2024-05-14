namespace AssociationRegistry.Test.Admin.Api.Framework;

using Marten;
using Marten.Events;

public static class TestDocumentStoreFactory
{
    public static DocumentStore Create()
    {
        return DocumentStore.For(options =>
        {
            options.Connection(GetConnectionString());
            options.Events.StreamIdentity = StreamIdentity.AsString;
        });
    }

    private static string GetConnectionString()
        => $"host=127.0.0.1;" +
           $"database=verenigingsregister;" +
           $"password=root;" +
           $"username=root";
}
