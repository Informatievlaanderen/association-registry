namespace AssociationRegistry.Integrations.Magda.Onderneming;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using Magda;
using GeefOnderneming;
using Models;
using Persoon;
using AssociationRegistry.Magda.Kbo;
using Microsoft.Extensions.Logging;
using Models.GeefOnderneming;
using ResultNet;
using Shared.Exceptions;
using Shared.Extensions;
using Shared.Models;
using Validation;

public class MagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    private readonly IMagdaClient _magdaClient;
    private readonly ILogger _logger;

    public MagdaGeefVerenigingService(
        IMagdaClient magdaClient,
        ILogger<MagdaGeefVerenigingService> logger)
    {
        _magdaClient = magdaClient;
        _logger = logger;
    }

    public async Task<Result> GeefVereniging(
        KboNummer kboNummer,
        AanroependeFunctie aanroependeFunctie,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var magdaResponse = await _magdaClient.GeefOnderneming(kboNummer, aanroependeFunctie, metadata, cancellationToken);

            if (MagdaResponseValidator.HasBlokkerendeUitzonderingen(magdaResponse))
            {
                LogUitzonderingen(kboNummer, magdaResponse);
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;
            }

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;

            if (magdaOnderneming is null ||
                !magdaOnderneming.HeeftToegestaneActieveRechtsvorm() ||
                !magdaOnderneming.IsOnderneming() ||
                !magdaOnderneming.IsRechtspersoon())
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            var naamOndernemingType = magdaOnderneming.Namen.MaatschappelijkeNamen.GetBestMatchingNaam();

            if (naamOndernemingType is null)
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            return VerenigingVolgensKboResult.GeldigeVereniging(
                magdaOnderneming.MapVerenigingVolgensKbo(kboNummer, naamOndernemingType));
        }
        catch (Exception e)
        {
            throw new MagdaException(message: "Er heeft zich een fout voorgedaan bij het aanroepen van de Magda GeefOndernemingDienst.", e);
        }
    }

    private void LogUitzonderingen(KboNummer kboNummer, ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
    {
        var uitzonderingen = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Uitzonderingen;

        _logger.LogInformation(
            "Uitzondering bij het aanroepen van de Magda GeefOnderneming service voor KBO-nummer {KboNummer}: " +
            "\nFouten:\n'{Uitzonderingen}'" +
            "\nWaarschuwingen:\n'{Waarschuwingen}'" +
            "\nInformatie:\n'{Informatie}'",
            (string)kboNummer,
            uitzonderingen?.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.FOUT),
            uitzonderingen?.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.WAARSCHUWING),
            uitzonderingen?.ConcatenateUitzonderingen(separator: "\n", UitzonderingTypeType.INFORMATIE));
    }
}
