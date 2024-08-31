namespace AssociationRegistry.ScheduledTaskHost.Configuration;

public record AddressSynchronisationOptions(string BaseUrl, string ApiKey, string SlackWebhook, string CronExpression)
{
    public const string SectionName = "AddressSyncOptions";
}
