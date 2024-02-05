namespace AssociationRegistry.Admin.Schema.Detail;

public record Doelgroep
{
    public JsonLdMetadata JsonLdMetadata { get; init; }
    public int Minimumleeftijd { get; init; }
    public int Maximumleeftijd { get; init; }
}
