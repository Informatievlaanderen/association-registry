namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using global::AutoFixture;

public class V063_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_StartdatumWerdGewijzigd : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly StartdatumWerdGewijzigd StartdatumWerdGewijzigd;

    public V063_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_StartdatumWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = "Recht door zee",
            KorteNaam = "RDZ",
            KboNummer = "7981199945",
            Rechtsvorm = Verenigingstype.VZW.Code,
            Startdatum = new DateOnly(year: 2024, month: 02, day: 14),
        };

        StartdatumWerdGewijzigd = new StartdatumWerdGewijzigd(VCode, new DateOnly(year: 2024, month: 03, day: 07));

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999063";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            StartdatumWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
