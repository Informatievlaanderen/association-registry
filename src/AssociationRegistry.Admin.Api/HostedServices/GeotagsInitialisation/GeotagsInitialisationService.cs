namespace AssociationRegistry.Admin.Api.HostedServices.GeotagsInitialisation;

using DecentraalBeheer.Geotags.InitialiseerGeotags;
using Framework;
using Queries;
using Vereniging;
using Wolverine;

public class GeotagsInitialisationService : BackgroundService
{
    private readonly IVerenigingenWithoutGeotagsQuery _query;
    private readonly IMessageBus _bus;
    private readonly ILogger<GeotagsInitialisationService> _logger;

    public GeotagsInitialisationService(IVerenigingenWithoutGeotagsQuery query, IMessageBus bus, ILogger<GeotagsInitialisationService> logger)
    {
        _query = query;
        _bus = bus;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GeotagsInitialisationService is starting.");

        var verenigingenWithoutGeotags = await _query.ExecuteAsync(cancellationToken);
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        foreach (var verenigingWithoutGeotag in verenigingenWithoutGeotags)
        {
            var command = new CommandEnvelope<InitialiseerGeotagsCommand>(new InitialiseerGeotagsCommand(VCode.Create(verenigingWithoutGeotag)), metadata);
            await _bus.InvokeAsync(command, cancellationToken);
        }

        _logger.LogInformation("GeotagsInitialisationService is stopping.");
        await StopAsync(cancellationToken);
    }
}
