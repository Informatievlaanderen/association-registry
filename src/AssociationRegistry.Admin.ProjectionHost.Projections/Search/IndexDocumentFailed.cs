namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

public class IndexDocumentFailed : Exception
{
    public IndexDocumentFailed(string debugInformation) : base($"IndexDocument failed: {debugInformation}")
    {
    }
}

public class BulkAllFailed : Exception
{
    public BulkAllFailed(string[] errorMessages, string debugInformation) : base($"IndexDocument failed: " +
                                                                                 $"{string.Join(", \n", errorMessages)} \n " +
                                                                                 $"Debug info: {debugInformation}")
    {
    }
}
