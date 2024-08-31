namespace AssociationRegistry.ScheduledTaskHost.HostedServices;

public class AddressMigrationKafkaConsumer : BackgroundService
{
    private readonly ILogger<AddressMigrationKafkaConsumer> _logger;

    public AddressMigrationKafkaConsumer(ILogger<AddressMigrationKafkaConsumer> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
