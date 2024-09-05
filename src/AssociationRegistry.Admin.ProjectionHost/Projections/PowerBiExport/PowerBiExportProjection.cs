namespace AssociationRegistry.Admin.ProjectionHost.Projections.PowerBiExport;

using Detail;
using Events;
using Formats;
using Framework;
using JsonLdContext;
using Marten.Events;
using Marten.Events.Aggregation;
using Schema.Constants;
using Schema.Detail;
using Schema.PowerBiExport;
using Vereniging;
using Contactgegeven = Schema.Detail.Contactgegeven;
using Doelgroep = Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = Schema.PowerBiExport.HoofdactiviteitVerenigingsloket;
using Locatie = Schema.Detail.Locatie;

public class PowerBiExportProjection : SingleStreamProjection<PowerBiExportDocument>
{
    public const string StatusVerwijderd = "Verwijderd";

    public PowerBiExportProjection()
    {

    }

    public PowerBiExportDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
        => new()
        {
            VCode = feitelijkeVerenigingWerdGeregistreerd.Data.VCode,
            Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.FeitelijkeVereniging),
            Naam = feitelijkeVerenigingWerdGeregistreerd.Data.Naam,
            KorteNaam = feitelijkeVerenigingWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = feitelijkeVerenigingWerdGeregistreerd.Data.KorteBeschrijving,
            Startdatum = feitelijkeVerenigingWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = BeheerVerenigingDetailMapper.MapDoelgroep(feitelijkeVerenigingWerdGeregistreerd.Data.Doelgroep,
                                                                  feitelijkeVerenigingWerdGeregistreerd.Data.VCode),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = feitelijkeVerenigingWerdGeregistreerd.Data.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = feitelijkeVerenigingWerdGeregistreerd.Data.Contactgegevens
                                                                   .Select(c => BeheerVerenigingDetailMapper.MapContactgegeven(
                                                                               c, feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
                                                                               feitelijkeVerenigingWerdGeregistreerd.Data.VCode))
                                                                   .ToArray(),
            Locaties = feitelijkeVerenigingWerdGeregistreerd.Data.Locaties
                                                            .Select(loc => BeheerVerenigingDetailMapper.MapLocatie(
                                                                        loc, feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
                                                                        feitelijkeVerenigingWerdGeregistreerd.Data.VCode)).ToArray(),
            AantalVertegenwoordigers = feitelijkeVerenigingWerdGeregistreerd.Data.Vertegenwoordigers.Length,
            HoofdactiviteitenVerenigingsloket = feitelijkeVerenigingWerdGeregistreerd.Data
                                                                                     .HoofdactiviteitenVerenigingsloket
                                                                                     .Select(x => new HoofdactiviteitVerenigingsloket()
                                                                                      {
                                                                                          Code = x.Code,
                                                                                          Naam = x.Naam,
                                                                                      })
                                                                                     .ToArray(),
            Bron = feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
        };

    public PowerBiExportDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
        => new()
        {
            VCode = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode,
            Verenigingstype = new VerenigingsType
            {
                Code = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Code,
                Naam = Verenigingstype.Parse(verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm).Naam,
            },
            Naam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Naam,
            Roepnaam = string.Empty,
            KorteNaam = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KorteNaam,
            KorteBeschrijving = string.Empty,
            Startdatum = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = new Doelgroep
            {
                JsonLdMetadata =
                    BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Doelgroep,
                                                                      verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.VCode),
                Minimumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMinimumleeftijd,
                Maximumleeftijd = AssociationRegistry.Vereniging.Doelgroep.StandaardMaximumleeftijd,
            },
            Rechtsvorm = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Rechtsvorm,
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = false,
            Contactgegevens = Array.Empty<Contactgegeven>(),
            Locaties = Array.Empty<Locatie>(),
            HoofdactiviteitenVerenigingsloket = Array.Empty<HoofdactiviteitVerenigingsloket>(),
            Bron = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Bron,
            KboNummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate(),
        };

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
        document.DatumLaatsteAanpassing = naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, PowerBiExportDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;
        document.DatumLaatsteAanpassing = korteNaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Roepnaam = roepnaamWerdGewijzigd.Data.Roepnaam;
        document.DatumLaatsteAanpassing = roepnaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, PowerBiExportDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;
        document.DatumLaatsteAanpassing = korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigd.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;
        document.DatumLaatsteAanpassing = startdatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<StartdatumWerdGewijzigdInKbo> startdatumWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;
        document.DatumLaatsteAanpassing = startdatumWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Doelgroep = new Doelgroep
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Doelgroep, doelgroepWerdGewijzigd.StreamKey!),
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };
        document.DatumLaatsteAanpassing = doelgroepWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.Append(
                                                new Contactgegeven
                                                {
                                                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                        JsonLdType.Contactgegeven, document.VCode,
                                                        contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                                                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                                                    Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                                                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdToegevoegd.Data.Beschrijving,
                                                    Bron = contactgegevenWerdToegevoegd.Data.Bron,
                                                    IsPrimair = contactgegevenWerdToegevoegd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<ContactgegevenWerdGewijzigd> contactgegevenWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .UpdateSingle(
                                                identityFunc: c => c.ContactgegevenId == contactgegevenWerdGewijzigd.Data.ContactgegevenId,
                                                update: c => c with
                                                {
                                                    Waarde = contactgegevenWerdGewijzigd.Data.Waarde,
                                                    Beschrijving = contactgegevenWerdGewijzigd.Data.Beschrijving,
                                                    IsPrimair = contactgegevenWerdGewijzigd.Data.IsPrimair,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(
                                                c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<HoofdactiviteitenVerenigingsloketWerdenGewijzigd> hoofdactiviteitenVerenigingsloketWerdenGewijzigd,
        PowerBiExportDocument document)
    {
        document.HoofdactiviteitenVerenigingsloket = hoofdactiviteitenVerenigingsloketWerdenGewijzigd.Data.HoofdactiviteitenVerenigingsloket
           .Select(x => new HoofdactiviteitVerenigingsloket()
            {
                Code = x.Code,
                Naam = x.Naam,
            }).ToArray();
        document.DatumLaatsteAanpassing = hoofdactiviteitenVerenigingsloketWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd,
        PowerBiExportDocument document)
    {
        ++document.AantalVertegenwoordigers;
        document.DatumLaatsteAanpassing = vertegenwoordigerWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd,
        PowerBiExportDocument document)
    {
        --document.AantalVertegenwoordigers;
        document.DatumLaatsteAanpassing = vertegenwoordigerWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdUitgeschrevenUitPubliekeDatastroom,
        PowerBiExportDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;
        document.DatumLaatsteAanpassing = verenigingWerdUitgeschrevenUitPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdToegevoegdAanPubliekeDatastroom,
        PowerBiExportDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;
        document.DatumLaatsteAanpassing = verenigingWerdToegevoegdAanPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(BeheerVerenigingDetailMapper.MapLocatie(locatieWerdToegevoegd.Data.Locatie,
                                                                                    locatieWerdToegevoegd.Data.Bron,
                                                                                    document.VCode))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = locatieWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<LocatieWerdGewijzigd> locatieWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == locatieWerdGewijzigd.Data.Locatie.LocatieId,
                                         update: l => l with
                                         {
                                             IsPrimair = locatieWerdGewijzigd.Data.Locatie.IsPrimair,
                                             Locatietype = locatieWerdGewijzigd.Data.Locatie.Locatietype,
                                             Naam = locatieWerdGewijzigd.Data.Locatie.Naam,
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 locatieWerdGewijzigd.Data.Locatie.Adres, document.VCode, l.LocatieId),
                                             Adresvoorstelling = AdresFormatter.ToAdresString(locatieWerdGewijzigd.Data.Locatie.Adres),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 locatieWerdGewijzigd.Data.Locatie.AdresId),
                                             VerwijstNaar =
                                             BeheerVerenigingDetailMapper.MapAdresVerwijzing(locatieWerdGewijzigd.Data.Locatie.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = locatieWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdVerwijderd.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = locatieWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Append(BeheerVerenigingDetailMapper.MapLocatie(maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie,
                                                                                    maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Bron,
                                                                                    document.VCode))
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = maatschappelijkeZetelWerdOvergenomenUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<MaatschappelijkeZetelVolgensKBOWerdGewijzigd> maatschappelijkeZetelVolgensKboWerdGewijzigd,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.LocatieId,
                                         update: l => l with
                                         {
                                             IsPrimair = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.IsPrimair,
                                             Naam = maatschappelijkeZetelVolgensKboWerdGewijzigd.Data.Naam,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = maatschappelijkeZetelVolgensKboWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> contactgegevenWerdToegevoegd,
        PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.Append(
                                                new Contactgegeven
                                                {
                                                    JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(
                                                        JsonLdType.Contactgegeven, document.VCode,
                                                        contactgegevenWerdToegevoegd.Data.ContactgegevenId.ToString()),
                                                    ContactgegevenId = contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                                                    Contactgegeventype = contactgegevenWerdToegevoegd.Data.Contactgegeventype,
                                                    Beschrijving = string.Empty,
                                                    Waarde = contactgegevenWerdToegevoegd.Data.Waarde,
                                                    Bron = contactgegevenWerdToegevoegd.Data.Bron,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<ContactgegevenUitKBOWerdGewijzigd> contactgegevenUitKboWerdGewijzigd,
        PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.UpdateSingle(
                                                identityFunc: c
                                                    => c.ContactgegevenId == contactgegevenUitKboWerdGewijzigd.Data.ContactgegevenId,
                                                update: contactgegeven => contactgegeven with
                                                {
                                                    IsPrimair = contactgegevenUitKboWerdGewijzigd.Data.IsPrimair,
                                                    Beschrijving = contactgegevenUitKboWerdGewijzigd.Data.Beschrijving,
                                                }
                                            ).OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenUitKboWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, PowerBiExportDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly);
        document.DatumLaatsteAanpassing = verenigingWerdGestopt.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<VerenigingWerdGestoptInKBO> verenigingWerdGestoptInKbo, PowerBiExportDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestoptInKbo.Data.Einddatum.ToString(WellknownFormats.DateOnly);
        document.DatumLaatsteAanpassing = verenigingWerdGestoptInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<EinddatumWerdGewijzigd> einddatumWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Einddatum = einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly);
        document.DatumLaatsteAanpassing = einddatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        PowerBiExportDocument document)
    {
        ++document.AantalVertegenwoordigers;
        document.DatumLaatsteAanpassing = vertegenwoordigerWerdOvergenomenUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.Naam = naamWerdGewijzigdInKbo.Data.Naam;
        document.DatumLaatsteAanpassing = naamWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigdInKbo> korteNaamWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigdInKbo.Data.KorteNaam;
        document.DatumLaatsteAanpassing = korteNaamWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<ContactgegevenWerdGewijzigdInKbo> contactgegevenWerdGewijzigdUitKbo,
        PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens.UpdateSingle(
                                                identityFunc: c
                                                    => c.ContactgegevenId == contactgegevenWerdGewijzigdUitKbo.Data.ContactgegevenId,
                                                update: contactgegeven => contactgegeven with
                                                {
                                                    Waarde = contactgegevenWerdGewijzigdUitKbo.Data.Waarde,
                                                }
                                            ).OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdGewijzigdUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<ContactgegevenWerdVerwijderdUitKBO> contactgegevenWerdVerwijderdUitKbo,
        PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(
                                                c => c.ContactgegevenId != contactgegevenWerdVerwijderdUitKbo.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdVerwijderdUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<ContactgegevenWerdInBeheerGenomenDoorKbo> contactgegevenWerdInBeheerGenomenDoorKbo,
        PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .UpdateSingle(
                                                identityFunc: c
                                                    => c.ContactgegevenId == contactgegevenWerdInBeheerGenomenDoorKbo.Data.ContactgegevenId,
                                                update: c => c with
                                                {
                                                    Bron = contactgegevenWerdInBeheerGenomenDoorKbo.Data.Bron,
                                                })
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();
        document.DatumLaatsteAanpassing = contactgegevenWerdInBeheerGenomenDoorKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<MaatschappelijkeZetelWerdGewijzigdInKbo> maatschappelijkeZetelWerdGewijzigdInKbo,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres, document.VCode, l.LocatieId),
                                             Adresvoorstelling = AdresFormatter.ToAdresString(maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId),
                                             VerwijstNaar =
                                             BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = maatschappelijkeZetelWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();

    }

    public void Apply(
        IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> maatschappelijkeZetelWerdVerwijderdUitKbo,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != maatschappelijkeZetelWerdVerwijderdUitKbo.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = maatschappelijkeZetelWerdVerwijderdUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<AdresWerdOvergenomenUitAdressenregister> adresWerdOvergenomenUitAdressenregister,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdOvergenomenUitAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 adresWerdOvergenomenUitAdressenregister.Data.Adres, document.VCode,
                                                 l.LocatieId),
                                             Adresvoorstelling = AdresFormatter.ToAdresString(adresWerdOvergenomenUitAdressenregister.Data.Adres),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                             VerwijstNaar = BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = adresWerdOvergenomenUitAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<AdresWerdGewijzigdInAdressenregister> adresWerdGewijzigdInAdressenregister,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdGewijzigdInAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             Adres = BeheerVerenigingDetailMapper.MapAdres(
                                                 adresWerdGewijzigdInAdressenregister.Data.Adres, document.VCode,
                                                 l.LocatieId),
                                             Adresvoorstelling = AdresFormatter.ToAdresString(adresWerdGewijzigdInAdressenregister.Data.Adres),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                             VerwijstNaar = BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = adresWerdGewijzigdInAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.Verenigingstype = new VerenigingsType
        {
            Code = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam,
        };

        document.Rechtsvorm = rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm;
        document.DatumLaatsteAanpassing = rechtsvormWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();

    }

    public void Apply(
        IEvent<AdresWerdNietGevondenInAdressenregister> adresWerdNietGevondenInAdressenregister,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdNietGevondenInAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = adresWerdNietGevondenInAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<AdresNietUniekInAdressenregister> adresNietUniekInAdressenregister,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresNietUniekInAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = adresNietUniekInAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<AdresKonNietOvergenomenWordenUitAdressenregister> adresKonNietOvergenomenWordenUitAdressenregister,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingleOrNothing(
                                         identityFunc: l => l.LocatieId == adresKonNietOvergenomenWordenUitAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = adresKonNietOvergenomenWordenUitAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<AdresWerdOntkoppeldVanAdressenregister> adresWerdOntkoppeldVanAdressenregister,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .UpdateSingle(
                                         identityFunc: l => l.LocatieId == adresWerdOntkoppeldVanAdressenregister.Data.LocatieId,
                                         update: l => l with
                                         {
                                             AdresId = null,
                                             VerwijstNaar = null,
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = adresWerdOntkoppeldVanAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }


    public void Apply(
        IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> locatieDuplicaatWerdVerwijderdNaAdresMatch,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieDuplicaatWerdVerwijderdNaAdresMatch.Data.VerwijderdeLocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();
        document.DatumLaatsteAanpassing = locatieDuplicaatWerdVerwijderdNaAdresMatch.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }

    public void Apply(
        IEvent<VerenigingWerdVerwijderd> verenigingWerdVerwijderd,
        PowerBiExportDocument document)
    {
        document.Status = StatusVerwijderd;
        document.DatumLaatsteAanpassing = verenigingWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDate();
    }
}
