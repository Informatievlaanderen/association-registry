namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record BewaartermijnId
{
    public const string BewaartermijnAggregateName = "Bewaartermijn";

    public BewaartermijnId(VCode vCode, PersoonsgegevensType persoonsgegevensType, int recordId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recordId);

        VCode = vCode;
        PersoonsgegevensType = persoonsgegevensType;
        RecordId = recordId;
    }

    public static implicit operator string(BewaartermijnId bewaartermijnId) =>
        $"{BewaartermijnAggregateName}-{bewaartermijnId.VCode}-{bewaartermijnId.PersoonsgegevensType.Value}-{bewaartermijnId.RecordId}";

    public VCode VCode { get; }
    public PersoonsgegevensType PersoonsgegevensType { get; }
    public int RecordId { get; }

    public static string CreateId(VCode vCode, PersoonsgegevensType persoonsgegevensType, int recordId) =>
        $"{BewaartermijnAggregateName}-{vCode}-{persoonsgegevensType.Value}-{recordId}";
}
