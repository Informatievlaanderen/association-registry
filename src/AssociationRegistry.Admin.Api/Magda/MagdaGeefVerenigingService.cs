namespace AssociationRegistry.Admin.Api.Magda;

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
using Framework;
using Infrastructure.Extensions;
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

    public async Task<Result<VerenigingVolgensKbo>> GeefVereniging(KboNummer kboNummer, CommandMetadata metadata, CancellationToken cancellationToken)
    {
        try
        {
            var reference = await CreateReference(_magdaCallReferenceRepository, metadata.Initiator, metadata.CorrelationId, kboNummer, cancellationToken);
            var magdaResponse = await _magdaFacade.GeefOnderneming(kboNummer, reference);

            if (MagdaResponseValidator.HasBlokkerendeUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;
            if (magdaOnderneming is null) return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            if (!Rechtsvorm.TryParse(Rechtsvorm.UitMagda, GetActiveRechtsvorm(magdaOnderneming)?.Code.Value, out var rechtsvorm))
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            if (!IsOnderneming(magdaOnderneming) ||
                !IsActief(magdaOnderneming) ||
                !IsRechtspersoon(magdaOnderneming))
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            var naamOndernemingType = GetBestMatchingNaam(magdaOnderneming.Namen.MaatschappelijkeNamen);

            if (naamOndernemingType is null)
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            return VerenigingVolgensKboResult.GeldigeVereniging(
                new VerenigingVolgensKbo
                {
                    KboNummer = KboNummer.Create(kboNummer),
                    Rechtsvorm = rechtsvorm,
                    Naam = naamOndernemingType.Naam,
                    KorteNaam = GetBestMatchingNaam(magdaOnderneming.Namen.AfgekorteNamen)?.Naam,
                    StartDatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Start.Datum, Formats.DateOnly),
                    Adres = GetMaatschappelijkeZetel(magdaOnderneming.Adressen),
                });
        }
        catch (Exception e)
        {
            throw new MagdaException("Er heeft zich een fout voorgedaan bij het aanroepen van de Magda GeefOndernemingDienst.", e);
        }
    }

    private static Result<Adres?> GetMaatschappelijkeZetel(AdresOndernemingType[] adressen)
    {
        var maatschappelijkeZetel = adressen.SingleOrDefault(a => a.Type.Code.Value == AdresCodes.MaatschappelijkeZetel && IsActiveToday(a.DatumBegin, a.DatumEinde));
        if (maatschappelijkeZetel is null)
            return Result.Success<Adres?>(null);
        var adresDetails = GetBestMatchingAdres(maatschappelijkeZetel.Descripties).Adres;
        return TryCreateAddress(adresDetails);
    }

    private static Result<Adres?> TryCreateAddress(AdresOndernemingBasisType adresDetails)
    {
        try
        {
            return Adres.Create(adresDetails.Straat.Naam, adresDetails.Huisnummer, adresDetails.Busnummer, adresDetails.Gemeente.PostCode, adresDetails.Gemeente.Naam, adresDetails.Land.Naam);
        }
        catch
        {
            return Result.Failure<Adres?>();
        }
    }

    private static NaamOndernemingType? GetBestMatchingNaam(NaamOndernemingType[] namen)
    {
        var activeNamen = namen
            .Where(n => IsActiveToday(n.DatumBegin, n.DatumEinde))
            .ToArray();

        if (activeNamen.Length == 0)
            return null;
        if (activeNamen.Length == 1)
            return activeNamen.Single();

        return GetNaamInTaal(activeNamen, TaalCodes.Nederlands) ??
               GetNaamInTaal(activeNamen, TaalCodes.Frans) ??
               GetNaamInTaal(activeNamen, TaalCodes.Duits) ??
               GetNaamInTaal(activeNamen, TaalCodes.Engels) ??
               activeNamen.First();
    }

    private static DescriptieType GetBestMatchingAdres(DescriptieType[] descripties)
    {
        if (descripties.Length == 1)
            return descripties.Single();

        return GetDescriptieInTaal(descripties, TaalCodes.Nederlands) ??
               GetDescriptieInTaal(descripties, TaalCodes.Frans) ??
               GetDescriptieInTaal(descripties, TaalCodes.Duits) ??
               GetDescriptieInTaal(descripties, TaalCodes.Engels) ??
               descripties.First();
    }

    private static NaamOndernemingType? GetNaamInTaal(NaamOndernemingType[] namen, string taalcode)
        => namen.SingleOrDefault(n => n.Taalcode.Equals(taalcode, StringComparison.InvariantCultureIgnoreCase));

    private static DescriptieType? GetDescriptieInTaal(DescriptieType[] descripties, string taalcode)
        => descripties.SingleOrDefault(n => n.Taalcode.Equals(taalcode, StringComparison.InvariantCultureIgnoreCase));

    private static bool IsActiveToday(string datumBegin, string datumEinde)
        => DateOnlyHelper.ParseOrNull(datumBegin, Formats.DateOnly).IsNullOrBeforeToday() &&
           DateOnlyHelper.ParseOrNull(datumEinde, Formats.DateOnly).IsNullOrAfterToday();

    private static RechtsvormExtentieType? GetActiveRechtsvorm(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.Rechtsvormen?.FirstOrDefault(
            r => IsActiveToday(r.DatumBegin, r.DatumEinde));


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

    private static async Task<MagdaCallReference> CreateReference(IMagdaCallReferenceRepository repository, string initiator, Guid correlationId, string opgevraagdOnderwerp, CancellationToken cancellationToken)
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
