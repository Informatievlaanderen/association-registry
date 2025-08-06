namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using DecentraalBeheer.Vereniging;
using DecentraalBeheer.Vereniging.Adressen;
using Events;
using Events.Factories;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;
using Vereniging;

public class V027_LocatieWerdGewijzigd : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly LocatieWerdGewijzigd LocatieWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public V027_LocatieWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999027";

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam: "Party in Brakeldorp",
            KorteNaam: "FBD",
            KorteBeschrijving: "De party van Brakeldorp",
            DateOnly.FromDateTime(new DateTime(year: 2022, month: 11, day: 9)),
            EventFactory.Doelgroep(Doelgroep.Null),
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            new[]
            {
                new Registratiedata.Locatie(
                    LocatieId: 1,
                    Locatietype: "Correspondentie",
                    IsPrimair: true,
                    Naam: "Correspondentie",
                    new Registratiedata.Adres(
                        Straatnaam: "Stationsstraat",
                        Huisnummer: "1",
                        Busnummer: "B",
                        Postcode: "1790",
                        Gemeente: "Affligem",
                        Land: "België"),
                    new Registratiedata.AdresId(Adresbron.AR.Code, Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    LocatieId: 2,
                    Locatietype: "Activiteiten",
                    IsPrimair: false,
                    Naam: "Activiteiten",
                    Adres: null,
                    new Registratiedata.AdresId(Adresbron.AR.Code, Bronwaarde: "https://data.vlaanderen.be/id/adres/0")),
                new Registratiedata.Locatie(
                    LocatieId: 3,
                    Locatietype: "Activiteiten",
                    IsPrimair: false,
                    Naam: "Activiteiten",
                    new Registratiedata.Adres(
                        Straatnaam: "Dorpstraat",
                        Huisnummer: "1",
                        Busnummer: "B",
                        Postcode: "1790",
                        Gemeente: "Affligem",
                        Land: "België"),
                    AdresId: null),
            },
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>());

        LocatieWerdGewijzigd = new LocatieWerdGewijzigd(
            new Registratiedata.Locatie(
                LocatieId: 1,
                Locatietype.Activiteiten,
                IsPrimair: false,
                Naam: "Gewijzigd",
                new Registratiedata.Adres(
                    Straatnaam: "straatnaam",
                    Huisnummer: "99",
                    Busnummer: "A1",
                    Postcode: "0123",
                    Gemeente: "Zonnedorp",
                    Land: "Frankrijk"),
                new Registratiedata.AdresId(Adresbron.AR, AdresId.DataVlaanderenAdresPrefix + "4")
            ));

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            FeitelijkeVerenigingWerdGeregistreerd,
            LocatieWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
