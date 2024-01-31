namespace AssociationRegistry.Admin.Schema.Detail;

public record Doelgroep
{
    public int Minimumleeftijd { get; init; }
    public int Maximumleeftijd { get; init; }
}