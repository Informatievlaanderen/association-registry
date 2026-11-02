namespace AssociationRegistry.Test.Common.DubbelMerge;

using AssociationRegistry.DecentraalBeheer.Vereniging.Erkenningen;
using AssociationRegistry.Events;
using Gegevens = Events.SamenvoegingDubbeleVereniging;

#pragma warning disable CS0618
public class DubbelMergeEventBuilder
{
    private readonly string _vCode;
    private readonly string _vCodeDubbeleVereniging;

    private Gegevens.OvergenomenBasisgegevens _overgenomenBasisgegevens = Gegevens.OvergenomenBasisgegevens.Geen;
    private readonly List<Gegevens.OvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>> _overgenomenHoofdactiviteiten =
    [];
    private readonly List<Gegevens.OvergenomenGegeven<Registratiedata.Werkingsgebied>> _overgenomenWerkingsgebieden =
    [];
    private readonly List<Gegevens.OvergenomenGegeven<Registratiedata.Contactgegeven>> _overgenomenContactgegevens = [];
    private readonly List<Gegevens.OvergenomenGegeven<Registratiedata.Locatie>> _overgenomenLocaties = [];
    private readonly List<Gegevens.OvergenomenGegeven<Registratiedata.Vertegenwoordiger>> _overgenomenVertegenwoordigers =
    [];
    private readonly List<Gegevens.OvergenomenGegeven<Registratiedata.Bankrekeningnummer>> _overgenomenBankrekeningnummers =
    [];
    private readonly List<Gegevens.OvergenomenGegeven<Gegevens.Erkenning>> _overgenomenErkenningen = [];
    private Gegevens.OvergenomenGegeven<Registratiedata.SubverenigingVan>? _overgenomenSubverenigingVan;
    private readonly List<Gegevens.OvergenomenGegeven<Registratiedata.Lidmaatschap>> _overgenomenLidmaatschappen = [];

    private Gegevens.NietOvergenomenBasisgegevens _nietOvergenomenBasisgegevens =
        Gegevens.NietOvergenomenBasisgegevens.Geen;
    private readonly List<Gegevens.NietOvergenomenGegeven<Registratiedata.HoofdactiviteitVerenigingsloket>> _nietOvergenomenHoofdactiviteiten =
    [];
    private readonly List<Gegevens.NietOvergenomenGegeven<Registratiedata.Werkingsgebied>> _nietOvergenomenWerkingsgebieden =
    [];
    private readonly List<Gegevens.NietOvergenomenGegeven<Registratiedata.Contactgegeven>> _nietOvergenomenContactgegevens =
    [];
    private readonly List<Gegevens.NietOvergenomenGegeven<Registratiedata.Locatie>> _nietOvergenomenLocaties = [];
    private readonly List<Gegevens.NietOvergenomenGegeven<Registratiedata.Vertegenwoordiger>> _nietOvergenomenVertegenwoordigers =
    [];
    private readonly List<Gegevens.NietOvergenomenGegeven<Registratiedata.Bankrekeningnummer>> _nietOvergenomenBankrekeningnummers =
    [];
    private readonly List<Gegevens.NietOvergenomenGegeven<Gegevens.Erkenning>> _nietOvergenomenErkenningen = [];
    private Gegevens.NietOvergenomenGegeven<Registratiedata.SubverenigingVan>? _nietOvergenomenSubverenigingVan;
    private readonly List<Gegevens.NietOvergenomenGegeven<Registratiedata.Lidmaatschap>> _nietOvergenomenLidmaatschappen =
    [];

    public DubbelMergeEventBuilder(string vCode, string vCodeDubbeleVereniging)
    {
        _vCode = vCode;
        _vCodeDubbeleVereniging = vCodeDubbeleVereniging;
    }

    public DubbelMergeEventBuilder WithOvergenomenVertegenwoordiger(
        Registratiedata.Vertegenwoordiger originalValue,
        Registratiedata.Vertegenwoordiger newValue
    )
    {
        _overgenomenVertegenwoordigers.Add(
            new(Gegevens.Acties.ToegevoegdMetAanpassing, "Vertegenwoordiger werd toegevoegd.", originalValue, newValue)
        );

        return this;
    }

    public DubbelMergeEventBuilder WithNietOvergenomenVertegenwoordiger(Registratiedata.Vertegenwoordiger value)
    {
        _nietOvergenomenVertegenwoordigers.Add(
            new(Gegevens.Acties.Gededupliceerd, "Bestond al.", "Vertegenwoordiger werd niet opnieuw toegevoegd.", value)
        );

        return this;
    }

    public DubbelMergeEventBuilder WithOvergenomenBankrekeningnummer(
        Registratiedata.Bankrekeningnummer originalValue,
        Registratiedata.Bankrekeningnummer newValue
    )
    {
        _overgenomenBankrekeningnummers.Add(
            new(Gegevens.Acties.Toegevoegd, "Bankrekeningnummer werd toegevoegd.", originalValue, newValue)
        );

        return this;
    }

    public DubbelMergeEventBuilder WithNietOvergenomenBankrekeningnummer(Registratiedata.Bankrekeningnummer value)
    {
        _nietOvergenomenBankrekeningnummers.Add(
            new(
                Gegevens.Acties.Gededupliceerd,
                "Bestond al.",
                "Bankrekeningnummer werd niet opnieuw toegevoegd.",
                value
            )
        );

        return this;
    }

    /// <summary>
    ///     Vult elke groep zonder persoonsgegevens, zowel overgenomen als niet overgenomen, met unieke waarden.
    ///     Zo valt op wanneer een groep vergeten of verkeerd gemapt wordt.
    /// </summary>
    public DubbelMergeEventBuilder WithDataInAllOtherGroups()
    {
        var hoofdactiviteit = new Registratiedata.HoofdactiviteitVerenigingsloket("SPRT", "Sport");
        var werkingsgebied = new Registratiedata.Werkingsgebied("BE10", "Vlaams Gewest");

        _overgenomenBasisgegevens = new Gegevens.OvergenomenBasisgegevens(
            KorteNaam: Verrijkt("Korte naam werd ingevuld.", "Fellowship"),
            KorteBeschrijving: Verrijkt("Korte beschrijving werd ingevuld.", "Een korte beschrijving"),
            Startdatum: Verrijkt("Startdatum werd ingevuld.", new DateOnly(2020, 1, 1)),
            Doelgroep: Verrijkt("Doelgroep werd ingevuld.", new Registratiedata.Doelgroep(12, 18))
        );
        _overgenomenHoofdactiviteiten.Add(
            new(Gegevens.Acties.Toegevoegd, "Hoofdactiviteit werd toegevoegd.", hoofdactiviteit, hoofdactiviteit)
        );
        _overgenomenWerkingsgebieden.Add(
            new(Gegevens.Acties.Toegevoegd, "Werkingsgebied werd toegevoegd.", werkingsgebied, werkingsgebied)
        );
        _overgenomenContactgegevens.Add(
            new(Gegevens.Acties.Toegevoegd, "Website werd toegevoegd.", Website, Website with { ContactgegevenId = 5 })
        );
        _overgenomenLocaties.Add(
            new(
                Gegevens.Acties.ToegevoegdMetAanpassing,
                "Primaire locatie werd toegevoegd als niet-primair.",
                MaatschappelijkeZetel,
                MaatschappelijkeZetel with
                {
                    LocatieId = 7,
                    IsPrimair = false,
                }
            )
        );
        _overgenomenErkenningen.Add(
            new(Gegevens.Acties.Toegevoegd, "Erkenning werd toegevoegd.", Erkenning, Erkenning with { ErkenningId = 9 })
        );
        _overgenomenSubverenigingVan = new(
            Gegevens.Acties.Toegevoegd,
            "Subvereniging van werd toegevoegd.",
            SubverenigingVan,
            SubverenigingVan
        );
        _overgenomenLidmaatschappen.Add(
            new(
                Gegevens.Acties.Toegevoegd,
                "Lidmaatschap werd toegevoegd.",
                Lidmaatschap,
                Lidmaatschap with
                {
                    LidmaatschapId = 8,
                }
            )
        );

        _nietOvergenomenBasisgegevens = new Gegevens.NietOvergenomenBasisgegevens(
            KorteNaam: NietOvergenomen("Authentieke vereniging had al een korte naam.", "Voorbeeld"),
            KorteBeschrijving: NietOvergenomen("Authentieke vereniging had al een korte beschrijving.", "Andere beschrijving"),
            Startdatum: NietOvergenomen("Authentieke vereniging had al een startdatum.", new DateOnly(2019, 5, 5)),
            Doelgroep: NietOvergenomen("Authentieke vereniging had al een doelgroep.", new Registratiedata.Doelgroep(6, 99))
        );
        _nietOvergenomenHoofdactiviteiten.Add(
            new(
                Gegevens.Acties.Gededupliceerd,
                "Bestond al.",
                "Hoofdactiviteit werd niet opnieuw toegevoegd.",
                new Registratiedata.HoofdactiviteitVerenigingsloket("CULT", "Cultuur")
            )
        );
        _nietOvergenomenWerkingsgebieden.Add(
            new(
                Gegevens.Acties.Gededupliceerd,
                "Bestond al.",
                "Werkingsgebied werd niet opnieuw toegevoegd.",
                new Registratiedata.Werkingsgebied("BE24", "Vlaams-Brabant")
            )
        );
        _nietOvergenomenContactgegevens.Add(
            new(Gegevens.Acties.Gededupliceerd, "Bestond al.", "Contactgegeven werd niet opnieuw toegevoegd.", Telefoon)
        );
        _nietOvergenomenLocaties.Add(
            new(
                Gegevens.Acties.Gededupliceerd,
                "Bestond al.",
                "Locatie werd niet opnieuw toegevoegd.",
                Activiteitenlocatie
            )
        );
        _nietOvergenomenErkenningen.Add(
            new(
                Gegevens.Acties.NietOvergenomen,
                "Was nog niet goedgekeurd.",
                "Erkenning werd niet overgenomen.",
                ErkenningWithoutDates
            )
        );
        _nietOvergenomenSubverenigingVan = new(
            Gegevens.Acties.NietOvergenomen,
            "Verwees naar de authentieke vereniging.",
            "Subvereniging van werd niet overgenomen.",
            SubverenigingVan with
            {
                AndereVereniging = _vCode,
                AndereVerenigingNaam = "Authentieke vereniging",
            }
        );
        _nietOvergenomenLidmaatschappen.Add(
            new(
                Gegevens.Acties.Gededupliceerd,
                "Bestond al.",
                "Lidmaatschap werd niet opnieuw toegevoegd.",
                LidmaatschapWithoutDates
            )
        );

        return this;
    }

    public DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging Build() =>
        new(
            _vCode,
            _vCodeDubbeleVereniging,
            new Gegevens.OvergenomenGegevensPerGroep<
                Registratiedata.Vertegenwoordiger,
                Registratiedata.Bankrekeningnummer
            >(
                _overgenomenBasisgegevens,
                [.. _overgenomenHoofdactiviteiten],
                [.. _overgenomenWerkingsgebieden],
                [.. _overgenomenContactgegevens],
                [.. _overgenomenLocaties],
                [.. _overgenomenVertegenwoordigers],
                [.. _overgenomenBankrekeningnummers],
                [.. _overgenomenErkenningen],
                _overgenomenSubverenigingVan,
                [.. _overgenomenLidmaatschappen]
            ),
            new Gegevens.NietOvergenomenGegevensPerGroep<
                Registratiedata.Vertegenwoordiger,
                Registratiedata.Bankrekeningnummer
            >(
                _nietOvergenomenBasisgegevens,
                [.. _nietOvergenomenHoofdactiviteiten],
                [.. _nietOvergenomenWerkingsgebieden],
                [.. _nietOvergenomenContactgegevens],
                [.. _nietOvergenomenLocaties],
                [.. _nietOvergenomenVertegenwoordigers],
                [.. _nietOvergenomenBankrekeningnummers],
                [.. _nietOvergenomenErkenningen],
                _nietOvergenomenSubverenigingVan,
                [.. _nietOvergenomenLidmaatschappen]
            )
        );

    private static Gegevens.OvergenomenGegeven<T> Verrijkt<T>(string description, T value) =>
        new(Gegevens.Acties.Verrijkt, description, value, value);

    private static Gegevens.NietOvergenomenGegeven<T> NietOvergenomen<T>(string reason, T value) =>
        new(Gegevens.Acties.NietOvergenomen, reason, "Werd niet overgenomen.", value);

    private static readonly Registratiedata.Contactgegeven Website = new(
        ContactgegevenId: 1,
        Contactgegeventype: "Website",
        Waarde: "https://www.voorbeeldvereniging.be",
        Beschrijving: "Website van de vereniging",
        IsPrimair: true
    );

    private static readonly Registratiedata.Contactgegeven Telefoon = new(
        ContactgegevenId: 2,
        Contactgegeventype: "Telefoon",
        Waarde: "093334455",
        Beschrijving: "Secretariaat",
        IsPrimair: false
    );

    private static readonly Registratiedata.Locatie MaatschappelijkeZetel = new(
        LocatieId: 1,
        Locatietype: "Maatschappelijke zetel volgens KBO",
        IsPrimair: true,
        Naam: "Zetel",
        Adres: new Registratiedata.Adres("Kerkstraat", "1", "A", "9000", "Gent", "België"),
        AdresId: new Registratiedata.AdresId("AR", "https://data.vlaanderen.be/id/adres/1")
    );

    private static readonly Registratiedata.Locatie Activiteitenlocatie = new(
        LocatieId: 2,
        Locatietype: "Activiteiten",
        IsPrimair: false,
        Naam: "Lokaal",
        Adres: new Registratiedata.Adres("Molenstraat", "22", "", "2000", "Antwerpen", "België"),
        AdresId: new Registratiedata.AdresId("AR", "https://data.vlaanderen.be/id/adres/2")
    );

    private static readonly Gegevens.Erkenning Erkenning = new(
        ErkenningId: 1,
        IpdcProduct: new IpdcProduct { Nummer = "1502", Naam = "Erkenning jeugdwerk" },
        Startdatum: new DateOnly(2024, 1, 1),
        Einddatum: new DateOnly(2028, 12, 31),
        Hernieuwingsdatum: new DateOnly(2028, 6, 30),
        HernieuwingsUrl: "https://example.org/hernieuwing",
        GeregistreerdDoor: new GegevensInitiator { OvoCode = "OVO000101", Naam = "Rivendell" },
        RedenSchorsing: "Lopend onderzoek",
        Status: "Geschorst"
    );

    private static readonly Gegevens.Erkenning ErkenningWithoutDates = new(
        ErkenningId: 2,
        IpdcProduct: new IpdcProduct { Nummer = "1503", Naam = "Erkenning sport" },
        Startdatum: null,
        Einddatum: null,
        Hernieuwingsdatum: null,
        HernieuwingsUrl: "",
        GeregistreerdDoor: new GegevensInitiator { OvoCode = "OVO000102", Naam = "Lothlórien" },
        RedenSchorsing: "",
        Status: "Ingediend"
    );

    private static readonly Registratiedata.SubverenigingVan SubverenigingVan = new(
        AndereVereniging: "V0006003",
        AndereVerenigingNaam: "Koepelvereniging",
        Identificatie: "KOEPEL-1",
        Beschrijving: "Lid van de koepel"
    );

    private static readonly Registratiedata.Lidmaatschap Lidmaatschap = new(
        LidmaatschapId: 1,
        AndereVereniging: "V0006004",
        AndereVerenigingNaam: "Sportfederatie",
        DatumVan: new DateOnly(2023, 1, 1),
        DatumTot: new DateOnly(2025, 12, 31),
        Identificatie: "FED-1",
        Beschrijving: "Aangesloten bij de federatie"
    );

    private static readonly Registratiedata.Lidmaatschap LidmaatschapWithoutDates = new(
        LidmaatschapId: 2,
        AndereVereniging: "V0006005",
        AndereVerenigingNaam: "Cultuurraad",
        DatumVan: null,
        DatumTot: null,
        Identificatie: "",
        Beschrijving: ""
    );
}
#pragma warning restore CS0618
