namespace AssociationRegistry.Public.Api.Projections;

using System;

public class IndexDocumentFailed : Exception
{
    public IndexDocumentFailed(string debugInformation) : base($"IndexDocument failed: {debugInformation}")
    {
    }
}
