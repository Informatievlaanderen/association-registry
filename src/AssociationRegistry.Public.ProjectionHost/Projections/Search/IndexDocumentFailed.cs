namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System;

public class IndexDocumentFailed : Exception
{
    public IndexDocumentFailed(string debugInformation) : base($"IndexDocument failed: {debugInformation}")
    {
    }
}
