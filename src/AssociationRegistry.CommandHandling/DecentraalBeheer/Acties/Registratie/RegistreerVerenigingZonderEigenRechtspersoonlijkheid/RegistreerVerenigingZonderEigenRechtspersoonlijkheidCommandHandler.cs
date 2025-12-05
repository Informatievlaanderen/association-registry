namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Registratie.RegistreerVerenigingZonderEigenRechtspersoonlijkheid;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Adressen;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Persoon;
using DuplicateVerenigingDetection;
using Locaties.ProbeerAdresTeMatchen;
using Marten;
using Microsoft.Extensions.Logging;
using Middleware;
using ResultNet;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
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
        PersonenUitKsz personenUitKsz,
        CancellationToken cancellationToken = default)
    {
        // Because of the use of middleware on this handler, a SaveChanges() does not send an outbox message
        // To fix this issue temporary, we will enroll the session into the outbox
        // A jira ticket is made to fix this issue: OR-2884
        _outbox.Enroll(_session);

        _logger.LogInformation($"Handle {nameof(RegistreerVerenigingZonderEigenRechtspersoonlijkheidCommandHandler)} start");

        if (personenUitKsz.HeeftOverledenPersonen)
            throw new VerenigingKanNietGeregistreerdWordenMetOverledenVertegenwoordigers();

        if(potentialDuplicates.HasDuplicates)
            return new Result<PotentialDuplicatesFound>(potentialDuplicates, ResultStatus.Failed);

        var command = message.Command;

        var registratieData = new RegistratieDataVerenigingZonderEigenRechtspersoonlijkheid(
            command.Naam,
            command.KorteNaam,
            command.KorteBeschrijving,
            command.Startdatum,
            command.Doelgroep,
            command.IsUitgeschrevenUitPubliekeDatastroom,
            command.Contactgegevens,
            command.Locaties,
            command.Vertegenwoordigers,
            command.HoofdactiviteitenVerenigingsloket,
            command.Werkingsgebieden);

        var vereniging = await Vereniging.RegistreerVerenigingZonderEigenRechtspersoonlijkheid(
            registratieData,
            potentialDuplicates.PotentialDuplicatesSkipped,
            potentialDuplicates.Bevestigingstoken,
            _vCodeService,
            _clock);

        var (metAdresId, zonderAdresId) = vereniging.GeefLocatiesMetEnZonderAdresId();

        vereniging.NeemAdresDetailsOver(metAdresId, verrijkteAdressenUitGrar);
        await vereniging.BerekenGeotags(_geotagsService);

        foreach (var locatieZonderAdresId in zonderAdresId)
        {
            await _outbox.SendAsync(new ProbeerAdresTeMatchenCommand(vereniging.VCode, locatieZonderAdresId.LocatieId));
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
