namespace AssociationRegistry.Scheduled.Host.Bewaartermijnen;

public class BewaartermijnOptions
{
    public string SlackWebhook { get; set; }
    public string Cron { get; set; } = "0 0 3 * * ?";
}
