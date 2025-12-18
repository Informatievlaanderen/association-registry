namespace AssociationRegistry.CommandHandling.MagdaSync.SyncKsz;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using Magda.Persoon;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;
using Wolverine;

public class SyncKszMessageHandler
{
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerPersoonsgegevensRepository;
    private readonly IMagdaGeefPersoonService _magdaGeefPersoonService;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<SyncKszMessageHandler> _logger;

    public SyncKszMessageHandler(
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerPersoonsgegevensRepository,
        IMagdaGeefPersoonService magdaGeefPersoonService,
        IMessageBus messageBus,
        ILogger<SyncKszMessageHandler> logger)
    {
        _vertegenwoordigerPersoonsgegevensRepository = vertegenwoordigerPersoonsgegevensRepository;
        _magdaGeefPersoonService = magdaGeefPersoonService;
        _messageBus = messageBus;
        _logger = logger;
    }

    public async Task Handle(
        SyncKszMessage message,
        CancellationToken cancellationToken = default)
    {
        // persoonsrepository getby insz
        // if [] -> return
        // else
        // magda geef persoon
        // if ! overleden -> return
        // else
        // foreach distinc persoon record
        // send cmd foreach VCode vertegenwoordigerId
        // postgres queue + aantal retries
        await _messageBus.SendAsync(new MarkeerVertegenwoordigerAlsOverledenMessage(VCode.Hydrate("V0001001"), 111));

        _logger.LogInformation("done handling ---------------------------");
        return;
       var vertegenwoordigerPersoonsgegevens = await _vertegenwoordigerPersoonsgegevensRepository.Get(message.Insz, cancellationToken);

       if(!vertegenwoordigerPersoonsgegevens.Any())
           return;

       var commandMetadata = CommandMetadata.ForDigitaalVlaanderenProcess;

       var magdaPersoon = await _magdaGeefPersoonService.GeefPersoon(
           new GeefPersoonRequest(
               message.Insz.Value),
           commandMetadata,
           cancellationToken);

       if(!magdaPersoon.Overleden)
           return;

       foreach (var vertegenwoordigerPersoonsgegeven in vertegenwoordigerPersoonsgegevens.DistinctBy(x => x.VCode))
       {
           await _messageBus.SendAsync(new CommandEnvelope<MarkeerVertegenwoordigerAlsOverledenMessage>(new MarkeerVertegenwoordigerAlsOverledenMessage(vertegenwoordigerPersoonsgegeven.VCode, vertegenwoordigerPersoonsgegeven.VertegenwoordigerId), commandMetadata));
       }
    }
}

public record MarkeerVertegenwoordigerAlsOverledenMessage(VCode VCode, int VertegenwoordigerId);
