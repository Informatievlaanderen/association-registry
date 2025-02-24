﻿namespace AssociationRegistry.DecentraalBeheer.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using DuplicateVerenigingDetection;
using Events;
using Framework;
using Grar.Clients;
using Messages;
using Vereniging;
using Marten;
using Microsoft.Extensions.Logging;
using ResultNet;
using Wolverine.Marten;

public class RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler
{
    private readonly IClock _clock;
    private readonly IGrarClient _grarClient;
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
        ILogger<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler> logger)
    {
        _verenigingsRepository = verenigingsRepository;
        _vCodeService = vCodeService;
        _duplicateVerenigingDetectionService = duplicateVerenigingDetectionService;
        _outbox = outbox;
        _session = session;
        _clock = clock;
        _grarClient = grarClient;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommand> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} start");

        var command = message.Command;

        if (!command.SkipDuplicateDetection)
        {
            var duplicates = (await _duplicateVerenigingDetectionService.GetDuplicates(command.Naam, command.Locaties)).ToList();

            if (duplicates.Any())
                return new Result<PotentialDuplicatesFound>(new PotentialDuplicatesFound(duplicates), ResultStatus.Failed);
        }

        var vCode = await _vCodeService.GetNext();

        var vereniging = Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
            vCode,
            command.Naam,
            command.KorteNaam,
            command.KorteBeschrijving,
            command.Startdatum,
            command.Doelgroep,
            command.IsUitgeschrevenUitPubliekeDatastroom,
            command.Contactgegevens,
            command.Locaties,
            message.Command.Vertegenwoordigers,
            command.HoofdactiviteitenVerenigingsloket,
            command.Werkingsgebieden,
            _clock);

        var toegevoegdeLocaties = vereniging.UncommittedEvents.OfType<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd>()
                                            .Single().Locaties;

        foreach (var teSynchroniserenLocatie in toegevoegdeLocaties)
        {
            await SynchroniseerLocatie(cancellationToken, teSynchroniserenLocatie, vereniging, vCode);
        }

        var result = await _verenigingsRepository.Save(vereniging, _session ,message.Metadata, cancellationToken);

        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} end");

        return Result.Success(CommandResult.Create(vCode, result));
    }

    private async Task SynchroniseerLocatie(
        CancellationToken cancellationToken,
        Registratiedata.Locatie teSynchroniserenLocatie,
        Vereniging vereniging,
        VCode vCode)
    {
        if (teSynchroniserenLocatie.AdresId is not null)
        {
            await vereniging.NeemAdresDetailOver(teSynchroniserenLocatie, _grarClient, cancellationToken);
        }
        else if (teSynchroniserenLocatie.Adres is not null)
        {
            await _outbox.SendAsync(new TeAdresMatchenLocatieMessage(vCode.Value, teSynchroniserenLocatie.LocatieId));
        }
    }
}
