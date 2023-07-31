﻿namespace AssociationRegistry.Admin.Api.Magda;

using System;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Kbo;
using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Exceptions;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using Microsoft.Extensions.Logging;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    private readonly IMagdaCallReferenceRepository _magdaCallReferenceRepository;
    private readonly IMagdaFacade _magdaFacade;
    private readonly ILogger<MagdaGeefVerenigingService> _logger;

    public MagdaGeefVerenigingService(
        IMagdaCallReferenceRepository magdaCallReferenceRepository,
        IMagdaFacade magdaFacade,
        ILogger<MagdaGeefVerenigingService> logger)
    {
        _magdaCallReferenceRepository = magdaCallReferenceRepository;
        _magdaFacade = magdaFacade;
        _logger = logger;
    }

    public async Task<Result<VerenigingVolgensKbo>> GeefVereniging(KboNummer kboNummer, string initiator, CancellationToken cancellationToken)
    {
        try
        {
            var reference = await CreateReference(_magdaCallReferenceRepository, initiator, cancellationToken);
            var magdaResponse = await _magdaFacade.GeefOnderneming(kboNummer, reference);

            if (HasFoutUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;
            if (magdaOnderneming is null) return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            if (!Rechtsvorm.TryParse(Rechtsvorm.UitMagda, GetActiveRechtsvorm(magdaOnderneming)?.Code.Value, out var rechtsvorm))
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            if (!IsOnderneming(magdaOnderneming) ||
                !IsActief(magdaOnderneming) ||
                !IsRechtspersoon(magdaOnderneming))
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            return VerenigingVolgensKboResult.GeldigeVereniging(
                new VerenigingVolgensKbo
                {
                    KboNummer = KboNummer.Create(kboNummer),
                    Rechtsvorm = rechtsvorm,
                });
        }
        catch (Exception e)
        {
            throw new MagdaException("Something went Wrong with magda. Look at inner exception for moer details", e);
        }
    }

    private static RechtsvormExtentieType? GetActiveRechtsvorm(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.Rechtsvormen.FirstOrDefault(
            r =>
                string.IsNullOrWhiteSpace(r.DatumEinde) ||
                DateTime.ParseExact(r.DatumEinde, "yyyy-MM-dd", null) >= DateTime.Today);

    private static bool IsRechtspersoon(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.SoortOnderneming.Code.Value == SoortOndernemingCodes.Rechtspersoon;

    private static bool IsActief(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.StatusKBO.Code.Value == StatusKBOCodes.Actief;

    private static bool IsOnderneming(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.OndernemingOfVestiging.Code.Value == OndernemingOfVestigingCodes.Onderneming;

    private Result<VerenigingVolgensKbo> HandleUitzonderingen(string kboNummer, ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
    {
        _logger.LogInformation(
            "Uitzondering bij het aanroepen van de Magda GeefOnderneming service voor KBO-nummer {KboNummer}: '{Diagnose}'",
            kboNummer,
            magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Uitzonderingen.ConcatenateUitzonderingen("\n"));

        return VerenigingVolgensKboResult.GeenGeldigeVereniging;
    }

    private static bool HasFoutUitzonderingen(ResponseEnvelope<GeefOndernemingResponseBody>? magdaResponse)
        => magdaResponse.HasUitzonderingenOfTypes(UitzonderingTypeType.FOUT);

    private static async Task<MagdaCallReference> CreateReference(IMagdaCallReferenceRepository repository, string initiator, CancellationToken cancellationToken)
    {
        var magdaCallReference = new MagdaCallReference
        {
            Reference = Guid.NewGuid(),
            CalledAt = DateTimeOffset.UtcNow,
            Initiator = initiator,
        };
        await repository.Save(magdaCallReference, cancellationToken);

        return magdaCallReference;
    }
}
