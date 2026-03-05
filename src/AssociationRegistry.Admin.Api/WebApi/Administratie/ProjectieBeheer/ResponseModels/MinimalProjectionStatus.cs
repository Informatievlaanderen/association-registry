namespace AssociationRegistry.Admin.Api.WebApi.Administratie.ProjectieBeheer.ResponseModels;

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
        var highWaterMarkShard = projectionStatus.Single(s => s.ShardName.Equals(ProjectionStatus.HighWaterMarkKey));

        HighWaterMark = highWaterMarkShard.Sequence;
        Timestamp = highWaterMarkShard.Timestamp;

        var status = projectionStatus
                    .Where(w => !w.ShardName.Equals(ProjectionStatus.HighWaterMarkKey))
                    .OrderBy(o => o.ShardName);

        Status = status.ToDictionary(k => k.ShardName.Replace(ProjectionStatus.AllSuffix, string.Empty), v => (long)v.Sequence);
    }
}
