﻿namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AssociationRegistry.Magda;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Vereniging;

public class V029_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_Addres : IEventsInDbScenario
{
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly CommandMetadata Metadata;

    public V029_VerenigingeMetRechtspersoonlijkheidWerdGeregistreerd_With_Addres()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VCode = "V9999029";
        Naam = "Recht door zee";
        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = new(
            VCode,
            "7981199887",
            Rechtsvorm.VZW.Waarde,
            Naam,
            string.Empty,
            null);
        MaatschappelijkeZetelWerdOvergenomenUitKbo = new MaatschappelijkeZetelWerdOvergenomenUitKbo(
            Locatie: fixture.Create<Registratiedata.Locatie>() with
            {
                Locatietype = Locatietype.MaatschappelijkeZetelVolgensKbo,
                Naam = string.Empty,
                IsPrimair = false,
                Adres = new Registratiedata.Adres(
                    "Stationsstraat",
                    "1",
                    "B",
                    "1790",
                    "Affligem",
                    "België"),
            });
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
        };

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
