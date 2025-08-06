namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using MartenDb.Store;

public class V056_VerenigingWerdGeregistreerd_And_Gestopt_For_DuplicateDetection : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly FeitelijkeVerenigingWerdGeregistreerd FeitelijkeVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdGestopt VerenigingWerdGestopt;

    public V056_VerenigingWerdGeregistreerd_And_Gestopt_For_DuplicateDetection()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        FeitelijkeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = "De vereniging voor duplicate detection",
            Locaties =
            [
                fixture.Create<Registratiedata.Locatie>() with
                {
                    Adres = fixture.Create<Registratiedata.Adres>(),
                },
            ],
        };

        VerenigingWerdGestopt = fixture.Create<VerenigingWerdGestopt>();
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999056";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { FeitelijkeVerenigingWerdGeregistreerd, VerenigingWerdGestopt };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
