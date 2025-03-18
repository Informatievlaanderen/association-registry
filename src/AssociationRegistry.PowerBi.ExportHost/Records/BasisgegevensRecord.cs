namespace AssociationRegistry.PowerBi.ExportHost.Records;

using CsvHelper.Configuration.Attributes;

record BasisgegevensRecord(
    [property: Name("bron"), Index(0)] string Bron,
    [property: Name("doelgroep.maximumleeftijd"), Index(1)] int DoelgroepMaxLeeftijd,
    [property: Name("doelgroep.minimumleeftijd"), Index(2)] int DoelgroepMinLeeftijd,
    [property: Name("einddatum"), Index(3)] string EindDatum,
    [property: Name("isUitgeschrevenUitPubliekeDatastroom"), Index(4)] bool IsUitgeschrevenUitPubliekeDatastroom,
    [property: Name("korteBeschrijving"), Index(5)] string KorteBeschrijving,
    [property: Name("korteNaam"), Index(6)] string KorteNaam,
    [property: Name("naam"), Index(7)] string Naam,
    [property: Name("roepnaam"), Index(8)] string RoepNaam,
    [property: Name("startdatum"), Index(9)] string StartDatum,
    [property: Name("status"), Index(10)] string Status,
    [property: Name("vCode"), Index(11)] string VCode,
    [property: Name("verenigingstype.code"), Index(12)] string CodeVerenigingsType,
    [property: Name("verenigingstype.naam"), Index(13)] string NaamVerenigingsType,
    [property: Name("kboNummer"), Index(14)] string KboNummer,
    [property: Name("corresponderendeVCodes"), Index(15)] string CorresponderendeVCodes,
    [property: Name("aantalVertegenwoordigers"), Index(16)] int AantalVertegenwoordigers,
    [property: Name("datumLaatsteAanpassing"), Index(17)] string DatumLaatsteAanpassing,
    [property: Name("verenigingssubtype.code"), Index(18)] string CodeVerenigingssubtype,
    [property: Name("verenigingssubtype.naam"), Index(19)] string NaamVerenigingssubtype,
    [property: Name("subverenigingVan.andereVereniging"), Index(20)] string SubverenigingVanAndereVereniging,
    [property: Name("subverenigingVan.identificatie"), Index(21)] string SubverenigingVanIdentificatie,
    [property: Name("subverenigingVan.beschrijving"), Index(22)] string SubverenigingVanBeschrijving
    );
