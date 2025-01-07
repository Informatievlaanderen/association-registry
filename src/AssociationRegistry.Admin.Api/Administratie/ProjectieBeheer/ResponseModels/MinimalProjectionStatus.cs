namespace AssociationRegistry.Admin.Api.Administratie.ProjectieBeheer.ResponseModels;

using AssociationRegistry.Admin.Api.Infrastructure;
using System.Text.Json.Serialization;

public record MinimalProjectionStatusResponse
{
    [JsonPropertyOrder(0)]
    public long HighWaterMark { get; init; }

    [JsonPropertyOrder(1)]
    public IReadOnlyDictionary<string, long> Status { get; init; }

    [JsonPropertyOrder(2)]
    public DateTimeOffset Timestamp { get; init; }

    public MinimalProjectionStatusResponse(ProjectionStatus[] projectionStatus)
    {
        var highWaterMarkShard = projectionStatus.Single(s => s.ShardName.Equals("HighWaterMark"));

        HighWaterMark = highWaterMarkShard.Sequence;
        Timestamp = highWaterMarkShard.Timestamp;

        var status = projectionStatus
                    .Where(w => !w.ShardName.Equals("HighWaterMark"))
                    .Select(s => new MinimalProjectionStatus(s.ShardName, s.Sequence))
                    .OrderBy(o => o.ShardName);

        Status = status.ToDictionary(keySelector: k => k.ShardName, elementSelector: v => v.Sequence);
    }

    public record MinimalProjectionStatus(string FullShardName, long Sequence)
    {
        public string ShardName { get; init; } = FullShardName.Split('.').Last().Replace(oldValue: ":All", newValue: "");
    }
}
