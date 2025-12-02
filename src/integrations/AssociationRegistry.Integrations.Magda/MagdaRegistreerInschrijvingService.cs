namespace AssociationRegistry.Integrations.Magda;

using AssociationRegistry.Magda.Kbo;
using DecentraalBeheer.Vereniging;
using Exceptions;
using Extensions;
using Framework;
using Microsoft.Extensions.Logging;
using Models;
using Repertorium.RegistreerInschrijving0201;
using ResultNet;

public class MagdaRegistreerInschrijvingService : IMagdaRegistreerInschrijvingService
{
    private const ResultaatEnumType Geslaagd = ResultaatEnumType.Item1;
    private readonly IMagdaCallReferenceRepository _magdaCallReferenceRepository;
    private readonly IMagdaClient _magdaClient;
    private readonly ILogger<MagdaRegistreerInschrijvingService> _logger;

    public MagdaRegistreerInschrijvingService(
        IMagdaCallReferenceRepository magdaCallReferenceRepository,
        IMagdaClient magdaClient,
        ILogger<MagdaRegistreerInschrijvingService> logger)
    {
        _magdaCallReferenceRepository =
            magdaCallReferenceRepository ?? throw new ArgumentNullException(nameof(magdaCallReferenceRepository));

        _magdaClient = magdaClient ?? throw new ArgumentNullException(nameof(magdaClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result> RegistreerInschrijving(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var reference = await MagdaCallReferenceService.CreateReference(metadata.Initiator,
                                                                            metadata.CorrelationId,
                                                                            kboNummer,
                                                                            cancellationToken, "RegistreerInschrijvingDienst-02.01", "Registreer inschrijving voor vereniging met rechtspersoonlijkheid", "Verenigingsregister Beheer Api");

            _logger.LogInformation(
                $"MAGDA Call Reference - RegistreerInschrijving Service - KBO nummer '{kboNummer}' met referentie '{reference.Reference}'");

            var registreerInschrijvingResponse = await _magdaClient.RegistreerInschrijving(kboNummer, reference);
            var responseBody = registreerInschrijvingResponse?.Body?.RegistreerInschrijvingResponse;

            if (responseBody is null)
            {
                _logger.LogError("Magda RegistreerInschrijvingDienst gaf een lege body terug.");

                return Result.Failure();
            }

            ThrowIfRepliekUitzonderingenFout(responseBody);

            var uitzonderingen = responseBody.Repliek.Antwoorden.Antwoord.Uitzonderingen;

            LogIndienAntwoordUitzonderingen(kboNummer, uitzonderingen);

            return responseBody.Repliek.Antwoorden.Antwoord.Inhoud.Resultaat.Value == Geslaagd
                ? Result.Success()
                : Result.Failure();
        }
        catch (Exception e)
        {
            throw new MagdaException(e);
        }
    }

    private void ThrowIfRepliekUitzonderingenFout(RegistreerInschrijvingResponse response)
    {
        var repliekUitzonderingen = response.Repliek?.Uitzonderingen;

        if (repliekUitzonderingen is not null && repliekUitzonderingen.Any())
        {
            LogUitzonderingenWarnings(repliekUitzonderingen);

            ThrowUitzonderingenFouten(repliekUitzonderingen);
        }
    }

    private void ThrowUitzonderingenFouten(UitzonderingType[] repliekUitzonderingen)
    {
        var uitzonderingFouten = repliekUitzonderingen.Where(w => w.Type == UitzonderingTypeType.FOUT);

        if (uitzonderingFouten.Any())
        {
            throw new MagdaRepliekException(uitzonderingFouten);
        }
    }

    private void LogUitzonderingenWarnings(UitzonderingType[] repliekUitzonderingen)
    {
        foreach (var uitzonderingType in repliekUitzonderingen.Where(w => w.Type == UitzonderingTypeType.WAARSCHUWING))
        {
            _logger.LogWarning("De Magda RegistreerInschrijvingDienst geeft volgende waarschuwing: {Waarschuwing}",
                               uitzonderingType.Diagnose);
        }
    }

    private void LogIndienAntwoordUitzonderingen(KboNummer kboNummer, UitzonderingType[]? uitzonderingen)
    {
        if (uitzonderingen is null || !uitzonderingen.Any())
            return;

        _logger.LogInformation(
            "Uitzondering bij het aanroepen van de Magda GeefOnderneming service voor KBO-nummer {KboNummer}: " +
            "\nFouten:\n'{Uitzonderingen}'" +
            "\nWaarschuwingen:\n'{Waarschuwingen}'" +
            "\nInformatie:\n'{Informatie}'",
            kboNummer,
            uitzonderingen.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.FOUT),
            uitzonderingen.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.WAARSCHUWING),
            uitzonderingen.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.INFORMATIE));
    }
}

public interface IMagdaCallReferenceService
{
    Task<MagdaCallReference> CreateReference(
        string initiator,
        Guid correlationId,
        string opgevraagdOnderwerp,
        CancellationToken cancellationToken,
        string opgevraagdeDienst,
        string context,
        string aanroependeDienst);
};
public class MagdaCallReferenceService : IMagdaCallReferenceService
{
    private readonly IMagdaCallReferenceRepository _repository;

    public MagdaCallReferenceService(IMagdaCallReferenceRepository repository)
    {
        _repository = repository;
    }
    public async Task<MagdaCallReference> CreateReference(
        string initiator,
        Guid correlationId,
        string opgevraagdOnderwerp,
        CancellationToken cancellationToken,
        string opgevraagdeDienst,
        string context,
        string aanroependeDienst)
    {
        var magdaCallReference = new MagdaCallReference
        {
            Reference = Guid.NewGuid(),
            CalledAt = DateTimeOffset.UtcNow,
            Initiator = initiator,
            OpgevraagdeDienst = opgevraagdeDienst,
            Context = context,
            AanroependeDienst = aanroependeDienst,
            CorrelationId = correlationId,
            OpgevraagdOnderwerp = opgevraagdOnderwerp,
        };

        await _repository.Save(magdaCallReference, cancellationToken);

        return magdaCallReference;
    }
}
