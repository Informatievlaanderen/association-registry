﻿namespace AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using DuplicateVerenigingDetection;
using Framework;
using Marten;
using Messages;
using Microsoft.Extensions.Logging;
using ResultNet;
using System.Collections.ObjectModel;
using Vereniging;
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
        VerrijkteAdressenUitGrar verrijkteAdressenUitGrar,
        PotentialDuplicatesFound potentialDuplicates,
        CancellationToken cancellationToken = default)
    {
        // Because of the use of middleware on this handler, a SaveChanges() does not send an outbox message
        // To fix this issue temporary, we will enroll the session into the outbox
        // A jira ticket is made to fix this issue: OR-2884
        _outbox.Enroll(_session);

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

        vereniging.NeemAdresDetailsOver(metAdresId, verrijkteAdressenUitGrar);
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

public class VerrijkteAdressenUitGrar : ReadOnlyDictionary<string, Adres>
{
    public VerrijkteAdressenUitGrar(IDictionary<string, Adres> adresWithBronwaarde) : base(adresWithBronwaarde)
    {
    }

    public static VerrijkteAdressenUitGrar Empty => new(new Dictionary<string, Adres>());

    public Adres For(AdresId adresId) => this[adresId.Bronwaarde];
}
