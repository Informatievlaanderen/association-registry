namespace AssociationRegistry.Integrations.Magda;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Magda.Kbo;
using AssociationRegistry.Vereniging;
using Constants;
using Exceptions;
using Extensions;
using Microsoft.Extensions.Logging;
using Models;
using Models.GeefOnderneming;
using Onderneming.GeefOnderneming;
using ResultNet;

public class MagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    protected readonly IMagdaCallReferenceRepository _magdaCallReferenceRepository;
    protected readonly IMagdaClient _magdaClient;
    protected readonly ILogger<MagdaGeefVerenigingService> _logger;

    public MagdaGeefVerenigingService(
        IMagdaCallReferenceRepository magdaCallReferenceRepository,
        IMagdaClient magdaClient,
        ILogger<MagdaGeefVerenigingService> logger)
    {
        _magdaCallReferenceRepository = magdaCallReferenceRepository;
        _magdaClient = magdaClient;
        _logger = logger;
    }

    public async Task<Result> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var reference = await CreateReference(_magdaCallReferenceRepository, metadata.Initiator, metadata.CorrelationId, kboNummer,
                                                  cancellationToken);

            _logger.LogInformation(
                $"MAGDA Call Reference - GeefOnderneming Service - KBO nummer '{kboNummer}' met referentie '{reference.Reference}'");

            var magdaResponse = await _magdaClient.GeefOnderneming(kboNummer, reference);

            if (MagdaResponseValidator.HasBlokkerendeUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;

            if (magdaOnderneming is null ||
                !magdaOnderneming.HeeftToegestaneActieveRechtsvorm() ||
                !magdaOnderneming.IsOnderneming() ||
                !magdaOnderneming.IsRechtspersoon())
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            return VerenigingVolgensKboResult.GeldigeVereniging(magdaOnderneming.MapVerenigingVolgensKbo(kboNummer));
        }
        catch (Exception e)
        {
            throw new MagdaException(message: "Er heeft zich een fout voorgedaan bij het aanroepen van de Magda GeefOndernemingDienst.", e);
        }
    }

    public async Task<Result> GeefSyncVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var reference = await CreateReference(_magdaCallReferenceRepository, metadata.Initiator, metadata.CorrelationId, kboNummer,
                                                  cancellationToken);

            _logger.LogInformation(
                $"MAGDA Call Reference - GeefOnderneming Service - KBO nummer '{kboNummer}' met referentie '{reference.Reference}'");

            var magdaResponse = await _magdaClient.GeefOnderneming(kboNummer, reference);

            if (MagdaResponseValidator.HasBlokkerendeUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;

            if (magdaOnderneming is null ||
                !magdaOnderneming.IsOnderneming() ||
                !magdaOnderneming.IsRechtspersoon())
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            if (!magdaOnderneming.IsActief())
                return VerenigingVolgensKboResult.InactieveVereniging(new InactieveVereniging()
                {
                    EindDatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Stopzetting?.Datum, Formats.DateOnly)
                });

            return VerenigingVolgensKboResult.GeldigeVereniging(magdaOnderneming.MapVerenigingVolgensKbo(kboNummer));
        }
        catch (Exception e)
        {
            throw new MagdaException(message: "Er heeft zich een fout voorgedaan bij het aanroepen van de Magda GeefOndernemingDienst.", e);
        }
    }

    protected virtual Result<VerenigingVolgensKbo> HandleUitzonderingen(
        string kboNummer,
        ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
    {
        var uitzonderingen = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Uitzonderingen;

        _logger.LogInformation(
            "Uitzondering bij het aanroepen van de Magda GeefOnderneming service voor KBO-nummer {KboNummer}: " +
            "\nFouten:\n'{Uitzonderingen}'" +
            "\nWaarschuwingen:\n'{Waarschuwingen}'" +
            "\nInformatie:\n'{Informatie}'",
            kboNummer,
            uitzonderingen?.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.FOUT),
            uitzonderingen?.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.WAARSCHUWING),
            uitzonderingen?.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.INFORMATIE));

        return VerenigingVolgensKboResult.GeenGeldigeVereniging;
    }

    protected virtual async Task<MagdaCallReference> CreateReference(
        IMagdaCallReferenceRepository repository,
        string initiator,
        Guid correlationId,
        string opgevraagdOnderwerp,
        CancellationToken cancellationToken)
    {
        var magdaCallReference = new MagdaCallReference
        {
            Reference = Guid.NewGuid(),
            CalledAt = DateTimeOffset.UtcNow,
            Initiator = initiator,
            OpgevraagdeDienst = "GeefOndernemingDienst-02.00",
            Context = "Registreer vereniging met rechtspersoonlijkheid",
            AanroependeDienst = "Verenigingsregister Beheer Api",
            CorrelationId = correlationId,
            OpgevraagdOnderwerp = opgevraagdOnderwerp,
        };

        await repository.Save(magdaCallReference, cancellationToken);

        return magdaCallReference;
    }
}
