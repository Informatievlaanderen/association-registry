using System;
using System.Collections.Generic;

namespace AssociationRegistry.Acm.Api.S3;

public class S3BlobClientOptions
{
    private IDictionary<string, string> _buckets = null!;

    public IDictionary<string, string> Buckets
    {
        get => _buckets;
        set => _buckets = new Dictionary<string, string>(value, StringComparer.OrdinalIgnoreCase);
    }
}
