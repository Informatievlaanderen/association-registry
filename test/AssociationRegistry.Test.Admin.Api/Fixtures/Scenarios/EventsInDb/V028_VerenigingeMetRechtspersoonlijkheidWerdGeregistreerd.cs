namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;

public class V028_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly CommandMetadata Metadata;

    public V028_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCode = "V9999028";
        Naam = "Het recht zal overwinnen";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
        };

        KboNummer = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string KboNummer { get; set; }
    public string Naam { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
            { VerenigingMetRechtspersoonlijkheidWerdGeregistreerd };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}

public class V039_VerenigingMetRechtspersoonlijkheid_RoepnaamWerdGewijzigd : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly RoepnaamWerdGewijzigd RoepnaamWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public V039_VerenigingMetRechtspersoonlijkheid_RoepnaamWerdGewijzigd()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCode = "V9999039";
        Naam = "Het recht zal overwinnen";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
        };

        RoepnaamWerdGewijzigd = fixture.Create<RoepnaamWerdGewijzigd>();
        KboNummer = VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.KboNummer;
        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string KboNummer { get; set; }
    public string Naam { get; set; }
    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            RoepnaamWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
