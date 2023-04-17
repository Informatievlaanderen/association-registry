namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios;

using Events;
using EventStore;
using AssociationRegistry.Framework;
using Framework;
using AutoFixture;

public class V010_VerenigingWerdGeregistreerd_WithAllFields : IEventsInDbScenario
{
    public readonly VerenigingWerdGeregistreerd VerenigingWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V010_VerenigingWerdGeregistreerd_WithAllFields()
    {
        var fixture = new Fixture().CustomizeAll();
        VCode = "V9999010";
        Naam = "De leuke club";
        VerenigingWerdGeregistreerd = fixture.Create<VerenigingWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            Contactgegevens = fixture.CreateMany<VerenigingWerdGeregistreerd.Contactgegeven>().Select(
                (contactgegeven, w) => contactgegeven with
                {
                    IsPrimair = w == 0,
                }
            ).ToArray(),
            Vertegenwoordigers = fixture.CreateMany<VerenigingWerdGeregistreerd.Vertegenwoordiger>().Select(
                (vertegenwoordiger, i) => vertegenwoordiger with
                {
                    PrimairContactpersoon = i == 0,
                    Contactgegevens = fixture.CreateMany<VerenigingWerdGeregistreerd.Contactgegeven>().Select(
                        (contactgegeven, p) => contactgegeven with
                        {
                            IsPrimair = p == 0,
                        }).ToArray(),
                }).ToArray(),
        };
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => VerenigingWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
