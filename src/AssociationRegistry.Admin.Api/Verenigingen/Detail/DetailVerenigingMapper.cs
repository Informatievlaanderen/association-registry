﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Detail;

using System.Linq;
using Infrastructure.ConfigurationBindings;
using ResponseModels;
using Schema.Detail;
using Vereniging;
using Adres = ResponseModels.Adres;
using AdresId = ResponseModels.AdresId;
using Contactgegeven = ResponseModels.Contactgegeven;
using HoofdactiviteitVerenigingsloket = ResponseModels.HoofdactiviteitVerenigingsloket;
using Locatie = ResponseModels.Locatie;
using Vertegenwoordiger = ResponseModels.Vertegenwoordiger;

public class DetailVerenigingMapper
{
    private readonly AppSettings _appSettings;

    public DetailVerenigingMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public DetailVerenigingResponse Map(BeheerVerenigingDetailDocument vereniging)
        => new()
        {
            Context = $"{_appSettings.BaseUrl}/v1/contexten/detail-vereniging-context.json",
            Vereniging = Map(vereniging, _appSettings.BaseUrl),
            Metadata = MapMetadata(vereniging),
        };

    private static MetadataDetail MapMetadata(BeheerVerenigingDetailDocument vereniging)
    {
        var routeTypePrefix = vereniging.Type.Code == Verenigingstype.VerenigingMetRechtspersoonlijkheid.Code ? "/kbo" : "";
        return new MetadataDetail
        {
            DatumLaatsteAanpassing = vereniging.DatumLaatsteAanpassing,
            BeheerBasisUri = $"/verenigingen{routeTypePrefix}/{vereniging.VCode}",
        };
    }

    private static VerenigingDetail Map(BeheerVerenigingDetailDocument vereniging, string baseUrl)
    {
        return new VerenigingDetail
        {
            VCode = vereniging.VCode,
            Type = Map(vereniging.Type),
            Naam = vereniging.Naam,
            KorteNaam = vereniging.KorteNaam,
            KorteBeschrijving = vereniging.KorteBeschrijving,
            Startdatum = vereniging.Startdatum,
            Status = vereniging.Status,
            IsUitgeschrevenUitPubliekeDatastroom = vereniging.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = vereniging.Contactgegevens.Select(Map).ToArray(),
            Locaties = vereniging.Locaties.Select(Map).ToArray(),
            Vertegenwoordigers = vereniging.Vertegenwoordigers.Select(Map).ToArray(),
            HoofdactiviteitenVerenigingsloket = vereniging.HoofdactiviteitenVerenigingsloket.Select(Map).ToArray(),
            Sleutels = vereniging.Sleutels.Select(Map).ToArray(),
            Relaties = vereniging.Relaties.Select(relatie => Map(relatie, baseUrl)).ToArray(),
        };
    }

    private static Relatie Map(BeheerVerenigingDetailDocument.Relatie relatie, string baseUrl)
        => new()
        {
            Type = relatie.Type,
            AndereVereniging = Map(relatie.AndereVereniging, baseUrl),
        };

    private static GerelateerdeVereniging Map(BeheerVerenigingDetailDocument.Relatie.GerelateerdeVereniging gerelateerdeVereniging, string baseUrl)
        => new()
        {
            KboNummer = gerelateerdeVereniging.KboNummer,
            VCode = gerelateerdeVereniging.VCode,
            Naam = gerelateerdeVereniging.Naam,
            Detail = !string.IsNullOrEmpty(gerelateerdeVereniging.VCode)
                ? $"{baseUrl}/v1/verenigingen/{gerelateerdeVereniging.VCode}"
                : string.Empty,
        };

    private static VerenigingsType Map(BeheerVerenigingDetailDocument.VerenigingsType verenigingsType)
        => new()
        {
            Code = verenigingsType.Code,
            Beschrijving = verenigingsType.Beschrijving,
        };

    private static Sleutel Map(BeheerVerenigingDetailDocument.Sleutel sleutel)
        => new()
        {
            Bron = sleutel.Bron,
            Waarde = sleutel.Waarde,
        };

    private static Contactgegeven Map(BeheerVerenigingDetailDocument.Contactgegeven contactgegeven)
        => new()
        {
            ContactgegevenId = contactgegeven.ContactgegevenId,
            Type = contactgegeven.Type,
            Waarde = contactgegeven.Waarde,
            Beschrijving = contactgegeven.Beschrijving,
            IsPrimair = contactgegeven.IsPrimair,
        };

    private static HoofdactiviteitVerenigingsloket Map(BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket hoofdactiviteitVerenigingsloket)
        => new()
        {
            Code = hoofdactiviteitVerenigingsloket.Code,
            Beschrijving = hoofdactiviteitVerenigingsloket.Beschrijving,
        };

    private static Vertegenwoordiger Map(BeheerVerenigingDetailDocument.Vertegenwoordiger ver)
        => new()
        {
            VertegenwoordigerId = ver.VertegenwoordigerId,
            Voornaam = ver.Voornaam,
            Achternaam = ver.Achternaam,
            Roepnaam = ver.Roepnaam,
            Rol = ver.Rol,
            PrimairContactpersoon = ver.IsPrimair,
            Email = ver.Email,
            Telefoon = ver.Telefoon,
            Mobiel = ver.Mobiel,
            SocialMedia = ver.SocialMedia,
        };

    private static Locatie Map(BeheerVerenigingDetailDocument.Locatie loc)
        => new()
        {
            LocatieId = loc.LocatieId,
            Locatietype = loc.Locatietype,
            Hoofdlocatie = loc.Hoofdlocatie,
            AdresWeergave = loc.AdresWeergave,
            Naam = loc.Naam,
            Adres = new Adres
            {
                Straatnaam = loc.Adres.Straatnaam,
                Huisnummer = loc.Adres.Huisnummer,
                Busnummer = loc.Adres.Busnummer,
                Postcode = loc.Adres.Postcode,
                Gemeente = loc.Adres.Gemeente,
                Land = loc.Adres.Land,
            },
            AdresId = loc.AdresId is not null
                ? new AdresId
                {
                    Broncode = loc.AdresId.Broncode,
                    Bronwaarde = loc.AdresId.Bronwaarde,
                }
                : null,
        };
}
