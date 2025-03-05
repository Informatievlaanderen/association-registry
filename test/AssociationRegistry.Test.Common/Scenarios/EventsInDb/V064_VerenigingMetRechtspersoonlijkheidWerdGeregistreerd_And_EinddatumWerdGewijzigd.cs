namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using Vereniging;


public class V064_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_EinddatumWerdGewijzigd : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly EinddatumWerdGewijzigd EinddatumWerdGewijzigd;

    public V064_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_EinddatumWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = "Recht door zee",
            KorteNaam = "RDZ",
            KboNummer = "7981199946",
            Rechtsvorm = Verenigingstype.VZW.Code,
            Startdatum = new DateOnly(year: 2024, month: 02, day: 14),
        };

        EinddatumWerdGewijzigd = new EinddatumWerdGewijzigd(new DateOnly(year: 2024, month: 03, day: 07));

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999064";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            EinddatumWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
