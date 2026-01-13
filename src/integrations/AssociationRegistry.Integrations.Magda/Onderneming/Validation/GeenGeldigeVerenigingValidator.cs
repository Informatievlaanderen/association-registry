namespace AssociationRegistry.Integrations.Magda.Onderneming.Validation;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Integrations.Magda.Onderneming.GeefOnderneming;
using AssociationRegistry.Integrations.Magda.Shared.Constants;
using AssociationRegistry.Integrations.Magda.Shared.Extensions;
using AssociationRegistry.Magda.Kbo;

public static class MagdaOndernemingExtensions
{
    public static readonly Dictionary<string, Verenigingstype> RechtsvormMap = new()
    {
        { RechtsvormCodes.VZW, Verenigingstype.VZW },
        { RechtsvormCodes.IVZW, Verenigingstype.IVZW },
        { RechtsvormCodes.PrivateStichting, Verenigingstype.PrivateStichting },
        { RechtsvormCodes.StichtingVanOpenbaarNut, Verenigingstype.StichtingVanOpenbaarNut },
    };


    public static bool IsOnderneming(this Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.OndernemingOfVestiging.Code.Value == OndernemingOfVestigingCodes.Onderneming;

    public static bool IsRechtspersoon(this Onderneming2_0Type? magdaOnderneming)
        => magdaOnderneming.SoortOnderneming.Code.Value == SoortOndernemingCodes.Rechtspersoon;

    public static bool HeeftToegestaneActieveRechtsvorm(this Onderneming2_0Type? magdaOnderneming)
    {
        var rechtsvormCode = GetActiveRechtsvorm(magdaOnderneming)?.Code.Value;

        return rechtsvormCode != null && RechtsvormMap.ContainsKey(rechtsvormCode);
    }

    public static Verenigingstype MapRechtsvorm(this Onderneming2_0Type? magdaOnderneming)
        => RechtsvormMap[GetActiveRechtsvorm(magdaOnderneming)!.Code.Value];

    public static RechtsvormExtentieType? GetActiveRechtsvorm(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.Rechtsvormen?.FirstOrDefault(
            r => IsActiveToday(r.DatumBegin, r.DatumEinde));

    public static bool IsActief(this Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.StatusKBO.Code.Value == StatusKBOCodes.Actief;
    public static bool IsActiveToday(string datumBegin, string datumEinde)
        => DateOnlyHelper.ParseOrNull(datumBegin, Formats.DateOnly).IsNullOrBeforeToday() &&
           DateOnlyHelper.ParseOrNull(datumEinde, Formats.DateOnly).IsNullOrAfterToday();

    public static VerenigingVolgensKbo MapVerenigingVolgensKbo(
        this Onderneming2_0Type magdaOnderneming,
        KboNummer kboNummer,
        NaamOndernemingType naamOndernemingType)
    {
        var maatschappelijkeZetel =
            magdaOnderneming.Adressen.SingleOrDefault(a => a.Type.Code.Value == AdresCodes.MaatschappelijkeZetel &&
                                                           IsActiveToday(a.DatumBegin, a.DatumEinde));
        return new VerenigingVolgensKbo
        {
            KboNummer = KboNummer.Create(kboNummer),
            Type = magdaOnderneming.MapRechtsvorm(),
            Naam = naamOndernemingType.Naam ?? string.Empty,
            KorteNaam = GetBestMatchingNaam(magdaOnderneming.Namen.AfgekorteNamen)?.Naam ?? string.Empty,
            Startdatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Start.Datum, Formats.DateOnly),
            EindDatum = DateOnlyHelper.ParseOrNull(magdaOnderneming.Stopzetting?.Datum, Formats.DateOnly),
            IsActief = IsActiefOfInOprichting(magdaOnderneming),
            Adres = GetAdresFrom(maatschappelijkeZetel),
            Contactgegevens = GetContactgegevensFrom(maatschappelijkeZetel),
            Vertegenwoordigers = GetVertegenwoordigers(magdaOnderneming.Functies),
        };
    }

    public static ContactgegevensVolgensKbo GetContactgegevensFrom(AdresOndernemingType? maatschappelijkeZetel)
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

    public static DescriptieType GetBestMatchingAdres(DescriptieType[] descripties)
    {
        if (descripties.Length == 1)
            return descripties.Single();

        return GetDescriptieInTaal(descripties, TaalCodes.Nederlands) ??
               GetDescriptieInTaal(descripties, TaalCodes.Frans) ??
               GetDescriptieInTaal(descripties, TaalCodes.Duits) ??
               GetDescriptieInTaal(descripties, TaalCodes.Engels) ??
               descripties.First();
    }

    public static DescriptieType? GetDescriptieInTaal(DescriptieType[] descripties, string taalcode)
        => descripties.SingleOrDefault(n => n.Taalcode.Equals(taalcode, StringComparison.InvariantCultureIgnoreCase));

    public static AdresVolgensKbo GetAdresFrom(AdresOndernemingType? maatschappelijkeZetel)
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


    public static bool IsActiefOfInOprichting(Onderneming2_0Type magdaOnderneming)
        => IsActief(magdaOnderneming) || IsInOprichting(magdaOnderneming);

    public static VertegenwoordigerVolgensKbo[] GetVertegenwoordigers(FunctieType[] functies)
        => functies is null ? [] : functies.Select(x => new VertegenwoordigerVolgensKbo()
        {
            Insz = x.Persoon.INSZ,
            Voornaam = x.Persoon.VoorNaam,
            Achternaam = x.Persoon.AchterNaam,
        }).ToArray();


    public static bool IsInOprichting(Onderneming2_0Type magdaOnderneming)
        => magdaOnderneming.StatusKBO.Code.Value == StatusKBOCodes.InOprichting;

    public static NaamOndernemingType? GetBestMatchingNaam(this NaamOndernemingType[]? namen)
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

    public static NaamOndernemingType? GetNaamInTaal(NaamOndernemingType[] namen, string taalcode)
        => namen.SingleOrDefault(n => n.Taalcode.Equals(taalcode, StringComparison.InvariantCultureIgnoreCase));
}


