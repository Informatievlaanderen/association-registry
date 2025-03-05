namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;
using Vereniging;


public class V045_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_ContactgegevenFromKbo_For_Wijzigen : IEventsInDbScenario
{
    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKBO;
    public readonly CommandMetadata Metadata;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;

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
            KboNummer = "7981199845",
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        ContactgegevenWerdOvergenomenUitKBO = new ContactgegevenWerdOvergenomenUitKBO(
            ContactgegevenId: 1,
            Contactgegeventype.Email.Waarde,
            Contactgegeventype.Email.Waarde,
            Waarde: "test@example.org");

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
