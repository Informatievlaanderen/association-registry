namespace AssociationRegistry.MartenDb.BankrekeningnummerPersoonsgegevens;

public record BankrekeningnummerPersoonsgegevensByRefIdFilter
{
    public Guid RefId { get; }

    public BankrekeningnummerPersoonsgegevensByRefIdFilter(Guid refId)
    {
        RefId = refId;
    }
}
