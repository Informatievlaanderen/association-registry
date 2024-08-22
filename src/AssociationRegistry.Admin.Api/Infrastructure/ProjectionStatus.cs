namespace AssociationRegistry.Admin.Api.Infrastructure;

public class ProjectionStatus
{
    public DateTimeOffset Timestamp { get; set; }
    public string ShardName { get; set; }
    public int Sequence { get; set; }
    public string Exception { get; set; }
}
