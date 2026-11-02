namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Events;

/// <summary>
///     Locaties voor de samenvoeging van een dubbele vereniging. De ids zijn die op de eigen vereniging.
///     Het lokaal van Fellowship 'Rivendell' is hetzelfde adres als dat van Rivendell, anders geschreven.
/// </summary>
public static class TestLocaties
{
    public static readonly Registratiedata.Locatie LokaalRivendell = new(
        LocatieId: 1,
        Locatietype: Locatietype.Activiteiten.Waarde,
        IsPrimair: true,
        Naam: "Rivendell",
        Adres: new Registratiedata.Adres("Kerkstraat", "1", "", "9000", "Gent", "België"),
        AdresId: null
    );

    public static readonly Registratiedata.Locatie PostadresRivendell = new(
        LocatieId: 2,
        Locatietype: Locatietype.Correspondentie.Waarde,
        IsPrimair: false,
        Naam: "Postadres",
        Adres: new Registratiedata.Adres("Stationsstraat", "10", "", "9000", "Gent", "België"),
        AdresId: null
    );

    public static readonly Registratiedata.Locatie LokaalFellowship = new(
        LocatieId: 1,
        Locatietype: Locatietype.Activiteiten.Waarde,
        IsPrimair: true,
        Naam: "Bag End",
        Adres: new Registratiedata.Adres("Molenstraat", "22", "", "2000", "Antwerpen", "België"),
        AdresId: null
    );

    public static readonly Registratiedata.Locatie RivendellBijFellowship = new(
        LocatieId: 2,
        Locatietype: Locatietype.Activiteiten.Waarde,
        IsPrimair: false,
        Naam: "Rivendell",
        Adres: new Registratiedata.Adres("kerk-straat", "1", "", "9000", "GENT", "Belgie"),
        AdresId: null
    );

    public static readonly Registratiedata.Locatie PostadresFellowship = new(
        LocatieId: 3,
        Locatietype: Locatietype.Correspondentie.Waarde,
        IsPrimair: false,
        Naam: "Postadres",
        Adres: new Registratiedata.Adres("Bergstraat", "5", "", "2000", "Antwerpen", "België"),
        AdresId: null
    );

    public static readonly Registratiedata.Locatie OnvolledigeLocatieFellowship = new(
        LocatieId: 4,
        Locatietype: Locatietype.Activiteiten.Waarde,
        IsPrimair: false,
        Naam: "Mordor",
        Adres: new Registratiedata.Adres("", "1", "", "2000", "Antwerpen", "België"),
        AdresId: null
    );

    public static readonly Registratiedata.Locatie TweedeLokaalFellowship = new(
        LocatieId: 5,
        Locatietype: Locatietype.Activiteiten.Waarde,
        IsPrimair: false,
        Naam: "The Prancing Pony",
        Adres: new Registratiedata.Adres("Marktplein", "3", "", "3000", "Leuven", "België"),
        AdresId: null
    );
}

public static class TestLocatiesExtensions
{
    public static FeitelijkeVerenigingWerdGeregistreerd WithLocaties(
        this FeitelijkeVerenigingWerdGeregistreerd @event,
        Registratiedata.Locatie[] locaties
    ) => @event with { Locaties = locaties };

    public static VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd WithLocaties(
        this VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event,
        Registratiedata.Locatie[] locaties
    ) => @event with { Locaties = locaties };
}
