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
using Middleware;
using ResultNet;
using System.Collections.ObjectModel;
using Vereniging.Geotags;
using Wolverine.Marten;

public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler
{
    private readonly IClock _clock;
    private readonly IGeotagsService _geotagsService;
    private readonly ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> _logger;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;
    private readonly IVCodeService _vCodeService;
    private readonly IVerenigingsRepository _verenigingsRepository;

    public RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler(
        IVerenigingsRepository verenigingsRepository,
        IVCodeService vCodeService,
        IMartenOutbox outbox,
        IDocumentSession session,
        IClock clock,
        IGeotagsService geotagsService,
        ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _outbox = outbox;
        _session = session;
        _clock = clock;
        _geotagsService = geotagsService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> message,
        EnrichedLocaties enrichedLocaties,
        PotentialDuplicatesFound potentialDuplicates,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} start");

        if(potentialDuplicates.HasDuplicates)
            return new Result<PotentialDuplicatesFound>(potentialDuplicates, ResultStatus.Failed);

        var command = message.Command;

        var vereniging = await Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
            command,
            _vCodeService,
            _geotagsService,
            _clock);

        var (metAdresId, zonderAdresId) = vereniging.GeefLocatiesMetEnZonderAdresId();

        vereniging.NeemAdresDetailsOver(metAdresId, enrichedLocaties);
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

public record EnrichedLocatie()
{
    public string Naam { get; init; }
    public bool IsPrimair { get; init; }
    public Locatietype Locatietype { get; init; }
    public AdresId? AdresId { get; init; }
    public Adres Adres { get; init; }

    public static EnrichedLocatie FromLocatieWithAdres(Locatie locatie)
        => new()
        {
            Naam = locatie.Naam,
            IsPrimair = locatie.IsPrimair,
            Locatietype = locatie.Locatietype,
            AdresId = null,
            Adres = locatie.Adres!,
        };

    public static EnrichedLocatie FromLocatieWithAdresId(Locatie locatie, Adres adres)
        => new()
        {
            Naam = locatie.Naam,
            IsPrimair = locatie.IsPrimair,
            Locatietype = locatie.Locatietype,
            AdresId = locatie.AdresId,
            Adres = adres,
        };

    public Registratiedata.AdresUitAdressenregister ToAdres()
        => new(
            Adres.Straatnaam,
            Adres.Huisnummer,
            Adres.Busnummer,
            Adres.Postcode,
            Adres.Gemeente.Naam);

    public Registratiedata.AdresId ToAdresId()
        => new(
            AdresId.Adresbron.Code,
            AdresId.Bronwaarde);
}

public class EnrichedLocaties : ReadOnlyCollection<EnrichedLocatie>
{
    public EnrichedLocaties(IList<EnrichedLocatie> list) : base(list)
    {
    }

    public static EnrichedLocaties Empty => new(new List<EnrichedLocatie>());
}
