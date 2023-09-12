namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V045_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_ContactgegevenFromKbo_For_Wijzigen : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKBO;
    public readonly CommandMetadata Metadata;

    public V045_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_ContactgegevenFromKbo_For_Wijzigen()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCode = "V9999045";
        Naam = "Recht door zee";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            KorteNaam = "RDZ",
            KboNummer = "7981199887",
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        ContactgegevenWerdOvergenomenUitKBO = new ContactgegevenWerdOvergenomenUitKBO(
            1,
            ContactgegevenType.Email.Waarde,
            ContactgegevenType.Email.Waarde,
            "test@example.org");

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
            ContactgegevenWerdOvergenomenUitKBO,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
