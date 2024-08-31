namespace AssociationRegistry.ScheduledTaskHost.Invocables;

using AssociationRegistry.Notifications;
using Coravel.Invocable;
using Grar.AddressSync;
using Helpers;
using Marten;
using Notifications;

public class AddressSynchronisationInvocable : IInvocable, ICancellableInvocable
{
    private readonly TeSynchroniserenLocatieAdresMessageHandler _handler;
    private readonly IDocumentStore _store;
    private readonly INotifier _notifier;
    private readonly ITeSynchroniserenLocatiesFetcher _teSynchroniserenLocatiesFetcher;
    private readonly ILogger<AddressSynchronisationInvocable> _logger;

    public AddressSynchronisationInvocable(
        TeSynchroniserenLocatieAdresMessageHandler handler,
        IDocumentStore store,
        INotifier notifier,
        ITeSynchroniserenLocatiesFetcher teSynchroniserenLocatiesFetcher,
        ILogger<AddressSynchronisationInvocable> logger)
    {
        _handler = handler;
        _store = store;
        _notifier = notifier;
        _teSynchroniserenLocatiesFetcher = teSynchroniserenLocatiesFetcher;
        _logger = logger;
    }

    public async Task Invoke()
    {
        var session = _store.LightweightSession();

        try
        {
            _logger.LogInformation($"Adressen synchroniseren werd gestart.");

            var messages = await _teSynchroniserenLocatiesFetcher.GetTeSynchroniserenLocaties(session, CancellationToken);

            _logger.LogInformation($"Er werden {messages.Count()} synchroniseer berichten gevonden.");

            foreach (var synchroniseerLocatieMessage in messages)
            {
                await _handler.Handle(synchroniseerLocatieMessage, CancellationToken);
            }

            _logger.LogInformation($"Adressen synchroniseren werd voltooid.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Adressen synchroniseren kon niet voltooid worden. {ex.Message}");
            await _notifier.Notify(new AdresSynchronisatieGefaald(ex));

            throw;
        }
    }

    public CancellationToken CancellationToken { get; set; }
}

public record AddressSynchronisationOptions(string BaseUrl, string ApiKey, string SlackWebhook, string CronExpression)
{
    public const string SectionName = "AddressSyncOptions";
}
