namespace AssociationRegistry.Admin.Api.Infrastructure;

public class ProjectionStatus
{
    public const string HighWaterMarkKey = "HighWaterMark";
    public const string AllSuffix = ":All";

    public DateTimeOffset Timestamp { get; set; }
    public string ShardName { get; set; }
    public int Sequence { get; set; }
    public string Exception { get; set; }
}
