namespace AssociationRegistry.Admin.MartenDb.VertegenwoordigerPersoonsgegevens;

public record VertegenwoordigerPersoonsgegevensFilter
{
    public Guid RefId { get; }

    public VertegenwoordigerPersoonsgegevensFilter(Guid refId)
    {
        RefId = refId;
    }
}