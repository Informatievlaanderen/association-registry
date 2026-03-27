namespace AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;

using AssociationRegistry.DecentraalBeheer.Vereniging;

public record BewaartermijnId
{
    public const string BewaartermijnAggregateName = "Bewaartermijn";

    public BewaartermijnId(VCode vCode, PersoonsgegevensType persoonsgegevensType, int entityId)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(entityId);

        VCode = vCode;
        PersoonsgegevensType = persoonsgegevensType;
        EntityId = entityId;
    }

    public static implicit operator string(BewaartermijnId bewaartermijnId) =>
        $"{BewaartermijnAggregateName}-{bewaartermijnId.VCode}-{bewaartermijnId.PersoonsgegevensType.Value}-{bewaartermijnId.EntityId}";

    public VCode VCode { get; }
    public PersoonsgegevensType PersoonsgegevensType { get; }
    public int EntityId { get; }

    public static string CreateId(VCode vCode, PersoonsgegevensType persoonsgegevensType, int entityId) =>
        $"{BewaartermijnAggregateName}-{vCode}-{persoonsgegevensType.Value}-{entityId}";
}
