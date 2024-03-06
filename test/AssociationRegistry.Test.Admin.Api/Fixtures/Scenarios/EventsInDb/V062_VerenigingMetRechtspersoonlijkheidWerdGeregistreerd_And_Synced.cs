﻿namespace AssociationRegistry.Test.Admin.Api.Fixtures.Scenarios.EventsInDb;

using AssociationRegistry.Framework;
using AutoFixture;
using Events;
using EventStore;
using Framework;
using Test.Framework.Customizations;
using Vereniging;

public class V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced : IEventsInDbScenario
{
    public readonly CommandMetadata Metadata;
    public readonly VerenigingMetRechtspersoonlijkheidWerdGeregistreerd VerenigingMetRechtspersoonlijkheidWerdGeregistreerd;
    public readonly ContactgegevenWerdOvergenomenUitKBO ContactgegevenWerdOvergenomenUitKbo;
    public readonly MaatschappelijkeZetelWerdOvergenomenUitKbo MaatschappelijkeZetelWerdOvergenomenUitKbo;
    public readonly RechtsvormWerdGewijzigdInKBO RechtsvormWerdGewijzigdInKBO;
    public readonly NaamWerdGewijzigdInKbo NaamWerdGewijzigdInKbo;
    public readonly KorteNaamWerdGewijzigdInKbo KorteNaamWerdGewijzigdInKbo;
    public readonly ContactgegevenWerdGewijzigdInKbo ContactgegevenWerdGewijzigdInKbo;
    public readonly MaatschappelijkeZetelWerdGewijzigdInKbo MaatschappelijkeZetelWerdGewijzigdInKbo;

    public V062_VerenigingMetRechtspersoonlijkheidWerdGeregistreerd_And_Synced()
    {
        var fixture = new Fixture().CustomizeAdminApi();

        VerenigingMetRechtspersoonlijkheidWerdGeregistreerd = fixture.Create<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd>() with
        {
            VCode = VCode,
            Naam = "Recht door zee",
            KorteNaam = "RDZ",
            KboNummer = "7981199944",
            Rechtsvorm = Verenigingstype.VZW.Code,
        };

        RechtsvormWerdGewijzigdInKBO = fixture.Create<RechtsvormWerdGewijzigdInKBO>();
        NaamWerdGewijzigdInKbo = fixture.Create<NaamWerdGewijzigdInKbo>();
        KorteNaamWerdGewijzigdInKbo = fixture.Create<KorteNaamWerdGewijzigdInKbo>();

        MaatschappelijkeZetelWerdOvergenomenUitKbo = fixture.Create<MaatschappelijkeZetelWerdOvergenomenUitKbo>();

        MaatschappelijkeZetelWerdGewijzigdInKbo = fixture.Create<MaatschappelijkeZetelWerdGewijzigdInKbo>() with
        {
            Locatie = fixture.Create<Registratiedata.Locatie>() with
            {
                LocatieId = MaatschappelijkeZetelWerdOvergenomenUitKbo.Locatie.LocatieId,
            },
        };

        ContactgegevenWerdOvergenomenUitKbo = fixture.Create<ContactgegevenWerdOvergenomenUitKBO>() with
        {
            ContactgegevenId = 1,
        };

        ContactgegevenWerdGewijzigdInKbo = new ContactgegevenWerdGewijzigdInKbo(1, ContactgegevenWerdOvergenomenUitKbo.Contactgegeventype,
                                                                                ContactgegevenWerdOvergenomenUitKbo.TypeVolgensKbo,
                                                                                fixture.CreateContactgegevenVolgensType(
                                                                                    ContactgegevenWerdOvergenomenUitKbo
                                                                                       .Contactgegeventype).Waarde);

        Metadata = fixture.Create<CommandMetadata>() with { ExpectedVersion = null };
    }

    public string VCode { get; set; } = "V9999062";
    public StreamActionResult Result { get; set; } = null!;

    public IEvent[] GetEvents()
        => new IEvent[]
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

    public CommandMetadata GetCommandMetadata()
        => Metadata;
}
