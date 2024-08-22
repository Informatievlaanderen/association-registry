namespace AssociationRegistry.Test.Common.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using global::AutoFixture;

public class V034_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForAddingLocatie : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly CommandMetadata Metadata;

    public V034_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_WithMaatschappelijkeZetel_ForAddingLocatie()
    {
        var fixture = new Fixture().CustomizeAdminApi();
        VCode = "V9999034";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            KorteNaam = string.Empty,
            Startdatum = null,
        };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>() with
        {
            Locatie = fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = 1,
            },
        };

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; }
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
