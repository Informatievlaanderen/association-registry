namespace AssociationRegistry.MartenDb.BankrekeningnummerPersoonsgegevens;

public record BankrekeningnummerPersoonsgegevensByRefIdsFilter
{
    public Guid[] RefIds { get; }

    public BankrekeningnummerPersoonsgegevensByRefIdsFilter(Guid[] refIds)
    {
        RefIds = refIds;
    }
}
