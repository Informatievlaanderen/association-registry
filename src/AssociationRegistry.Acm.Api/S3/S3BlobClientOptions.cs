using System.Collections.Generic;

namespace AssociationRegistry.Acm.Api.S3;

public class S3BlobClientOptions
{
    public IDictionary<string, Bucket> Buckets { get; set; } = null!;

    public class Bucket
    {
        public string Name { get; set; } = null!;
        public IDictionary<string, string> Blobs { get; set; } = null!;
    }
}
