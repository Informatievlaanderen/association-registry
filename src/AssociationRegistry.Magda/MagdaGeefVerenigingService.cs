namespace AssociationRegistry.Magda;

using Constants;
using Exceptions;
using Extensions;
using Framework;
using Hosts.Configuration.ConfigurationBindings;
using Kbo;
using Microsoft.Extensions.Logging;
using Models;
using Models.GeefOnderneming;
using Onderneming.GeefOnderneming;
using ResultNet;
using Vereniging;

public class MagdaGeefVerenigingService : IMagdaGeefVerenigingService
{
    protected readonly Dictionary<string, Verenigingstype> rechtsvormMap = new()
    {
        { RechtsvormCodes.VZW, Verenigingstype.VZW },
        { RechtsvormCodes.IVZW, Verenigingstype.IVZW },
        { RechtsvormCodes.PrivateStichting, Verenigingstype.PrivateStichting },
        { RechtsvormCodes.StichtingVanOpenbaarNut, Verenigingstype.StichtingVanOpenbaarNut },
    };

    protected readonly IMagdaCallReferenceRepository _magdaCallReferenceRepository;
    protected readonly IMagdaClient _magdaClient;
    protected readonly TemporaryMagdaVertegenwoordigersSection _temporaryMagdaVertegenwoordigersSection;
    protected readonly ILogger<MagdaGeefVerenigingService> _logger;

    public MagdaGeefVerenigingService(
        IMagdaCallReferenceRepository magdaCallReferenceRepository,
        IMagdaClient magdaClient,
        TemporaryMagdaVertegenwoordigersSection temporaryMagdaVertegenwoordigersSection,
        ILogger<MagdaGeefVerenigingService> logger)
    {
        _magdaCallReferenceRepository = magdaCallReferenceRepository;
        _magdaClient = magdaClient;
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

            _logger.LogInformation($"MAGDA Call Reference - GeefOnderneming Service - KBO nummer '{kboNummer}' met referentie '{reference.Reference}'");

            var magdaResponse = await _magdaClient.GeefOnderneming(kboNummer, reference);

            if (MagdaResponseValidator.HasBlokkerendeUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;

            if (magdaOnderneming is null ||
                !HeeftToegestaneActieveRechtsvorm(magdaOnderneming) ||
                !IsOnderneming(magdaOnderneming) ||
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
                    Naam = naamOndernemingType.Naam ?? string.Empty,
                    KorteNaam = GetBestMatchingNaam(magdaOnderneming.Namen.AfgekorteNamen)?.Naam ?? string.Empty,
                    Startdatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Start.Datum, Formats.DateOnly),
                    EindDatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Stopzetting?.Datum, Formats.DateOnly),
                    IsActief = IsActiefOfInOprichting(magdaOnderneming),
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

    public async Task<Result<VerenigingVolgensKbo>> GeefSyncVereniging(
        KboNummer kboNummer,
        CommandMetadata metadata,
        CancellationToken cancellationToken)
    {
        try
        {
            var reference = await CreateReference(_magdaCallReferenceRepository, metadata.Initiator, metadata.CorrelationId, kboNummer,
                                                  cancellationToken);

            _logger.LogInformation($"MAGDA Call Reference - GeefOnderneming Service - KBO nummer '{kboNummer}' met referentie '{reference.Reference}'");

            var magdaResponse = await _magdaClient.GeefOnderneming(kboNummer, reference);

            if (MagdaResponseValidator.HasBlokkerendeUitzonderingen(magdaResponse))
                return HandleUitzonderingen(kboNummer, magdaResponse);

            var magdaOnderneming = magdaResponse?.Body?.GeefOndernemingResponse?.Repliek.Antwoorden.Antwoord.Inhoud.Onderneming ?? null;

            if (magdaOnderneming is null ||
              //  !HeeftToegestaneActieveRechtsvorm(magdaOnderneming) ||
                !IsOnderneming(magdaOnderneming) ||
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
                    Naam = naamOndernemingType.Naam ?? string.Empty,
                    KorteNaam = GetBestMatchingNaam(magdaOnderneming.Namen.AfgekorteNamen)?.Naam ?? string.Empty,
                    Startdatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Start.Datum, Formats.DateOnly),
                    EindDatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Stopzetting?.Datum, Formats.DateOnly),
                    IsActief = IsActiefOfInOprichting(magdaOnderneming),
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

    protected bool IsActiefOfInOprichting(Onderneming2_0Type magdaOnderneming)
        => IsActief(magdaOnderneming) || IsInOprichting(magdaOnderneming);

    protected VertegenwoordigerVolgensKbo[] GetVertegenwoordigers()
        => _temporaryMagdaVertegenwoordigersSection.TemporaryVertegenwoordigers
                                                   .Select(x => new VertegenwoordigerVolgensKbo
                                                    {
                                                        Insz = x.Insz,
                                                        Voornaam = x.Voornaam,
                                                        Achternaam = x.Achternaam,
                                                    })
                                                   .ToArray();

    protected bool HeeftToegestaneActieveRechtsvorm(Onderneming2_0Type magdaOnderneming)
    {
        var rechtsvormCode = GetActiveRechtsvorm(magdaOnderneming)?.Code.Value;

        return rechtsvormCode != null && rechtsvormMap.ContainsKey(rechtsvormCode);
    }

    protected ContactgegevensVolgensKbo GetContactgegevensFrom(AdresOndernemingType? maatschappelijkeZetel)
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

    protected AdresVolgensKbo GetAdresFrom(AdresOndernemingType? maatschappelijkeZetel)
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

    protected static NaamOndernemingType? GetBestMatchingNaam(NaamOndernemingType[]? namen)
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

    protected static DescriptieType GetBestMatchingAdres(DescriptieType[] descripties)
    {
        if (descripties.Length == 1)
            return descripties.Single();

        return GetDescriptieInTaal(descripties, TaalCodes.Nederlands) ??
               GetDescriptieInTaal(descripties, TaalCodes.Frans) ??
               GetDescriptieInTaal(descripties, TaalCodes.Duits) ??
               GetDescriptieInTaal(descripties, TaalCodes.Engels) ??
               descripties.First();
    }

    protected static NaamOndernemingType? GetNaamInTaal(NaamOndernemingType[] namen, string taalcode)
        => namen.SingleOrDefault(n => n.Taalcode.Equals(taalcode, StringComparison.InvariantCultureIgnoreCase));

    protected static DescriptieType? GetDescriptieInTaal(DescriptieType[] descripties, string taalcode)
        => descripties.SingleOrDefault(n => n.Taalcode.Equals(taalcode, StringComparison.InvariantCultureIgnoreCase));

    protected static bool IsActiveToday(string datumBegin, string datumEinde)
        => DateOnlyHelper.ParseOrNull(datumBegin, Formats.DateOnly).IsNullOrBeforeToday() &&
           DateOnlyHelper.ParseOrNull(datumEinde, Formats.DateOnly).IsNullOrAfterToday();

    protected static RechtsvormExtentieType? GetActiveRechtsvorm(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.Rechtsvormen?.FirstOrDefault(
            r => IsActiveToday(r.DatumBegin, r.DatumEinde));

    protected static bool IsRechtspersoon(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.SoortOnderneming.Code.Value == SoortOndernemingCodes.Rechtspersoon;

    protected static bool IsActief(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.StatusKBO.Code.Value == StatusKBOCodes.Actief;

    protected static bool IsInOprichting(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.StatusKBO.Code.Value == StatusKBOCodes.InOprichting;

    protected static bool IsOnderneming(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.OndernemingOfVestiging.Code.Value == OndernemingOfVestigingCodes.Onderneming;

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
