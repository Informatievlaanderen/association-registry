namespace AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;

public record VertegenwoordigerPersoonsgegevensByRefIdsFilter
{
    public Guid[] RefIds { get; }

    public VertegenwoordigerPersoonsgegevensByRefIdsFilter(Guid[] refIds)
    {
        RefIds = refIds;
    }
}
