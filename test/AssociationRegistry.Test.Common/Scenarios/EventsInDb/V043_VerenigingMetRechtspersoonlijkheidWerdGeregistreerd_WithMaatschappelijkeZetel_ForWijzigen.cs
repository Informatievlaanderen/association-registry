namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Events;
using AssociationRegistry.EventStore;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AutoFixture;
using global::AutoFixture;

public class V043_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForWijzigen : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly CommandMetadata Metadata;

    public V043_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForWijzigen()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999043";
        Naam = "Dee sjiekste club";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
        };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = new MaatschappelijkeZetelWerdOvergenomenUitKbo(
            fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 1,
                Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo,
            });

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;
    public string Naam { get; set; }

    public DateOnly? Startdatum
        => VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.Startdatum;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
