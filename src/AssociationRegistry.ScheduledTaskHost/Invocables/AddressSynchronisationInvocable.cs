namespace AssociationRegistry.ScheduledTaskHost.Invocables;

using AssociationRegistry.Notifications;
using Coravel.Invocable;
using Coravel.Mailer.Mail.Interfaces;
using Grar.AddressSync;
using Helpers;
using Mailables;
using Marten;
using Notifications;

public class AddressSynchronisationInvocable : IInvocable, ICancellableInvocable
{
    private readonly TeSynchroniserenLocatieAdresMessageHandler _handler;
    private readonly IDocumentStore _store;
    private readonly INotifier _notifier;
    private readonly IMailer _mailer;
    private readonly ITeSynchroniserenLocatiesFetcher _teSynchroniserenLocatiesFetcher;
    private readonly ILogger<AddressSynchronisationInvocable> _logger;

    public AddressSynchronisationInvocable(
        TeSynchroniserenLocatieAdresMessageHandler handler,
        IDocumentStore store,
        INotifier notifier,
        IMailer mailer,
        ITeSynchroniserenLocatiesFetcher teSynchroniserenLocatiesFetcher,
        ILogger<AddressSynchronisationInvocable> logger)
    {
        _handler = handler;
        _store = store;
        _notifier = notifier;
        _mailer = mailer;
        _teSynchroniserenLocatiesFetcher = teSynchroniserenLocatiesFetcher;
        _logger = logger;
    }

    public async Task Invoke()
    {
        var result = await ExecuteAsync();
        await _mailer.SendAsync(new AddressSynchronisationReport(result));
    }

    public async Task<AddressSynchronisationReportModel> ExecuteAsync()
    {
        var result = new AddressSynchronisationReportModel();
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

        return result;
    }

    public CancellationToken CancellationToken { get; set; }
}
