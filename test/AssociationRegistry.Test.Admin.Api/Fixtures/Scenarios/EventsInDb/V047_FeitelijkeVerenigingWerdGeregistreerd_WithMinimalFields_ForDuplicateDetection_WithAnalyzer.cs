namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V047_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForDuplicateDetection_WithAnalyzer
{
    public readonly CommandMetadata Metadata;
    public (VCode, IEvent[])[] EventsPerVCode { get; }

    public V047_FeitelijkeVerenigingWerdGeregistreerd_WithMinimalFields_ForDuplicateDetection_WithAnalyzer()
    {
        var fixture = new Fixture().CustomizeAdminApi();
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

        EventsPerVCode = new[]
        {
            VerenigingWerdGeregistreerd(fixture, naam: "Vereniging van Technologïeënthusiasten: Inováçie & Ëntwikkeling", vCode: "V9999047",
                                        postcode: "9832", gemeente: "Neder-over-opper-onder-heembeek"),
            VerenigingWerdGeregistreerd(fixture, naam: "Grote Vereniging", vCode: "V9999048", postcode: "9832",
                                        gemeente: "Neder-over-opper-onder-heembeek"),
            VerenigingWerdGeregistreerd(fixture, naam: "Cafésport", vCode: "V9999049", postcode: "8800", gemeente: "Rumbeke"),
            VerenigingWerdGeregistreerd(fixture, naam: "Sint-Servaas", vCode: "V9999050", postcode: "8800", gemeente: "Roeselare"),
            VerenigingWerdGeregistreerd(fixture, naam: "De pottestampers", vCode: "V9999051", postcode: "9830",
                                        gemeente: "Heist-op-den-Berg"),
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    private (VCode, IEvent[]) VerenigingWerdGeregistreerd(Fixture fixture, string naam, string vCode, string postcode, string gemeente)
        => (VCode.Create(vCode), new IEvent[]
        {
            fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
            {
                VCode = vCode,
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
                                Postcode = postcode,
                                Gemeente = gemeente,
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
            },
        });

    public StreamActionResult Result { get; set; } = null!;

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
