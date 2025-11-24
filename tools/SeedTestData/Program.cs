using AssociationRegistry.Events;
using AssociationRegistry.MartenDb.Setup;
using JasperFx.Events;
using Marten;
using NodaTime;

Console.WriteLine("🌱 Seeding test data for verenigingsregister...");

var connectionString = Environment.GetEnvironmentVariable("ConnectionString")
    ?? "Host=localhost;Port=5432;Database=golden_master_template;Username=root;Password=root";

Console.WriteLine($"Connecting to: {connectionString.Replace("Password=root", "Password=***")}");

var store = DocumentStore.For(options =>
{
    options.Connection(connectionString);
    options.Events.StreamIdentity = StreamIdentity.AsString;
    options.Events.MetadataConfig.EnableAll();
    options.Events.AppendMode = EventAppendMode.Quick;
    options.RegisterAllEventTypes();
});

await using var session = store.LightweightSession();

// Set metadata headers for all events in this session
session.SetHeader("Initiator", "SeedTestData");
session.SetHeader("Tijdstip", Instant.FromDateTimeUtc(DateTime.UtcNow).ToString());
session.CorrelationId = Guid.NewGuid().ToString();

// Scenario 1: FeitelijkeVereniging with Vertegenwoordiger lifecycle (Toegevoegd -> Gewijzigd -> Verwijderd)
Console.WriteLine("\n📝 Creating FeitelijkeVereniging with vertegenwoordiger lifecycle...");

session.Events.Append(
    "V9000001",
    new FeitelijkeVerenigingWerdGeregistreerd(
        VCode: "V9000001",
        Naam: "The Shire Preservation Society",
        KorteNaam: "",
        KorteBeschrijving: "",
        Startdatum: null,
        Doelgroep: new Registratiedata.Doelgroep(0, 150),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Contactgegevens: [],
        Locaties: [],
        Vertegenwoordigers: [],
        HoofdactiviteitenVerenigingsloket: []
    )
);

session.Events.Append(
    "V9000001",
    new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
        VCode: "V9000001"
    )
);

session.Events.Append(
    "V9000001",
    new VertegenwoordigerWerdToegevoegd(
        VertegenwoordigerId: 1,
        Insz: "12345678901",
        IsPrimair: true,
        Roepnaam: "Frodo",
        Rol: "Ring Bearer",
        Voornaam: "Frodo",
        Achternaam: "Baggins",
        Email: "frodo.baggins@shire.example",
        Telefoon: "0123456789",
        Mobiel: "0412345678",
        SocialMedia: "https://social.example/ringbearer"
    )
);

session.Events.Append(
    "V9000001",
    new VertegenwoordigerWerdGewijzigd(
        VertegenwoordigerId: 1,
        IsPrimair: false,
        Roepnaam: "Frodo",
        Rol: "Secretary",
        Voornaam: "Frodo",
        Achternaam: "Baggins",
        Email: "frodo@shire.example",
        Telefoon: "0123456789",
        Mobiel: "0498765432",
        SocialMedia: "https://social.example/frodobaggins"
    )
);

session.Events.Append(
    "V9000001",
    new VertegenwoordigerWerdVerwijderd(
        VertegenwoordigerId: 1,
        Insz: "12345678901",
        Voornaam: "Frodo",
        Achternaam: "Baggins"
    )
);

// Scenario 2: FeitelijkeVereniging registered WITH vertegenwoordigers in array
Console.WriteLine("📝 Creating FeitelijkeVereniging with vertegenwoordigers in registration...");

session.Events.Append(
    "V9000002",
    new FeitelijkeVerenigingWerdGeregistreerd(
        VCode: "V9000002",
        Naam: "The Fellowship of the Ring",
        KorteNaam: "",
        KorteBeschrijving: "",
        Startdatum: null,
        Doelgroep: new Registratiedata.Doelgroep(0, 150),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Contactgegevens: [],
        Locaties: [],
        Vertegenwoordigers: [
            new Registratiedata.Vertegenwoordiger(
                VertegenwoordigerId: 1,
                Insz: "98765432109",
                IsPrimair: true,
                Roepnaam: "Gandalf",
                Rol: "Wizard",
                Voornaam: "Gandalf",
                Achternaam: "the Grey",
                Email: "gandalf.grey@middleearth.example",
                Telefoon: "0298765432",
                Mobiel: "0487654321",
                SocialMedia: "https://wizards.example/gandalf"
            ),
            new Registratiedata.Vertegenwoordiger(
                VertegenwoordigerId: 2,
                Insz: "11223344556",
                IsPrimair: false,
                Roepnaam: "Sam",
                Rol: "Gardener",
                Voornaam: "Samwise",
                Achternaam: "Gamgee",
                Email: "sam@shire.example",
                Telefoon: "0211223344",
                Mobiel: "0411223344",
                SocialMedia: ""
            )
        ],
        HoofdactiviteitenVerenigingsloket: []
    )
);

session.Events.Append(
    "V9000002",
    new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
        VCode: "V9000002"
    )
);

// Scenario 3: VerenigingMetRechtspersoonlijkheid with KBO vertegenwoordigers
Console.WriteLine("📝 Creating VerenigingMetRechtspersoonlijkheid with KBO vertegenwoordigers...");

session.Events.Append(
    "V9000003",
    new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
        VCode: "V9000003",
        Naam: "Guardians of Minas Tirith",
        KorteNaam: "Minas Tirith",
        Startdatum: DateOnly.Parse("2020-01-01"),
        Rechtsvorm: "VZW",
        KboNummer: "0123456789"
    )
);

session.Events.Append(
    "V9000003",
    new VertegenwoordigerWerdOvergenomenUitKBO(
        VertegenwoordigerId: 1,
        Insz: "55667788990",
        Voornaam: "Aragorn",
        Achternaam: "Elessar"
    )
);

session.Events.Append(
    "V9000003",
    new VertegenwoordigerWerdToegevoegdVanuitKBO(
        VertegenwoordigerId: 2,
        Insz: "66778899001",
        Voornaam: "Boromir",
        Achternaam: "of Gondor"
    )
);

session.Events.Append(
    "V9000003",
    new VertegenwoordigerWerdVerwijderdUitKBO(
        VertegenwoordigerId: 1,
        Insz: "55667788990",
        Voornaam: "Aragorn",
        Achternaam: "Elessar"
    )
);

// Scenario 4: FeitelijkeVereniging with multiple vertegenwoordigers toegevoegd
Console.WriteLine("📝 Creating FeitelijkeVereniging with multiple vertegenwoordigers...");

session.Events.Append(
    "V9000004",
    new FeitelijkeVerenigingWerdGeregistreerd(
        VCode: "V9000004",
        Naam: "Council of Elrond",
        KorteNaam: "",
        KorteBeschrijving: "",
        Startdatum: null,
        Doelgroep: new Registratiedata.Doelgroep(0, 150),
        IsUitgeschrevenUitPubliekeDatastroom: false,
        Contactgegevens: [],
        Locaties: [],
        Vertegenwoordigers: [],
        HoofdactiviteitenVerenigingsloket: []
    )
);

session.Events.Append(
    "V9000004",
    new FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid(
        VCode: "V9000004"
    )
);

var councilMembers = new[]
{
    ("Legolas", "Greenleaf", "legolas@mirkwood.example"),
    ("Gimli", "son of Gloin", "gimli@erebor.example"),
    ("Pippin", "Took", "pippin@shire.example"),
    ("Merry", "Brandybuck", "merry@shire.example"),
    ("Elrond", "Half-elven", "elrond@rivendell.example")
};

for (int i = 1; i <= 5; i++)
{
    var (firstName, lastName, email) = councilMembers[i - 1];
    session.Events.Append(
        "V9000004",
        new VertegenwoordigerWerdToegevoegd(
            VertegenwoordigerId: i,
            Insz: $"1234567890{i}",
            IsPrimair: i == 1,
            Roepnaam: firstName,
            Rol: i == 1 ? "Chairman" : "Council Member",
            Voornaam: firstName,
            Achternaam: lastName,
            Email: email,
            Telefoon: $"012345678{i}",
            Mobiel: $"041234567{i}",
            SocialMedia: ""
        )
    );
}

// Scenario 5: VerenigingMetRechtspersoonlijkheid registered WITH vertegenwoordigers in array
Console.WriteLine("📝 Creating VerenigingMetRechtspersoonlijkheid with vertegenwoordigers in registration...");

session.Events.Append(
    "V9000005",
    new VerenigingMetRechtspersoonlijkheidWerdGeregistreerd(
        VCode: "V9000005",
        Naam: "White Council of the Istari",
        KorteNaam: "White Council",
        Startdatum: DateOnly.Parse("2015-06-15"),
        Rechtsvorm: "IVZW",
        KboNummer: "0987654321"
    )
);

// Add vertegenwoordigers separately to this one too
session.Events.Append(
    "V9000005",
    new VertegenwoordigerWerdToegevoegd(
        VertegenwoordigerId: 1,
        Insz: "77889900112",
        IsPrimair: true,
        Roepnaam: "Galadriel",
        Rol: "Chairwoman",
        Voornaam: "Galadriel",
        Achternaam: "of Lothlorien",
        Email: "galadriel@lorien.example",
        Telefoon: "0266778899",
        Mobiel: "0466778899",
        SocialMedia: ""
    )
);

session.Events.Append(
    "V9000005",
    new VertegenwoordigerWerdToegevoegd(
        VertegenwoordigerId: 2,
        Insz: "88990011223",
        IsPrimair: false,
        Roepnaam: "Saruman",
        Rol: "Chief Wizard",
        Voornaam: "Saruman",
        Achternaam: "the White",
        Email: "saruman@isengard.example",
        Telefoon: "0277889900",
        Mobiel: "0477889900",
        SocialMedia: "https://wizards.example/saruman"
    )
);

await session.SaveChangesAsync();

Console.WriteLine("\n✅ Test data seeded successfully!");
Console.WriteLine($"   - V9000001: The Shire Preservation Society (Frodo: toegevoegd → gewijzigd → verwijderd)");
Console.WriteLine($"   - V9000002: The Fellowship of the Ring (Gandalf & Sam in registration array)");
Console.WriteLine($"   - V9000003: Guardians of Minas Tirith (Aragorn & Boromir: overgenomen → toegevoegd → verwijderd)");
Console.WriteLine($"   - V9000004: Council of Elrond (5 council members toegevoegd)");
Console.WriteLine($"   - V9000005: White Council of the Istari (Galadriel & Saruman)");
Console.WriteLine($"\n🎯 Total: 5 verenigingen with rich vertegenwoordiger event sequences");
Console.WriteLine($"\n📊 Event coverage:");
Console.WriteLine($"   ✓ FeitelijkeVerenigingWerdGeregistreerd (with/without Vertegenwoordigers array)");
Console.WriteLine($"   ✓ FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid");
Console.WriteLine($"   ✓ VerenigingMetRechtspersoonlijkheidWerdGeregistreerd");
Console.WriteLine($"   ✓ VertegenwoordigerWerdToegevoegd");
Console.WriteLine($"   ✓ VertegenwoordigerWerdGewijzigd");
Console.WriteLine($"   ✓ VertegenwoordigerWerdVerwijderd");
Console.WriteLine($"   ✓ VertegenwoordigerWerdOvergenomenUitKBO");
Console.WriteLine($"   ✓ VertegenwoordigerWerdToegevoegdVanuitKBO");
Console.WriteLine($"   ✓ VertegenwoordigerWerdVerwijderdUitKBO");
