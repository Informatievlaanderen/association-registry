namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V051_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;
    public IEvent[] Verenigingen { get; }

    public V051_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9990000";

        /**
         * Hoofdletter ongevoelig → Vereniging = verEniging
         * Spatie ongevoelig
         * Leading spaces → Grote vereniging =  Grote vereniging
         * Trailing spaces → Grote vereniging = Grote vereniging
         * Dubbele spaces → Grote vereniging = Grote     vereniging
         * Accent ongevoelig → Cafésport = Cafesport
         * Leesteken ongevoelig → Sint-Servaas = Sint Servaas
         * Functiewoorden weglaten → De pottestampers = Pottestampers - de, het, van, … idealiter is deze lijst configureerbaar
         * Fuzzy search = kleine schrijfverschillen negeren. Deze zijn de elastic mogelijkheden:
         * Ander karakter gebruiken → Uitnodiging = Uitnodiding
         * 1 karakter minder → Vereniging = Verenging
         * 1 karakter meer → Vereniging = Vereeniging
         * 2 karakters van plaats wisselen → Pottestampers = Pottestapmers
         **/

        Verenigingen = new[]
        {
            VerenigingWerdGeregistreerd(fixture, naam: "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling"),
            VerenigingWerdGeregistreerd(fixture, naam: "Grote Vereniging"),
            VerenigingWerdGeregistreerd(fixture, naam: "Cafésport"),
            VerenigingWerdGeregistreerd(fixture, naam: "Sint-Servaas"),
            VerenigingWerdGeregistreerd(fixture, naam: "De pottestampers"),
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    private FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(Fixture fixture, string naam)
        => fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = naam,
            Locaties = new[]
            {
                fixture.Create<Registratiedata.Locatie>() with
                {
                    Adres = fixture.Create<Registratiedata.Adres>()
                        with
                        {
                            Straatnaam = fixture.Create<string>(),
                            Huisnummer = fixture.Create<string>(),
                            Postcode = "9832",
                            Gemeente = fixture.Create<string>(),
                            Land = fixture.Create<string>(),
                        },
                },
            },
            KorteNaam = string.Empty,
            Startdatum = null,
            KorteBeschrijving = string.Empty,
            Contactgegevens = Array.Empty<Registratiedata.Contactgegeven>(),
            Vertegenwoordigers = Array.Empty<Registratiedata.Vertegenwoordiger>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>(),
        };

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => Verenigingen;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
