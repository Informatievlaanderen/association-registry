namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;

/// <summary>Erkenningen zoals ze geregistreerd werden; de id is die op de eigen vereniging.</summary>
public static class TestErkenningen
{
    public static readonly GegevensInitiator Rivendell = GegevensInitiator.Hydrate("OVO000101", "Rivendell");
    public static readonly GegevensInitiator Gondor = GegevensInitiator.Hydrate("OVO000202", "Gondor");

    public static readonly IpdcProduct LembasBakken = new() { Nummer = "1001", Naam = "Lembas bakken" };

    public static readonly IpdcProduct RingdragersBegeleiden = new()
    {
        Nummer = "1002",
        Naam = "Ringdragers begeleiden",
    };

    /// <summary>Verlopen erkenning van Rivendell voor Lembas bakken.</summary>
    public static readonly ErkenningWerdGeregistreerd LembasBakkenRivendell = new(
        ErkenningId: 1,
        IpdcProduct: LembasBakken,
        Startdatum: new DateOnly(2020, 1, 1),
        Einddatum: new DateOnly(2021, 12, 31),
        Hernieuwingsdatum: null,
        HernieuwingsUrl: string.Empty,
        GeregistreerdDoor: Rivendell,
        Status: ErkenningStatus.Verlopen.Value
    );

    /// <summary>Verlopen erkenning van Rivendell voor Lembas bakken, die overlapt met <see cref="LembasBakkenRivendell" />.</summary>
    public static readonly ErkenningWerdGeregistreerd OverlappendLembasBakkenRivendell = new(
        ErkenningId: 2,
        IpdcProduct: LembasBakken,
        Startdatum: new DateOnly(2021, 6, 1),
        Einddatum: new DateOnly(2022, 12, 31),
        Hernieuwingsdatum: null,
        HernieuwingsUrl: string.Empty,
        GeregistreerdDoor: Rivendell,
        Status: ErkenningStatus.Verlopen.Value
    );

    /// <summary>Actieve erkenning van Gondor voor Ringdragers begeleiden.</summary>
    public static readonly ErkenningWerdGeregistreerd RingdragersBegeleidenGondor = new(
        ErkenningId: 3,
        IpdcProduct: RingdragersBegeleiden,
        Startdatum: new DateOnly(2024, 1, 1),
        Einddatum: null,
        Hernieuwingsdatum: new DateOnly(2030, 1, 1),
        HernieuwingsUrl: "https://www.minas-tirith.gondor/hernieuwen",
        GeregistreerdDoor: Gondor,
        Status: ErkenningStatus.Actief.Value
    );

    public static SamenvoegingDubbeleVereniging.Erkenning ToSamenvoeging(this ErkenningWerdGeregistreerd erkenning) =>
        new(
            erkenning.ErkenningId,
            erkenning.IpdcProduct,
            erkenning.Startdatum,
            erkenning.Einddatum,
            erkenning.Hernieuwingsdatum,
            erkenning.HernieuwingsUrl,
            erkenning.GeregistreerdDoor,
            RedenSchorsing: string.Empty,
            erkenning.Status
        );
}
