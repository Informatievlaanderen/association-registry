namespace AssociationRegistry.Public.Api.S3;

using System.Collections.Generic;

public class S3BlobClientOptions
{
    public IDictionary<string, Bucket> Buckets { get; set; } = null!;

    public class Bucket
    {
        public string Name { get; set; } = null!;
        public IDictionary<string, string> Blobs { get; set; } = null!;
    }
}
