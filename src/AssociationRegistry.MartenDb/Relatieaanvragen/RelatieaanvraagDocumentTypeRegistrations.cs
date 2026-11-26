namespace AssociationRegistry.MartenDb.Relatieaanvragen;

using Marten;

public static class RelatieaanvraagDocumentTypeRegistrations
{
    /// <summary>
    ///     Used by the Admin API, which saves and deletes the guards, and by the scheduled host, which deletes them on
    ///     verval. The scheduled host asserts at startup that the database matches its configuration: both hosts must
    ///     configure the table the same way.
    /// </summary>
    public static StoreOptions RegisterOpenRelatieaanvraagDocument(this StoreOptions opts)
    {
        opts.RegisterDocumentType<OpenRelatieaanvraagDocument>();

        return opts;
    }
}
