namespace AssociationRegistry.Integrations.Grar.Bewaartermijnen;

public class BewaartermijnOptions
{
    public TimeSpan Duration { get; set; } = TimeSpan.FromDays(365 * 2);
}
