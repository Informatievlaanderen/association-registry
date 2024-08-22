namespace AssociationRegistry.Admin.Api.GrarSync;

using Marten.Schema;

public record AddressKafkaConsumerOffset
{
    [Identity] public string IdempotenceKey { get; set; }
    public long Timestamp { get; set; }
    public DateTime DateTime { get; set; }
    public string Key { get; init; }
    public long Offset { get; init; }
}
