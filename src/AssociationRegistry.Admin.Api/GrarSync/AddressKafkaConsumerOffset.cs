namespace AssociationRegistry.Admin.Api.GrarSync;

using System;

public record AddressKafkaConsumerOffset
{
    public long Timestamp { get; set; }
    public DateTime DateTime { get; set; }
    public string IdempotenceKey { get; set; }
    public string Key { get; init; }
    public long Offset { get; init; }
}
