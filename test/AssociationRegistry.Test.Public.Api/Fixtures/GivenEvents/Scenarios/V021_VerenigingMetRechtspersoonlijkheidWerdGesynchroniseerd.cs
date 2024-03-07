﻿namespace AssociationRegistry.Test.Public.Api.Fixtures.GivenEvents.Scenarios;

using AssociationRegistry.Framework;
using Events;
using NodaTime.Extensions;
using Vereniging;

public class V021_VerenigingMetRechtspersoonlijkheidWerdGesynchroniseerd : IScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
        VCode: "V0001021",
        KboNummer: "0987654420",
        Rechtsvorm: Verenigingstype.VZW.Code,
        Naam: "Feesten Affligem",
        string.Empty,
        Startdatum: null);

    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo =
        new(new Registratiedata.Locatie(1, Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                        false, "",
                                        new Registratiedata.Adres(
                                            "maatstraat", "1", null, "1000", "Brussel",
                                            "Belgie"), null));

    public readonly MaatschappelijkeZetelWerdGewijzigdInKbo MaatschappelijkeZetelWerdGewijzigdInKbo =
        new(new Registratiedata.Locatie(1, Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde,
                                        false, "",
                                        new Registratiedata.Adres(
                                            "beterlaan", "42", "b", "2000", "Antwerpen",
                                            "Belgie"), null));

    public readonly RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO = new(Verenigingstype.IVZW.Code);
    public readonly NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo = new("Feesten Asse");
    public readonly KorteNaamWerdGewijzigdInKbo KorteNaamWerdGewijzigdInKbo = new("FA");

    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKbo =
        new(1, Contactgegeventype.Email, ContactgegeventypeVolgensKbo.Email, "example.me@example.org");

    public readonly ContactgegevenWerdGewijzigdInKbo ContactgegevenWerdGewijzigdInKbo =
        new(1, Contactgegeventype.Email, ContactgegeventypeVolgensKbo.Email, "test.me@example.org");


    public VCode VCode
        => VCode.Create(VerenigingMetRechtspersoonlijkheidWerdGeregistreerd.VCode);

    public IEvent[] GetEvents()
    {
        return new IEvent[]
        {
            VerenigingMetRechtspersoonlijkheidWerdGeregistreerd,
            MaatschappelijkeZetelWerdOvergenomenUitKbo,
            ContactgegevenWerdOvergenomenUitKbo,
            RechtsvormWerdGewijzigdInKBO,
            NaamWerdGewijzigdInKbo,
            KorteNaamWerdGewijzigdInKbo,
            MaatschappelijkeZetelWerdGewijzigdInKbo,
            ContactgegevenWerdGewijzigdInKbo,
            new SynchronisatieMetKboWasSuccesvol(),
        };
    }

    public CommandMetadata GetCommandMetadata()
        => new(Initiator: "OVO000001",
               new DateTimeOffset(year: 2023, month: 01, day: 25, hour: 0, minute: 0, second: 0, TimeSpan.Zero).ToInstant(),
               Guid.NewGuid());
}
