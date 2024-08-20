namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using global::AutoFixture;

public class V041_FeitelijkeVerenigingWerdGestopt : IEventsInDbScenario
{
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdGestopt VerenigingWerdGestopt;
    public readonly EinddatumWerdGewijzigd EinddatumWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public V041_FeitelijkeVerenigingWerdGestopt()
    {
        var startdatum = new DateOnly(year: 2023, month: 09, day: 06);
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999041";

        FeitelijkeVerenigingWerdGeregistreerd = new FeitelijkeVerenigingWerdGeregistreerd(
            VCode,
            Naam: "De dulste van Hulste",
            KorteNaam: "",
            KorteBeschrijving: "",
            startdatum,
            new Registratiedata.Doelgroep(Minimumleeftijd: 0, Maximumleeftijd: 150),
            IsUitgeschrevenUitPubliekeDatastroom: false,
            Array.Empty<Registratiedata.Contactgegeven>(),
            Array.Empty<Registratiedata.Locatie>(),
            Array.Empty<Registratiedata.Vertegenwoordiger>(),
            Array.Empty<Registratiedata.HoofdactiviteitVerenigingsloket>()
        );

        VerenigingWerdGestopt = new VerenigingWerdGestopt(new DateOnly(year: 2023, month: 10, day: 06));
        EinddatumWerdGewijzigd = new EinddatumWerdGewijzigd(new DateOnly(year: 2023, month: 10, day: 07));

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdGestopt, EinddatumWerdGewijzigd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
