namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

public class IndexDocumentFailed : Exception
{
    public IndexDocumentFailed(string debugInformation) : base($"IndexDocument failed: {debugInformation}")
    {
    }
}
