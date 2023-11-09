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

    public V051_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9990000";

        /**
         * Hoofdletter ongevoelig → Vereniging = verEniging
         * Spatie ongevoelig
         * leading spaces → Grote vereniging =  Grote vereniging
         * trailing spaces → Grote vereniging = Grote vereniging
         * dubbele spaces → Grote vereniging = Grote     vereniging
         * Accent ongevoelig → Cafésport = Cafesport
         * leesteken ongevoelig → Sint-Servaas = Sint Servaas
         * functiewoorden weglaten → De pottestampers = Pottestampers
         * de, het, van, … idealiter is deze lijst configureerbaar
         * fuzzy search = kleine schrijfverschillen negeren. Deze zijn de elastic mogelijkheden:
         * ander karakter gebruiken → Uitnodiging = Uitnodiding
         * 1 karakter minder → Vereniging = Verenging
         * 1 karakter meer → Vereniging = Vereeniging
         * 2 karakters van plaats wisselen → Pottestampers = Pottestapmers
         **/

        var verenigingen = new List<FeitelijkeVerenigingWerdGeregistreerd>
        {
            VerenigingWerdGeregistreerd(fixture, naam: "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling"),
            VerenigingWerdGeregistreerd(fixture, naam: "Grote Vereniging"),
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    private FeitelijkeVerenigingWerdGeregistreerd VerenigingWerdGeregistreerd(Fixture fixture, string naam)
        => fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = naam,
            Locaties = Array.Empty<Registratiedata.Locatie>(),
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
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
