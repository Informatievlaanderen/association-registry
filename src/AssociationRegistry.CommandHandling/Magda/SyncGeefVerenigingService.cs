namespace AssociationRegistry.CommandHandling.Magda;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using AssociationRegistry.Integrations.Magda.Constants;
using AssociationRegistry.Integrations.Magda.Exceptions;
using AssociationRegistry.Integrations.Magda.Extensions;
using AssociationRegistry.Integrations.Magda.GeefOnderneming.Models;
using AssociationRegistry.Integrations.Magda.Models;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Magda.Kbo;
using Integrations.Magda;
using Microsoft.Extensions.Logging;
using ResultNet;

public class SyncGeefVerenigingService : IMagdaSyncGeefVerenigingService
{
    private readonly IMagdaClient _magdaClient;
    private readonly ILogger _logger;

    public SyncGeefVerenigingService(
        IMagdaClient magdaClient,
        ILogger<SyncGeefVerenigingService> logger)
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
                !magdaOnderneming.IsOnderneming() ||
                !magdaOnderneming.IsRechtspersoon())
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            if (!magdaOnderneming.IsActief())
            {
                var inactieveVereniging = new InactieveVereniging()
                {
                    EindDatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Stopzetting?.Datum, Formats.DateOnly),
                };
                return VerenigingVolgensKboResult.InactieveVereniging(inactieveVereniging);
            }

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
