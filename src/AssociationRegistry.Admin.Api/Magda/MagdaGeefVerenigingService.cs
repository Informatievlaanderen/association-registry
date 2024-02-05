namespace AssociationRegistry.Admin.Api.Magda;

using AssociationRegistry.Magda;
using AssociationRegistry.Magda.Configuration;
using AssociationRegistry.Magda.Constants;
using AssociationRegistry.Magda.Exceptions;
using AssociationRegistry.Magda.Models;
using AssociationRegistry.Magda.Models.GeefOnderneming;
using AssociationRegistry.Magda.Onderneming.GeefOnderneming;
using Framework;
using Infrastructure.Extensions;
using Kbo;
using Microsoft.Extensions.Logging;
using ResultNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vereniging;

public class MagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    private readonly Dictionary<string, Verenigingstype> rechtsvormMap = new()
    {
        { RechtsvormCodes.VZW, Verenigingstype.VZW },
        { RechtsvormCodes.IVZW, Verenigingstype.IVZW },
        { RechtsvormCodes.PrivateStichting, Verenigingstype.PrivateStichting },
        { RechtsvormCodes.StichtingVanOpenbaarNut, Verenigingstype.StichtingVanOpenbaarNut },
    };

    private readonly IMagdaCallReferenceRepository _magdaCallReferenceRepository;
    private readonly IMagdaFacade _magdaFacade;
    private readonly TemporaryMagdaVertegenwoordigersSection _temporaryMagdaVertegenwoordigersSection;
    private readonly ILogger<MagdaGeefVerenigingService> _logger;

    public MagdaGeefVerenigingService(
        IMagdaCallReferenceRepository magdaCallReferenceRepository,
        IMagdaFacade magdaFacade,
        TemporaryMagdaVertegenwoordigersSection temporaryMagdaVertegenwoordigersSection,
        ILogger<MagdaGeefVerenigingService> logger)
    {
        _magdaCallReferenceRepository = magdaCallReferenceRepository;
        _magdaFacade = magdaFacade;
        _temporaryMagdaVertegenwoordigersSection = temporaryMagdaVertegenwoordigersSection;
        _logger = logger;
    }

    public async Task<Result<VerenigingVolgensKbo>> GeefVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var reference = await CreateReference(_magdaCallReferenceRepository, metadata.Initiator, metadata.CorrelationId, kboNummer,
                                                  cancellationToken);

            var magdaResponse = await _magdaFacade.GeefOnderneming(kboNummer, reference);

            if (MagdaResponseValidator.HasBlokkerendeUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;
            if (magdaOnderneming is null ||
                !HeeftToegestaneActieveRechtsvorm(magdaOnderneming) ||
                !IsOnderneming(magdaOnderneming) ||
                !IsActiefOfInOprichting(magdaOnderneming) ||
                !IsRechtspersoon(magdaOnderneming))
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            var naamOndernemingType = GetBestMatchingNaam(magdaOnderneming.Namen.MaatschappelijkeNamen);

            if (naamOndernemingType is null)
                return VerenigingVolgensKboResult.GeenGeldigeVereniging;

            var maatschappelijkeZetel =
                magdaOnderneming.Adressen.SingleOrDefault(a => a.Type.Code.Value == AdresCodes.MaatschappelijkeZetel &&
                                                               IsActiveToday(a.DatumBegin, a.DatumEinde));

            return VerenigingVolgensKboResult.GeldigeVereniging(
                new VerenigingVolgensKbo
                {
                    KboNummer = KboNummer.Create(kboNummer),
                    Type = rechtsvormMap[GetActiveRechtsvorm(magdaOnderneming)!.Code.Value],
                    Naam = naamOndernemingType.Naam,
                    KorteNaam = GetBestMatchingNaam(magdaOnderneming.Namen.AfgekorteNamen)?.Naam,
                    Startdatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Start.Datum, Formats.DateOnly),
                    Adres = GetAdresFrom(maatschappelijkeZetel),
                    Contactgegevens = GetContactgegevensFrom(maatschappelijkeZetel),
                    Vertegenwoordigers = GetVertegenwoordigers(),
                });
        }
        catch (Exception e)
        {
            throw new MagdaException(message: "Er heeft zich een fout voorgedaan bij het aanroepen van de Magda GeefOndernemingDienst.", e);
        }
    }

    private static bool IsActiefOfInOprichting(Onderneming2_0Type magdaOnderneming)
        => IsActief(magdaOnderneming) || IsInOprichting(magdaOnderneming);

    private VertegenwoordigerVolgensKbo[] GetVertegenwoordigers()
        => _temporaryMagdaVertegenwoordigersSection.TemporaryVertegenwoordigers
                                                   .Select(x => new VertegenwoordigerVolgensKbo
                                                    {
                                                        Insz = x.Insz,
                                                        Voornaam = x.Voornaam,
                                                        Achternaam = x.Achternaam,
                                                    })
                                                   .ToArray();

    private bool HeeftToegestaneActieveRechtsvorm(Onderneming2_0Type magdaOnderneming)
    {
        var rechtsvormCode = GetActiveRechtsvorm(magdaOnderneming)?.Code.Value;

        return rechtsvormCode != null && rechtsvormMap.ContainsKey(rechtsvormCode);
    }

    private ContactgegevensVolgensKbo GetContactgegevensFrom(AdresOndernemingType? maatschappelijkeZetel)
    {
        if (maatschappelijkeZetel is null)
            return new ContactgegevensVolgensKbo();

        var contactDetails = GetBestMatchingAdres(maatschappelijkeZetel.Descripties).Contact;

        return new ContactgegevensVolgensKbo
        {
            Email = contactDetails?.Email,
            Telefoonnummer = contactDetails?.Telefoonnummer,
            GSM = contactDetails?.GSM,
            Website = contactDetails?.Website,
        };
    }

    private static AdresVolgensKbo GetAdresFrom(AdresOndernemingType? maatschappelijkeZetel)
    {
        if (maatschappelijkeZetel is null)
            return new AdresVolgensKbo();

        var adresDetails = GetBestMatchingAdres(maatschappelijkeZetel.Descripties).Adres;

        return new AdresVolgensKbo
        {
            Straatnaam = adresDetails?.Straat?.Naam ?? maatschappelijkeZetel.Straat?.Naam,
            Huisnummer = adresDetails?.Huisnummer ?? maatschappelijkeZetel.Huisnummer,
            Busnummer = adresDetails?.Busnummer ?? maatschappelijkeZetel.Busnummer,
            Postcode = adresDetails?.Gemeente?.PostCode ?? maatschappelijkeZetel.Gemeente?.PostCode,
            Gemeente = adresDetails?.Gemeente?.Naam ?? maatschappelijkeZetel.Gemeente?.Naam,
            Land = adresDetails?.Land?.Naam ?? maatschappelijkeZetel.Land?.Naam,
        };
    }

    private static NaamOndernemingType? GetBestMatchingNaam(NaamOndernemingType[]? namen)
    {
        if (namen is null) return null;

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

    private static bool IsInOprichting(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.StatusKBO.Code.Value == StatusKBOCodes.InOprichting;

    private static bool IsOnderneming(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.OndernemingOfVestiging.Code.Value == OndernemingOfVestigingCodes.Onderneming;

    private Result<VerenigingVolgensKbo> HandleUitzonderingen(
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
