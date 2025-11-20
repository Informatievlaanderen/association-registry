namespace AssociationRegistry.MartenDb.VertegenwoordigerPersoonsgegevens;

public record VertegenwoordigerPersoonsgegevensByRefIdFilter
{
    public Guid RefId { get; }

    public VertegenwoordigerPersoonsgegevensByRefIdFilter(Guid refId)
    {
        RefId = refId;
    }
}
