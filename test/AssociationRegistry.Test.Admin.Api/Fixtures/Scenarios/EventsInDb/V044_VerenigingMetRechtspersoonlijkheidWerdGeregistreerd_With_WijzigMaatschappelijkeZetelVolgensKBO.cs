namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V044_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly MaatschappelijkeZetelVolgensKBOWerdGewijzigd MaatschappelijkeZetelVolgensKBOWerdGewijzigd;
    public readonly CommandMetadata Metadata;

    public V044_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_With_WijzigMaatschappelijkeZetelVolgensKBO()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCode = "V9999044";
        Naam = "Recht door zee";

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = Naam,
            KorteNaam = "RDZ",
            KboNummer = "7981199887",
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        MaatschappelijkeZetelWerdOvergenomenUitKbo = new MaatschappelijkeZetelWerdOvergenomenUitKbo(
            Locatie: new Registratiedata.Locatie(LocatieId: 1, Locatietype: Locatietype.MaatschappelijkeZetelVolgensKbo, Naam: string.Empty,
                                                 IsPrimair: false, Adres: new Registratiedata.Adres(
                                                     "Stationsstraat",
                                                     "1",
                                                     "B",
                                                     "1790",
                                                     "Affligem",
                                                     "België"), AdresId: null));

        MaatschappelijkeZetelVolgensKBOWerdGewijzigd = new MaatschappelijkeZetelVolgensKBOWerdGewijzigd(1, "Station", true);

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
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
            MaatschappelijkeZetelVolgensKBOWerdGewijzigd,
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
