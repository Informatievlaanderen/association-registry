namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record BewaartermijnId
{
    public const string BewaartermijnAggregateName = "Bewaartermijn";

    public BewaartermijnId(VCode vCode, BewaartermijnType bewaartermijnType, int recordId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recordId);

        VCode = vCode;
        BewaartermijnType = bewaartermijnType;
        RecordId = recordId;
    }

    public static implicit operator string(BewaartermijnId bewaartermijnId) =>
        $"{BewaartermijnAggregateName}-{bewaartermijnId.VCode}-{bewaartermijnId.BewaartermijnType.Value}-{bewaartermijnId.RecordId}";

    public VCode VCode { get; }
    public BewaartermijnType BewaartermijnType { get; }
    public int RecordId { get; }

    public static string CreateId(VCode vCode, BewaartermijnType bewaartermijnType, int recordId) =>
        $"{BewaartermijnAggregateName}-{vCode}-{bewaartermijnType.Value}-{recordId}";
}
