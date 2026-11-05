namespace AssociationRegistry.Test.E2E.Scenarios.Givens.FeitelijkeVereniging;

using AssociationRegistry.Test.Common.AutoFixture;
using AssociationRegistry.Test.Common.DubbelMerge;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using Events;

/// <summary>
///     Twee verenigingen met vaste gegevens, zoals <see cref="MultipleWerdGeregistreerdScenario" />, aangevuld met
///     lidmaatschappen voor elke regel van de samenvoeging. Lidmaatschappen verwijzen naar andere verenigingen, die pas
///     een VCode krijgen wanneer het scenario loopt; daarom worden ze hier aangemaakt.
///     De authentieke vereniging (AndereFeitelijkeVerenigingWerdGeregistreerd) is subvereniging van Fangorn en lid bij
///     <list type="bullet">
///         <item>Rohan van 2020 tot 2021 (id 1),</item>
///         <item>Gondor vanaf 2024 (id 2),</item>
///         <item>Erebor in 2018-2019 (id 3) en in 2022-2023 (id 4),</item>
///         <item>Lothlórien van 2010 tot 2030 (id 5).</item>
///     </list>
///     De dubbele vereniging (FeitelijkeVerenigingWerdGeregistreerd) is lid bij
///     <list type="bullet">
///         <item>Rohan van midden 2019 tot midden 2020: samengevoegd, de periode wordt 2019-2021,</item>
///         <item>Gondor zonder startdatum tot 2025: samengevoegd, de periode wordt open aan beide kanten,</item>
///         <item>Erebor van 2019 tot 2022: overlapt met twee lidmaatschappen, niet overgenomen,</item>
///         <item>Lothlórien in 2015-2016: valt binnen de bestaande periode, gededupliceerd,</item>
///         <item>Rohan vanaf 2025: overlapt niet, toegevoegd met id 6,</item>
///         <item>Isengard (verwijderd), Moria (dubbel van Minas Tirith), Dale (gestopt), de authentieke vereniging zelf
///         en Fangorn (de hoofdvereniging van de authentieke): niet overgenomen.</item>
///     </list>
/// </summary>
public class DubbelMergeVolledigScenario : MultipleWerdGeregistreerdScenario
{
    public DubbelMergeVolledigScenario(TestVerenigingsgegevens gegevens, TestVerenigingsgegevens andereGegevens)
        : base(gegevens, andereGegevens) { }

    public FeitelijkeVerenigingWerdGeregistreerd Rohan { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd Gondor { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd Erebor { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd Lothlorien { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd Isengard { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd Moria { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd MinasTirith { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd Dale { get; private set; } = null!;
    public FeitelijkeVerenigingWerdGeregistreerd Fangorn { get; private set; } = null!;

    public Registratiedata.Lidmaatschap AuthentiekBijRohan { get; private set; } = null!;
    public Registratiedata.Lidmaatschap AuthentiekBijGondor { get; private set; } = null!;
    public Registratiedata.Lidmaatschap AuthentiekBijEreborEerst { get; private set; } = null!;
    public Registratiedata.Lidmaatschap AuthentiekBijEreborDaarna { get; private set; } = null!;
    public Registratiedata.Lidmaatschap AuthentiekBijLothlorien { get; private set; } = null!;

    public Registratiedata.Lidmaatschap DubbelBijRohan { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijGondor { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijErebor { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijLothlorien { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelLaterBijRohan { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijIsengard { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijMoria { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijDale { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijDeAuthentiekeVereniging { get; private set; } = null!;
    public Registratiedata.Lidmaatschap DubbelBijFangorn { get; private set; } = null!;

    public Registratiedata.Lidmaatschap[] AuthentiekeLidmaatschappen =>
        [
            AuthentiekBijRohan,
            AuthentiekBijGondor,
            AuthentiekBijEreborEerst,
            AuthentiekBijEreborDaarna,
            AuthentiekBijLothlorien,
        ];

    public Registratiedata.Lidmaatschap[] DubbeleLidmaatschappen =>
        [
            DubbelBijRohan,
            DubbelBijGondor,
            DubbelBijErebor,
            DubbelBijLothlorien,
            DubbelLaterBijRohan,
            DubbelBijIsengard,
            DubbelBijMoria,
            DubbelBijDale,
            DubbelBijDeAuthentiekeVereniging,
            DubbelBijFangorn,
        ];

    /// <summary>De lidmaatschappen van de authentieke vereniging na de samenvoeging.</summary>
    public Registratiedata.Lidmaatschap[] LidmaatschappenNaSamenvoeging =>
        [
            AuthentiekBijRohan with
            {
                DatumVan = new DateOnly(2019, 6, 1),
            },
            AuthentiekBijGondor with
            {
                DatumVan = null,
                DatumTot = null,
            },
            AuthentiekBijEreborEerst,
            AuthentiekBijEreborDaarna,
            AuthentiekBijLothlorien,
            DubbelLaterBijRohan with
            {
                LidmaatschapId = 6,
            },
        ];

    public override async Task<KeyValuePair<string, IEvent[]>[]> GivenEvents(IVCodeService service)
    {
        var verenigingen = await base.GivenEvents(service);
        var fixture = new Fixture().CustomizeAdminApi();

        async Task<FeitelijkeVerenigingWerdGeregistreerd> Registreer(string naam) =>
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = await service.GetNext(),
                Naam = naam,
            };

        Rohan = await Registreer("Rohan");
        Gondor = await Registreer("Gondor");
        Erebor = await Registreer("Erebor");
        Lothlorien = await Registreer("Lothlórien");
        Isengard = await Registreer("Isengard");
        Moria = await Registreer("Moria");
        MinasTirith = await Registreer("Minas Tirith");
        Dale = await Registreer("Dale");
        Fangorn = await Registreer("Fangorn");

        var authentieke = AndereFeitelijkeVerenigingWerdGeregistreerd;
        var dubbele = FeitelijkeVerenigingWerdGeregistreerd;

        AuthentiekBijRohan = Lid(1, Rohan, new DateOnly(2020, 1, 1), new DateOnly(2021, 12, 31)) with
        {
            Identificatie = "ROHAN-AUTHENTIEK",
            Beschrijving = "Ruiters van Rohan",
        };
        AuthentiekBijGondor = Lid(2, Gondor, new DateOnly(2024, 1, 1), null);
        AuthentiekBijEreborEerst = Lid(3, Erebor, new DateOnly(2018, 1, 1), new DateOnly(2019, 12, 31));
        AuthentiekBijEreborDaarna = Lid(4, Erebor, new DateOnly(2022, 1, 1), new DateOnly(2023, 12, 31));
        AuthentiekBijLothlorien = Lid(5, Lothlorien, new DateOnly(2010, 1, 1), new DateOnly(2030, 12, 31));

        DubbelBijRohan = Lid(1, Rohan, new DateOnly(2019, 6, 1), new DateOnly(2020, 6, 30)) with
        {
            Identificatie = "ROHAN-DUBBEL",
            Beschrijving = "Andere beschrijving",
        };
        DubbelBijGondor = Lid(2, Gondor, null, new DateOnly(2025, 12, 31));
        DubbelBijErebor = Lid(3, Erebor, new DateOnly(2019, 1, 1), new DateOnly(2022, 6, 30));
        DubbelBijLothlorien = Lid(4, Lothlorien, new DateOnly(2015, 1, 1), new DateOnly(2016, 12, 31));
        DubbelLaterBijRohan = Lid(5, Rohan, new DateOnly(2025, 1, 1), null);
        DubbelBijIsengard = Lid(6, Isengard, null, null);
        DubbelBijMoria = Lid(7, Moria, null, null);
        DubbelBijDale = Lid(8, Dale, null, null);
        DubbelBijDeAuthentiekeVereniging = Lid(9, authentieke, null, null);
        DubbelBijFangorn = Lid(10, Fangorn, null, null);

        return
        [
            .. verenigingen.Select(x =>
                x.Key == authentieke.VCode ? new(x.Key, [.. x.Value, .. AuthentiekeEvents(authentieke.VCode)])
                : x.Key == dubbele.VCode
                    ? new(x.Key, [.. x.Value, .. LidmaatschapEvents(dubbele.VCode, DubbeleLidmaatschappen)])
                : x
            ),
            Stream(Rohan),
            Stream(Gondor),
            Stream(Erebor),
            Stream(Lothlorien),
            Stream(Isengard, new VerenigingWerdVerwijderd(Isengard.VCode, "Vernield door de Ents")),
            Stream(Moria, new VerenigingWerdGemarkeerdAlsDubbelVan(Moria.VCode, MinasTirith.VCode)),
            Stream(MinasTirith, new VerenigingAanvaarddeDubbeleVereniging(MinasTirith.VCode, Moria.VCode)),
            Stream(Dale, new VerenigingWerdGestopt(new DateOnly(2024, 12, 31))),
            Stream(Fangorn),
        ];
    }

    private IEnumerable<IEvent> AuthentiekeEvents(string vCode) =>
        [
            new VerenigingssubtypeWerdVerfijndNaarSubvereniging(
                vCode,
                new Registratiedata.SubverenigingVan(Fangorn.VCode, Fangorn.Naam, "ENT-1", "Hoeders van het woud")
            ),
            .. LidmaatschapEvents(vCode, AuthentiekeLidmaatschappen),
        ];

    private static IEnumerable<IEvent> LidmaatschapEvents(
        string vCode,
        Registratiedata.Lidmaatschap[] lidmaatschappen
    ) => lidmaatschappen.Select(x => new LidmaatschapWerdToegevoegd(vCode, x));

    private static KeyValuePair<string, IEvent[]> Stream(
        FeitelijkeVerenigingWerdGeregistreerd vereniging,
        params IEvent[] events
    ) => new(vereniging.VCode, [vereniging, .. events]);

    private static Registratiedata.Lidmaatschap Lid(
        int id,
        FeitelijkeVerenigingWerdGeregistreerd andereVereniging,
        DateOnly? van,
        DateOnly? tot
    ) => new(id, andereVereniging.VCode, andereVereniging.Naam, van, tot, Identificatie: "", Beschrijving: "");
}
