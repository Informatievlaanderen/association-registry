namespace AssociationRegistry.Scheduled.Host.DubbelBeheer.Schema;

using System.Security.Cryptography;
using System.Text;
using Marten.Metadata;

/// <summary>
///     Eén ingelezen CSV-bestand en de voortgang van de verwerking ervan.
///     De <see cref="Id" /> wordt afgeleid van de <see cref="S3Key" />, zodat hetzelfde bestand maar één keer
///     ingelezen kan worden.
///     Een run die met de batch bezig is, houdt een lease bij via <see cref="LockedBy" /> en <see cref="LeaseUntil" />.
///     Wijzigingen worden bewaakt met optimistic concurrency via <see cref="Version" />.
/// </summary>
public class DubbelImportBatchDocument : IVersioned
{
    public static readonly TimeSpan LeaseDuration = TimeSpan.FromMinutes(5);

    public Guid Id { get; set; }
    public string S3Key { get; set; } = string.Empty;
    public DubbelImportBatchState State { get; set; }

    /// <summary>De run die de lease heeft, <see cref="Guid.Empty" /> als niemand de lease heeft.</summary>
    public Guid LockedBy { get; set; }

    /// <summary>Tot wanneer de lease loopt, <see cref="DateTimeOffset.MinValue" /> als niemand de lease heeft.</summary>
    public DateTimeOffset LeaseUntil { get; set; }

    public Guid Version { get; set; }

    public static Guid IdFor(string s3Key) =>
        new(SHA256.HashData(Encoding.UTF8.GetBytes(s3Key)).AsSpan(start: 0, length: 16));

    public static DubbelImportBatchDocument Create(string s3Key, Guid runId, DateTimeOffset now)
    {
        var batch = new DubbelImportBatchDocument
        {
            Id = IdFor(s3Key),
            S3Key = s3Key,
            State = DubbelImportBatchState.Pending,
        };

        batch.ExtendLease(runId, now);

        return batch;
    }

    public bool IsLeasedByOtherRun(Guid runId, DateTimeOffset now) => LeaseUntil > now && LockedBy != runId;

    public void ExtendLease(Guid runId, DateTimeOffset now)
    {
        LockedBy = runId;
        LeaseUntil = now + LeaseDuration;
    }

    public void ReleaseLease()
    {
        LockedBy = Guid.Empty;
        LeaseUntil = DateTimeOffset.MinValue;
    }
}

public enum DubbelImportBatchState
{
    Pending,
    Finished,
    ReportSent,
}
