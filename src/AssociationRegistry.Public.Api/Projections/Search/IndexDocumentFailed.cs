namespace AssociationRegistry.Public.Api.Projections.Search;

using System;

public class IndexDocumentFailed : Exception
{
    public IndexDocumentFailed(string debugInformation) : base($"IndexDocument failed: {debugInformation}")
    {
    }
}
