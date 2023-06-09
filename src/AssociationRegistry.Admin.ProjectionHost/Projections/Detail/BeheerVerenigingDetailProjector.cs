namespace AssociationRegistry.Admin.ProjectionHost.Projections.Detail;

using System.Linq;
using AssociationRegistry.Admin.Schema;
using AssociationRegistry.Admin.Schema.Detail;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Marten.Events;

public class BeheerVerenigingDetailProjector
{
    public static BeheerVerenigingDetailDocument Create(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd)
        => new()
        {
            VCode = afdelingWerdGeregistreerd.Data.VCode,
            Type = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.Afdeling),
            Naam = afdelingWerdGeregistreerd.Data.Naam,
            KorteNaam = afdelingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = afdelingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = afdelingWerdGeregistreerd.Data.Startdatum?.ToString(Schema.WellknownFormats.DateOnly),
            DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(afdelingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)),
            Status = "Actief",
            Contactgegevens = afdelingWerdGeregistreerd.Data.Contactgegevens.Select(
                    BeheerVerenigingDetailMapper.MapContactgegeven)
                .ToArray(),
            Locaties = afdelingWerdGeregistreerd.Data.Locaties.Select(BeheerVerenigingDetailMapper.MapLocatie)
                .ToArray(),
            Vertegenwoordigers = afdelingWerdGeregistreerd.Data.Vertegenwoordigers.Select(
                    BeheerVerenigingDetailMapper.MapVertegenwoordiger)
                .ToArray(),
            HoofdactiviteitenVerenigingsloket = afdelingWerdGeregistreerd.Data.HoofdactiviteitenVerenigingsloket.Select(
                    BeheerVerenigingDetailMapper.MapHoofdactiviteitVerenigingsloket)
                .ToArray(),
            Relaties = new[]
            {
                BeheerVerenigingDetailMapper.MapMoederRelatie(afdelingWerdGeregistreerd.Data.Moedervereniging),
            },
            Metadata = new Metadata(afdelingWerdGeregistreerd.Sequence, afdelingWerdGeregistreerd.Version),
        };

    public static void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(naamWerdGewijzigd.Sequence, naamWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(korteNaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(korteNaamWerdGewijzigd.Sequence, korteNaamWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(korteBeschrijvingWerdGewijzigd.Sequence, korteBeschrijvingWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Startdatum = startdatumWerdGewijzigd.Data.Startdatum?.ToString(Schema.WellknownFormats.DateOnly);
        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(startdatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(startdatumWerdGewijzigd.Sequence, startdatumWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.Append(
            new BeheerVerenigingDetailDocument.Contactgegeven
            {
                ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                Type = contactgegevenWerdToegevoegd.Data.Type,
                Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
            }).ToArray();
        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(contactgegevenWerdToegevoegd.Sequence, contactgegevenWerdToegevoegd.Version);
    }

    public static void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Where(c => c.ContactgegevenId != contactgegevenWerdGewijzigd.Data.ContactgegevenId)
            .Append(
                new BeheerVerenigingDetailDocument.Contactgegeven
                {
                    ContactgegevenId = contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                    Type = contactgegevenWerdGewijzigd.Data.Type,
                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                })
            .ToArray();

        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(contactgegevenWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(contactgegevenWerdGewijzigd.Sequence, contactgegevenWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, BeheerVerenigingDetailDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
            .Where(
                c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
            .ToArray();

        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(contactgegevenWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(contactgegevenWerdVerwijderd.Sequence, contactgegevenWerdVerwijderd.Version);
    }

    public static void Apply(IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofactiviteitenVerenigingloketWerdenGewijzigd, BeheerVerenigingDetailDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofactiviteitenVerenigingloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket.Select(
            h => new BeheerVerenigingDetailDocument.HoofdactiviteitVerenigingsloket
            {
                Code = h.Code,
                Beschrijving = h.Beschrijving,
            }).ToArray();
        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(hoofactiviteitenVerenigingloketWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(hoofactiviteitenVerenigingloketWerdenGewijzigd.Sequence, hoofactiviteitenVerenigingloketWerdenGewijzigd.Version);
    }

    public static void Apply(IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd, BeheerVerenigingDetailDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers.Append(
            new BeheerVerenigingDetailDocument.Vertegenwoordiger
            {
                VertegenwoordigerId = vertegenwoordigerWerdToegevoegd.Data.VertegenwoordigerId,
                Achternaam = vertegenwoordigerWerdToegevoegd.Data.Achternaam,
                Voornaam = vertegenwoordigerWerdToegevoegd.Data.Voornaam,
                Roepnaam = vertegenwoordigerWerdToegevoegd.Data.Roepnaam,
                Rol = vertegenwoordigerWerdToegevoegd.Data.Rol,
                IsPrimair = vertegenwoordigerWerdToegevoegd.Data.IsPrimair,
                Email = vertegenwoordigerWerdToegevoegd.Data.Email,
                Telefoon = vertegenwoordigerWerdToegevoegd.Data.Telefoon,
                Mobiel = vertegenwoordigerWerdToegevoegd.Data.Mobiel,
                SocialMedia = vertegenwoordigerWerdToegevoegd.Data.SocialMedia,
            }).ToArray();

        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(vertegenwoordigerWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(vertegenwoordigerWerdToegevoegd.Sequence, vertegenwoordigerWerdToegevoegd.Version);
    }

    public static void Apply(IEvent<VertegenwoordigerWerdGewijzigd> vertegenwoordigerWerdGewijzigd, BeheerVerenigingDetailDocument document)
    {
        var vertegenwoordigerToUpdate = document.Vertegenwoordigers.Single(v => v.VertegenwoordigerId == vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId);
        document.Vertegenwoordigers = document.Vertegenwoordigers
            .Where(c => c.VertegenwoordigerId != vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId)
            .Append(
                new BeheerVerenigingDetailDocument.Vertegenwoordiger
                {
                    VertegenwoordigerId = vertegenwoordigerWerdGewijzigd.Data.VertegenwoordigerId,
                    Achternaam = vertegenwoordigerToUpdate.Achternaam,
                    Voornaam = vertegenwoordigerToUpdate.Voornaam,
                    Roepnaam = vertegenwoordigerWerdGewijzigd.Data.Roepnaam,
                    Rol = vertegenwoordigerWerdGewijzigd.Data.Rol,
                    IsPrimair = vertegenwoordigerWerdGewijzigd.Data.IsPrimair,
                    Email = vertegenwoordigerWerdGewijzigd.Data.Email,
                    Telefoon = vertegenwoordigerWerdGewijzigd.Data.Telefoon,
                    Mobiel = vertegenwoordigerWerdGewijzigd.Data.Mobiel,
                    SocialMedia = vertegenwoordigerWerdGewijzigd.Data.SocialMedia,
                })
            .ToArray();

        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(vertegenwoordigerWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(vertegenwoordigerWerdGewijzigd.Sequence, vertegenwoordigerWerdGewijzigd.Version);
    }

    public static void Apply(IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd, BeheerVerenigingDetailDocument document)
    {
        document.Vertegenwoordigers = document.Vertegenwoordigers
            .Where(
                c => c.VertegenwoordigerId != vertegenwoordigerWerdVerwijderd.Data.VertegenwoordigerId)
            .ToArray();

        document.DatumLaatsteAanpassing = Schema.Formatters.ToBelgianDate(vertegenwoordigerWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip));
        document.Metadata = new Metadata(vertegenwoordigerWerdVerwijderd.Sequence, vertegenwoordigerWerdVerwijderd.Version);
    }

    public static BeheerVerenigingDetailDocument Apply(IEvent<AfdelingWerdGeregistreerd> afdelingWerdGeregistreerd, BeheerVerenigingDetailDocument? moeder)
    {
        moeder = moeder with
        {
            Relaties = moeder.Relaties.Append(
                new BeheerVerenigingDetailDocument.Relatie
                {
                    Type = RelatieType.IsAfdelingVan.InverseBeschrijving,
                    AndereVereniging = new BeheerVerenigingDetailDocument.Relatie.GerelateerdeVereniging
                    {
                        KboNummer = string.Empty,
                        Naam = afdelingWerdGeregistreerd.Data.Naam,
                        VCode = afdelingWerdGeregistreerd.Data.VCode,
                    }
                }).ToArray()
        };
        return moeder;
    }
}
