namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;

public class V042_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly VertegenwoordigerWerdOvergenomenUitKBO VertegenwoordigerWerdOvergenomenUitKBO;
    public readonly CommandMetadata Metadata;

    public V042_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithVertegenwoordiger_ForWijzigVertegenwoordiger()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999042";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        VertegenwoordigerWerdOvergenomenUitKBO = fixture.Create<VertegenwoordigerWerdOvergenomenUitKBO>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public DateOnly? Startdatum
        => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            VertegenwoordigerWerdOvergenomenUitKBO,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
