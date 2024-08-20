namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AutoFixture;
using global::AutoFixture;

public class V057_VerenigingWerdGeregistreerd_With_KboLocatie_For_DuplicateDetection : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;

    public V057_VerenigingWerdGeregistreerd_With_KboLocatie_For_DuplicateDetection()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
        };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999057";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd, MaatschappelijkeZetelWerdOvergenomenUitKbo };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
