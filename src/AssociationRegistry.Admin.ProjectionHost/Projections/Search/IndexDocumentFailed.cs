namespace AssociationRegistry.Admin.ProjectionHost.Projections.Search;

public class IndexDocumentFailed : Exception
{
    public IndexDocumentFailed(string debugInformation) : base($"IndexDocument failed: {debugInformation}")
    {
    }
}
