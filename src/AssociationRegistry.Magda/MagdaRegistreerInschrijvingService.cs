namespace AssociationRegistry.Magda;

using Framework;
using Kbo;
using Exceptions;
using Extensions;
using Models;
using Repertorium.RegistreerInschrijving;
using Vereniging;
using Microsoft.Extensions.Logging;
using ResultNet;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        _magdaCallReferenceRepository = magdaCallReferenceRepository ?? throw new ArgumentNullException(nameof(magdaCallReferenceRepository));
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
            var reference = await CreateReference(
                _magdaCallReferenceRepository,
                metadata.Initiator,
                metadata.CorrelationId,
                kboNummer,
                cancellationToken);

            _logger.LogInformation($"MAGDA Call Reference - RegistreerInschrijving Service - KBO nummer '{kboNummer}' met referentie '{reference.Reference}'");

            var response = await _magdaClient.RegistreerInschrijving(kboNummer, reference);

            var uitzonderingen = response?.Body?.RegistreerInschrijvingResponse?.Repliek.Antwoorden.Antwoord.Uitzonderingen;

            LogIndienUitzonderingen(kboNummer, uitzonderingen);

            return response?.Body?.RegistreerInschrijvingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Resultaat.Value == Geslaagd ? Result.Success() : Result.Failure();
        }
        catch (Exception e)
        {
            throw new MagdaException(
                message: "Er heeft zich een fout voorgedaan bij het aanroepen van de Magda RegistreerInschrijvingDienst.", e);
        }
    }

    private void LogIndienUitzonderingen(KboNummer kboNummer, UitzonderingType[]? uitzonderingen)
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

    private static async Task<MagdaCallReference> CreateReference(
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
            OpgevraagdeDienst = "RegistreerInschrijvingDienst-02.01",
            Context = "Registreer inschrijving voor vereniging met rechtspersoonlijkheid",
            AanroependeDienst = "Verenigingsregister Beheer Api",
            CorrelationId = correlationId,
            OpgevraagdOnderwerp = opgevraagdOnderwerp,
        };

        await repository.Save(magdaCallReference, cancellationToken);

        return magdaCallReference;
    }
}
