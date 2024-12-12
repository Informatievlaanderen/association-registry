namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V080_VerenigingWerdGeregistreerd_And_Gemarkeerd_Als_Dubbel_For_DuplicateDetection : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly FeitelijkeVerenigingWerdGeregistreerd AuthentiekeVerenigingWerdGeregistreerd;
    public readonly FeitelijkeVerenigingWerdGeregistreerd DubbeleVerenigingWerdGeregistreerd;
    public readonly VerenigingWerdGemarkeerdAlsDubbelVan VerenigingWerdGemarkeerdAlsDubbelVan;

    public V080_VerenigingWerdGeregistreerd_And_Gemarkeerd_Als_Dubbel_For_DuplicateDetection()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = VCodeDubbeleVereniging;

        AuthentiekeVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCodeAuthentiekeVereniging,
            Locaties = new[]
            {
                fixture.Create<Registratiedata.Locatie>() with
                {
                    Adres = fixture.Create<Registratiedata.Adres>(),
                },
            },
        };

        DubbeleVerenigingWerdGeregistreerd = fixture.Create<FeitelijkeVerenigingWerdGeregistreerd>() with
        {
            VCode = VCodeDubbeleVereniging,
            Locaties = new[]
            {
                fixture.Create<Registratiedata.Locatie>() with
                {
                    Adres = fixture.Create<Registratiedata.Adres>(),
                },
            },
        };

        VerenigingWerdGemarkeerdAlsDubbelVan = new(VCode, AuthentiekeVerenigingWerdGeregistreerd.VCode);
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCodeAuthentiekeVereniging = "V9999080";
    public string VCodeDubbeleVereniging = "V9999081";
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { AuthentiekeVerenigingWerdGeregistreerd, DubbeleVerenigingWerdGeregistreerd, VerenigingWerdGemarkeerdAlsDubbelVan };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
