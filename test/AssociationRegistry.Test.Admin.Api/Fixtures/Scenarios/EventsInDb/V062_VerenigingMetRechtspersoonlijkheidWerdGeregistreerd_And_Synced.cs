namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo;

    public V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = "Recht door zee",
            KorteNaam = "RDZ",
            KboNummer = "7981199944",
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        NaamWerdGewijzigdInKbo = fixture.Create<NaamWerdGewijzigdInKbo>();

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999062";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            NaamWerdGewijzigdInKbo,
            new KboSyncSuccessful(),
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
