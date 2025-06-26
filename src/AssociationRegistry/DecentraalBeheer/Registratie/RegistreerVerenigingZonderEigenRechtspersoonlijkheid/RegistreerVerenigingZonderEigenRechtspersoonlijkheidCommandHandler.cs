namespace AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using DuplicateVerenigingDetection;
using Events;
using Framework;
using Grar.Clients;
using JasperFx.Core;
using Messages;
using Vereniging;
using Marten;
using Microsoft.Extensions.Logging;
using ResultNet;
using Vereniging.Geotags;
using Wolverine;
using Wolverine.Marten;
using Wolverine.Runtime;

public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler
{
    private readonly IClock _clock;
    private readonly IGrarClient _grarClient;
    private readonly IGeotagsService _geotagsService;
    private readonly ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> _logger;
    private readonly IDuplicateVerenigingDetectionService _duplicateVerenigingDetectionService;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        IMartenOutbox outbox,
        IDocumentSession session,
        IClock clock,
        IGrarClient grarClient,
        IGeotagsService geotagsService,
        ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _outbox = outbox;
        _session = session;
        _clock = clock;
        _grarClient = grarClient;
        _geotagsService = geotagsService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> message,
        EnrichedCommand enrichedCommand,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} start");

        var command = message.Command;

        if (!command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateVerenigingDetectionService.ExecuteAsync(command.Naam, command.Locaties)).ToList();

            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }


        var vereniging = await Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
            command,
            _vCodeService,
            _geotagsService,
            _clock);

        var (metAdresId, zonderAdresId) = vereniging.GeefLocatiesMetEnZonderAdresId();

        await vereniging.NeemAdresDetailsOver(metAdresId, _grarClient, CancellationToken.None);
        await vereniging.BerekenGeotags(_geotagsService);

        foreach (var locatieZonderAdresId in zonderAdresId)
        {
            await _outbox.SendAsync(new TeAdresMatchenLocatieMessage(vereniging.VCode, locatieZonderAdresId.LocatieId));
        }

        var result = await _verenigingsRepository.SaveNew(vereniging, _session ,message.Metadata, cancellationToken);

        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} end");

        return Result.Success(CommandResult.Create(vereniging.VCode, result));
    }
}

public class GrarAddressEnrichmentMiddleware
{
    private readonly IGrarClient _grarClient;
    private readonly ILogger<GrarAddressEnrichmentMiddleware> _logger;

    public GrarAddressEnrichmentMiddleware(IGrarClient grarClient, ILogger<GrarAddressEnrichmentMiddleware> logger)
    {
        _grarClient = grarClient;
        _logger = logger;
    }

    public async Task<EnrichedCommand> Before(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> command)
    {
        _logger.LogInformation("Enriching command {CommandType} with GRAR address data",
                               typeof(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>).Name);

        var enrichedLocaties = new List<Locatie>();

        foreach (var locatie in command.Command.Locaties)
        {
            if (locatie.Adres is not null)
            {
                enrichedLocaties.Add(locatie);

                continue;
            }

            try
            {
                var adresDetails = await _grarClient.GetAddressById(locatie.AdresId.Bronwaarde, CancellationToken.None);

                // Create enriched locatie with address detail

                enrichedLocaties.Add(locatie with
                {
                    Adres = Adres.Create(adresDetails.Straatnaam,
                                         adresDetails.Huisnummer,
                                         adresDetails.Busnummer,
                                         adresDetails.Postcode,
                                         adresDetails.Gemeente,
                                         Adres.België)
                });

                _logger.LogDebug("Enriched locatie {LocatieId} with address {Adres}",
                                 locatie.LocatieId, $"{adresDetails.Straatnaam} {adresDetails.Huisnummer}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to enrich locatie {LocatieId} with AdresId {AdresId}",
                                   locatie.LocatieId, locatie.AdresId);

                // Keep original locatie if enrichment fails
                enrichedLocaties.Add(locatie);
            }
        }

        return new EnrichedCommand(command, enrichedLocaties.ToArray());
    }
}
public static class AccountLookupMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<EnrichedCommand> BeforeAsync(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> command, IGrarClient _grarClient)
    {

        var enrichedLocaties = new List<Locatie>();

        foreach (var locatie in command.Command.Locaties)
        {
            if (locatie.Adres is not null)
            {
                enrichedLocaties.Add(locatie);

                continue;
            }

            try
            {
                var adresDetails = await _grarClient.GetAddressById(locatie.AdresId.Bronwaarde, CancellationToken.None);

                // Create enriched locatie with address detail

                enrichedLocaties.Add(locatie with
                {
                    Adres = Adres.Create(adresDetails.Straatnaam,
                                         adresDetails.Huisnummer,
                                         adresDetails.Busnummer,
                                         adresDetails.Postcode,
                                         adresDetails.Gemeente,
                                         Adres.België)
                });

            }
            catch (Exception ex)
            {

                // Keep original locatie if enrichment fails
                enrichedLocaties.Add(locatie);
            }
        }

        return  new EnrichedCommand(command, enrichedLocaties.ToArray());

    }
}

// public static class AccountLookupMiddleware
// {
//     // The message *has* to be first in the parameter list
//     // Before or BeforeAsync tells Wolverine this method should be called before the actual action
//     public static async Task<(HandlerContinuation, EnrichedCommand?, OutgoingMessages)> BeforeAsync(
//         CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> command,
//         ILogger logger,
//
//         // This app is using Marten for persistence
// IGrarClient _grarClient,
//         ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> _logger,
//         CancellationToken cancellation)
//     {
//         _logger.LogInformation("Enriching command {CommandType} with GRAR address data",
//                                typeof(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand>).Name);
//
//         var enrichedLocaties = new List<Locatie>();
//
//         foreach (var locatie in command.Command.Locaties)
//         {
//             if (locatie.Adres is not null)
//             {
//                 enrichedLocaties.Add(locatie);
//
//                 continue;
//             }
//
//             try
//             {
//                 var adresDetails = await _grarClient.GetAddressById(locatie.AdresId.Bronwaarde, CancellationToken.None);
//
//                 // Create enriched locatie with address detail
//
//                 enrichedLocaties.Add(locatie with
//                 {
//                     Adres = Adres.Create(adresDetails.Straatnaam,
//                                          adresDetails.Huisnummer,
//                                          adresDetails.Busnummer,
//                                          adresDetails.Postcode,
//                                          adresDetails.Gemeente,
//                                          Adres.België)
//                 });
//
//                 _logger.LogDebug("Enriched locatie {LocatieId} with address {Adres}",
//                                  locatie.LocatieId, $"{adresDetails.Straatnaam} {adresDetails.Huisnummer}");
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogWarning(ex, "Failed to enrich locatie {LocatieId} with AdresId {AdresId}",
//                                    locatie.LocatieId, locatie.AdresId);
//
//                 // Keep original locatie if enrichment fails
//                 enrichedLocaties.Add(locatie);
//             }
//         }
//
//         return (HandlerContinuation.Continue, new EnrichedCommand(command, enrichedLocaties.ToArray()), new OutgoingMessages());
//
//     }
// }

public record EnrichedCommand(CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> Command, Locatie[] Locaties);

