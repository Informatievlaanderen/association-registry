using System;
using System.Collections.Generic;

namespace AssociationRegistry.Acm.Api.S3;

public class S3BlobClientOptions
{
    public IDictionary<string, Bucket> Buckets { get; set; }
    public class Bucket
    {
        public string Name { get; set; }
        public IDictionary<string, string> Blobs { get; set; }
    }
}
