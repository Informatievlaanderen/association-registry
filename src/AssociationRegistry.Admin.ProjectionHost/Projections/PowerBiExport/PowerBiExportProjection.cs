namespace AssociationRegistry.Admin.ProjectionHost.Projections.PowerBiExport;

using Detail;
using Events;
using Formats;
using Framework;
using JasperFx.Core;
using JsonLdContext;
using Marten.Events;
using Marten.Events.Aggregation;
using Schema;
using Schema.PowerBiExport;
using Vereniging;
using Contactgegeven = Schema.Detail.Contactgegeven;
using Doelgroep = Schema.Detail.Doelgroep;
using HoofdactiviteitVerenigingsloket = Schema.PowerBiExport.HoofdactiviteitVerenigingsloket;
using IEvent = Marten.Events.IEvent;
using Lidmaatschap = Schema.PowerBiExport.Lidmaatschap;
using Locatie = Schema.Detail.Locatie;
using VerenigingStatus = Schema.Constants.VerenigingStatus;
using Werkingsgebied = Schema.PowerBiExport.Werkingsgebied;

public class PowerBiExportProjection : SingleStreamProjection<PowerBiExportDocument>
{
    public const string StatusVerwijderd = "Verwijderd";

    private static void UpdateHistoriek(PowerBiExportDocument document, IEvent @event)
        => document.Historiek = document.Historiek.Append(Gebeurtenis.FromEvent(@event));

    public PowerBiExportDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> feitelijkeVerenigingWerdGeregistreerd)
    {
        var document = new PowerBiExportDocument()
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
            Werkingsgebieden = [],
            Bron = feitelijkeVerenigingWerdGeregistreerd.Data.Bron,
            DatumLaatsteAanpassing = feitelijkeVerenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                          .ConvertAndFormatToBelgianDate(),
        };

        UpdateHistoriek(document, feitelijkeVerenigingWerdGeregistreerd);

        return document;
    }

    public PowerBiExportDocument Create(IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> @event)
    {
        var document = new PowerBiExportDocument()
        {
            VCode = @event.Data.VCode,
            Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.VZER),
            Naam = @event.Data.Naam,
            KorteNaam = @event.Data.KorteNaam,
            KorteBeschrijving = @event.Data.KorteBeschrijving,
            Startdatum = @event.Data.Startdatum?.ToString(WellknownFormats.DateOnly),
            Doelgroep = BeheerVerenigingDetailMapper.MapDoelgroep(@event.Data.Doelgroep,
                                                                  @event.Data.VCode),
            Status = VerenigingStatus.Actief,
            IsUitgeschrevenUitPubliekeDatastroom = @event.Data.IsUitgeschrevenUitPubliekeDatastroom,
            Contactgegevens = @event.Data.Contactgegevens
                                                                   .Select(c => BeheerVerenigingDetailMapper.MapContactgegeven(
                                                                               c, @event.Data.Bron,
                                                                               @event.Data.VCode))
                                                                   .ToArray(),
            Locaties = @event.Data.Locaties
                                                            .Select(loc => BeheerVerenigingDetailMapper.MapLocatie(
                                                                        loc, @event.Data.Bron,
                                                                        @event.Data.VCode)).ToArray(),
            AantalVertegenwoordigers = @event.Data.Vertegenwoordigers.Length,
            HoofdactiviteitenVerenigingsloket = @event.Data
                                                                                     .HoofdactiviteitenVerenigingsloket
                                                                                     .Select(x => new HoofdactiviteitVerenigingsloket()
                                                                                      {
                                                                                          Code = x.Code,
                                                                                          Naam = x.Naam,
                                                                                      })
                                                                                     .ToArray(),
            Werkingsgebieden = [],
            Bron = @event.Data.Bron,
            DatumLaatsteAanpassing = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                          .ConvertAndFormatToBelgianDate(),
        };

        UpdateHistoriek(document, @event);

        return document;
    }

    public PowerBiExportDocument Create(
        IEvent<VerenigingMetRechtspersoonlijkheidWerdGeregistreerd> verenigingMetRechtspersoonlijkheidWerdGeregistreerd)
    {
        var document = new PowerBiExportDocument
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
            Werkingsgebieden = Array.Empty<Werkingsgebied>(),
            Bron = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.Bron,
            KboNummer = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.Data.KboNummer,
            DatumLaatsteAanpassing = verenigingMetRechtspersoonlijkheidWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                        .ConvertAndFormatToBelgianDate(),
        };

        UpdateHistoriek(document, verenigingMetRechtspersoonlijkheidWerdGeregistreerd);

        return document;
    }

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Naam = naamWerdGewijzigd.Data.Naam;
        document.DatumLaatsteAanpassing = naamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();
        UpdateHistoriek(document, naamWerdGewijzigd);
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, PowerBiExportDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigd.Data.KorteNaam;

        document.DatumLaatsteAanpassing =
            korteNaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, korteNaamWerdGewijzigd);
    }

    public void Apply(IEvent<RoepnaamWerdGewijzigd> roepnaamWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Roepnaam = roepnaamWerdGewijzigd.Data.Roepnaam;

        document.DatumLaatsteAanpassing =
            roepnaamWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, roepnaamWerdGewijzigd);
    }

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, PowerBiExportDocument document)
    {
        document.KorteBeschrijving = korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving;

        document.DatumLaatsteAanpassing = korteBeschrijvingWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                        .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, korteBeschrijvingWerdGewijzigd);
    }

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigd.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigd.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;

        document.DatumLaatsteAanpassing =
            startdatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, startdatumWerdGewijzigd);
    }

    public void Apply(IEvent<StartdatumWerdGewijzigdInKbo> startdatumWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.Startdatum = !string.IsNullOrEmpty(startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString())
            ? startdatumWerdGewijzigdInKbo.Data.Startdatum?.ToString(WellknownFormats.DateOnly)
            : null;

        document.DatumLaatsteAanpassing =
            startdatumWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, startdatumWerdGewijzigdInKbo);
    }

    public void Apply(IEvent<DoelgroepWerdGewijzigd> doelgroepWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Doelgroep = new Doelgroep
        {
            JsonLdMetadata = BeheerVerenigingDetailMapper.CreateJsonLdMetadata(JsonLdType.Doelgroep, doelgroepWerdGewijzigd.StreamKey!),
            Minimumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Minimumleeftijd,
            Maximumleeftijd = doelgroepWerdGewijzigd.Data.Doelgroep.Maximumleeftijd,
        };

        document.DatumLaatsteAanpassing =
            doelgroepWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, doelgroepWerdGewijzigd);
    }

    public void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, PowerBiExportDocument document)
    {
        document.Contactgegevens = Enumerable.Append(document.Contactgegevens, new Contactgegeven
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

        document.DatumLaatsteAanpassing =
            contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenWerdToegevoegd);
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

        document.DatumLaatsteAanpassing =
            contactgegevenWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenWerdGewijzigd);
    }

    public void Apply(IEvent<ContactgegevenWerdVerwijderd> contactgegevenWerdVerwijderd, PowerBiExportDocument document)
    {
        document.Contactgegevens = document.Contactgegevens
                                           .Where(
                                                c => c.ContactgegevenId != contactgegevenWerdVerwijderd.Data.ContactgegevenId)
                                           .OrderBy(c => c.ContactgegevenId)
                                           .ToArray();

        document.DatumLaatsteAanpassing =
            contactgegevenWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenWerdVerwijderd);
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

        document.DatumLaatsteAanpassing = hoofdactiviteitenVerenigingsloketWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                          .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, hoofdactiviteitenVerenigingsloketWerdenGewijzigd);
    }

    public void Apply(
        IEvent<VertegenwoordigerWerdToegevoegd> vertegenwoordigerWerdToegevoegd,
        PowerBiExportDocument document)
    {
        ++document.AantalVertegenwoordigers;

        document.DatumLaatsteAanpassing = vertegenwoordigerWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                         .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, vertegenwoordigerWerdToegevoegd);
    }

    public void Apply(
        IEvent<VertegenwoordigerWerdGewijzigd> @event,
        PowerBiExportDocument document)
    {
        document.DatumLaatsteAanpassing = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                         .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(
        IEvent<VertegenwoordigerWerdVerwijderd> vertegenwoordigerWerdVerwijderd,
        PowerBiExportDocument document)
    {
        --document.AantalVertegenwoordigers;

        document.DatumLaatsteAanpassing = vertegenwoordigerWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                         .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, vertegenwoordigerWerdVerwijderd);
    }

    public void Apply(
        IEvent<VerenigingWerdUitgeschrevenUitPubliekeDatastroom> verenigingWerdUitgeschrevenUitPubliekeDatastroom,
        PowerBiExportDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = true;

        document.DatumLaatsteAanpassing = verenigingWerdUitgeschrevenUitPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                          .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, verenigingWerdUitgeschrevenUitPubliekeDatastroom);
    }

    public void Apply(
        IEvent<VerenigingWerdIngeschrevenInPubliekeDatastroom> verenigingWerdToegevoegdAanPubliekeDatastroom,
        PowerBiExportDocument document)
    {
        document.IsUitgeschrevenUitPubliekeDatastroom = false;

        document.DatumLaatsteAanpassing =
            verenigingWerdToegevoegdAanPubliekeDatastroom.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, verenigingWerdToegevoegdAanPubliekeDatastroom);
    }

    public void Apply(IEvent<LocatieWerdToegevoegd> locatieWerdToegevoegd, PowerBiExportDocument document)
    {
        document.Locaties = Enumerable.Append(document.Locaties, BeheerVerenigingDetailMapper.MapLocatie(locatieWerdToegevoegd.Data.Locatie,
                                                  locatieWerdToegevoegd.Data.Bron,
                                                  document.VCode))
                                      .OrderBy(l => l.LocatieId)
                                      .ToArray();

        document.DatumLaatsteAanpassing =
            locatieWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, locatieWerdToegevoegd);
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

        document.DatumLaatsteAanpassing =
            locatieWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, locatieWerdGewijzigd);
    }

    public void Apply(IEvent<LocatieWerdVerwijderd> locatieWerdVerwijderd, PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieWerdVerwijderd.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            locatieWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, locatieWerdVerwijderd);
    }

    public void Apply(
        IEvent<MaatschappelijkeZetelWerdOvergenomenUitKbo> maatschappelijkeZetelWerdOvergenomenUitKbo,
        PowerBiExportDocument document)
    {
        document.Locaties = Enumerable.Append(document.Locaties, BeheerVerenigingDetailMapper.MapLocatie(
                                                  maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Locatie,
                                                  maatschappelijkeZetelWerdOvergenomenUitKbo.Data.Bron,
                                                  document.VCode))
                                      .OrderBy(l => l.LocatieId)
                                      .ToArray();

        document.DatumLaatsteAanpassing =
            maatschappelijkeZetelWerdOvergenomenUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, maatschappelijkeZetelWerdOvergenomenUitKbo);
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

        document.DatumLaatsteAanpassing =
            maatschappelijkeZetelVolgensKboWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, maatschappelijkeZetelVolgensKboWerdGewijzigd);
    }

    public void Apply(
        IEvent<ContactgegevenWerdOvergenomenUitKBO> contactgegevenWerdToegevoegd,
        PowerBiExportDocument document)
    {
        document.Contactgegevens = Enumerable.Append(document.Contactgegevens, new Contactgegeven
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

        document.DatumLaatsteAanpassing =
            contactgegevenWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenWerdToegevoegd);
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

        document.DatumLaatsteAanpassing = contactgegevenUitKboWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                           .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenUitKboWerdGewijzigd);
    }

    public void Apply(IEvent<VerenigingWerdGestopt> verenigingWerdGestopt, PowerBiExportDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestopt.Data.Einddatum.ToString(WellknownFormats.DateOnly);

        document.DatumLaatsteAanpassing =
            verenigingWerdGestopt.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, verenigingWerdGestopt);
    }

    public void Apply(IEvent<VerenigingWerdGestoptInKBO> verenigingWerdGestoptInKbo, PowerBiExportDocument document)
    {
        document.Status = VerenigingStatus.Gestopt;
        document.Einddatum = verenigingWerdGestoptInKbo.Data.Einddatum.ToString(WellknownFormats.DateOnly);

        document.DatumLaatsteAanpassing =
            verenigingWerdGestoptInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, verenigingWerdGestoptInKbo);
    }

    public void Apply(IEvent<EinddatumWerdGewijzigd> einddatumWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Einddatum = einddatumWerdGewijzigd.Data.Einddatum.ToString(WellknownFormats.DateOnly);

        document.DatumLaatsteAanpassing =
            einddatumWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, einddatumWerdGewijzigd);
    }

    public void Apply(
        IEvent<VertegenwoordigerWerdOvergenomenUitKBO> vertegenwoordigerWerdOvergenomenUitKbo,
        PowerBiExportDocument document)
    {
        ++document.AantalVertegenwoordigers;

        document.DatumLaatsteAanpassing =
            vertegenwoordigerWerdOvergenomenUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, vertegenwoordigerWerdOvergenomenUitKbo);
    }

    public void Apply(IEvent<NaamWerdGewijzigdInKbo> naamWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.Naam = naamWerdGewijzigdInKbo.Data.Naam;

        document.DatumLaatsteAanpassing =
            naamWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, naamWerdGewijzigdInKbo);
    }

    public void Apply(IEvent<KorteNaamWerdGewijzigdInKbo> korteNaamWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.KorteNaam = korteNaamWerdGewijzigdInKbo.Data.KorteNaam;

        document.DatumLaatsteAanpassing =
            korteNaamWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, korteNaamWerdGewijzigdInKbo);
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

        document.DatumLaatsteAanpassing = contactgegevenWerdGewijzigdUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                           .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenWerdGewijzigdUitKbo);
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

        document.DatumLaatsteAanpassing = contactgegevenWerdVerwijderdUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                            .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenWerdVerwijderdUitKbo);
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

        document.DatumLaatsteAanpassing =
            contactgegevenWerdInBeheerGenomenDoorKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, contactgegevenWerdInBeheerGenomenDoorKbo);
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
                                             Adresvoorstelling =
                                             AdresFormatter.ToAdresString(maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.Adres),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId),
                                             VerwijstNaar =
                                             BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 maatschappelijkeZetelWerdGewijzigdInKbo.Data.Locatie.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            maatschappelijkeZetelWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, maatschappelijkeZetelWerdGewijzigdInKbo);
    }

    public void Apply(
        IEvent<MaatschappelijkeZetelWerdVerwijderdUitKbo> maatschappelijkeZetelWerdVerwijderdUitKbo,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != maatschappelijkeZetelWerdVerwijderdUitKbo.Data.Locatie.LocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            maatschappelijkeZetelWerdVerwijderdUitKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, maatschappelijkeZetelWerdVerwijderdUitKbo);
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
                                             Adresvoorstelling =
                                             AdresFormatter.ToAdresString(adresWerdOvergenomenUitAdressenregister.Data.Adres),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                             VerwijstNaar = BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 adresWerdOvergenomenUitAdressenregister.Data.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            adresWerdOvergenomenUitAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, adresWerdOvergenomenUitAdressenregister);
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
                                             Adresvoorstelling =
                                             AdresFormatter.ToAdresString(adresWerdGewijzigdInAdressenregister.Data.Adres),
                                             AdresId = BeheerVerenigingDetailMapper.MapAdresId(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                             VerwijstNaar = BeheerVerenigingDetailMapper.MapAdresVerwijzing(
                                                 adresWerdGewijzigdInAdressenregister.Data.AdresId),
                                         })
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            adresWerdGewijzigdInAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, adresWerdGewijzigdInAdressenregister);
    }

    public void Apply(IEvent<RechtsvormWerdGewijzigdInKBO> rechtsvormWerdGewijzigdInKbo, PowerBiExportDocument document)
    {
        document.Verenigingstype = new VerenigingsType
        {
            Code = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Code,
            Naam = Verenigingstype.Parse(rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm).Naam,
        };

        document.Rechtsvorm = rechtsvormWerdGewijzigdInKbo.Data.Rechtsvorm;

        document.DatumLaatsteAanpassing =
            rechtsvormWerdGewijzigdInKbo.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, rechtsvormWerdGewijzigdInKbo);
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

        document.DatumLaatsteAanpassing =
            adresWerdNietGevondenInAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, adresWerdNietGevondenInAdressenregister);
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

        document.DatumLaatsteAanpassing = adresNietUniekInAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                          .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, adresNietUniekInAdressenregister);
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

        document.DatumLaatsteAanpassing = adresKonNietOvergenomenWordenUitAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                          .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, adresKonNietOvergenomenWordenUitAdressenregister);
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

        document.DatumLaatsteAanpassing =
            adresWerdOntkoppeldVanAdressenregister.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, adresWerdOntkoppeldVanAdressenregister);
    }

    public void Apply(
        IEvent<LocatieDuplicaatWerdVerwijderdNaAdresMatch> locatieDuplicaatWerdVerwijderdNaAdresMatch,
        PowerBiExportDocument document)
    {
        document.Locaties = document.Locaties
                                    .Where(l => l.LocatieId != locatieDuplicaatWerdVerwijderdNaAdresMatch.Data.VerwijderdeLocatieId)
                                    .OrderBy(l => l.LocatieId)
                                    .ToArray();

        document.DatumLaatsteAanpassing =
            locatieDuplicaatWerdVerwijderdNaAdresMatch.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, locatieDuplicaatWerdVerwijderdNaAdresMatch);
    }

    public void Apply(
        IEvent<VerenigingWerdVerwijderd> verenigingWerdVerwijderd,
        PowerBiExportDocument document)
    {
        document.Status = StatusVerwijderd;

        document.DatumLaatsteAanpassing =
            verenigingWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, verenigingWerdVerwijderd);
    }

    public void Apply(
        IEvent<WerkingsgebiedenWerdenNietBepaald> werkingsgebiedenWerdenNietBepaald,
        PowerBiExportDocument document)
    {
        document.Werkingsgebieden = [];

        document.DatumLaatsteAanpassing = werkingsgebiedenWerdenNietBepaald.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                           .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, werkingsgebiedenWerdenNietBepaald);
    }

    public void Apply(
        IEvent<WerkingsgebiedenWerdenBepaald> werkingsgebiedenWerdenBepaald,
        PowerBiExportDocument document)
    {
        document.Werkingsgebieden = werkingsgebiedenWerdenBepaald.Data.Werkingsgebieden
                                                                 .Select(x => new Werkingsgebied()
                                                                  {
                                                                      Code = x.Code,
                                                                      Naam = x.Naam,
                                                                  }).ToArray();

        document.DatumLaatsteAanpassing = werkingsgebiedenWerdenBepaald.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                       .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, werkingsgebiedenWerdenBepaald);
    }

    public void Apply(
        IEvent<WerkingsgebiedenWerdenGewijzigd> werkingsgebiedenWerdenGewijzigd,
        PowerBiExportDocument document)
    {
        document.Werkingsgebieden = werkingsgebiedenWerdenGewijzigd.Data.Werkingsgebieden
                                                                   .Select(x => new Werkingsgebied()
                                                                    {
                                                                        Code = x.Code,
                                                                        Naam = x.Naam,
                                                                    }).ToArray();

        document.DatumLaatsteAanpassing = werkingsgebiedenWerdenGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                         .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, werkingsgebiedenWerdenGewijzigd);
    }

    public void Apply(
        IEvent<WerkingsgebiedenWerdenNietVanToepassing> werkingsgebiedenWerdenNietVanToepassing,
        PowerBiExportDocument document)
    {
        document.Werkingsgebieden =
        [
            new Werkingsgebied
            {
                Code = AssociationRegistry.Vereniging.Werkingsgebied.NietVanToepassing.Code,
                Naam = AssociationRegistry.Vereniging.Werkingsgebied.NietVanToepassing.Naam,
            }
        ];

        document.DatumLaatsteAanpassing = werkingsgebiedenWerdenNietVanToepassing.GetHeaderInstant(MetadataHeaderNames.Tijdstip)
                                                                                 .ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, werkingsgebiedenWerdenNietVanToepassing);
    }

    public void Apply(IEvent<LidmaatschapWerdToegevoegd> lidmaatschapWerdToegevoegd, PowerBiExportDocument document)
    {
        document.Lidmaatschappen = Enumerable
                                  .Append(document.Lidmaatschappen, new Lidmaatschap(
                                              lidmaatschapWerdToegevoegd.Data.Lidmaatschap.LidmaatschapId,
                                              lidmaatschapWerdToegevoegd.Data.Lidmaatschap.AndereVereniging,
                                              lidmaatschapWerdToegevoegd.Data.Lidmaatschap.DatumVan.FormatAsBelgianDate(),
                                              lidmaatschapWerdToegevoegd.Data.Lidmaatschap.DatumTot.FormatAsBelgianDate(),
                                              lidmaatschapWerdToegevoegd.Data.Lidmaatschap.Identificatie,
                                              lidmaatschapWerdToegevoegd.Data.Lidmaatschap.Beschrijving
                                          ))
                                  .OrderBy(l => l.LidmaatschapId)
                                  .ToArray();

        document.DatumLaatsteAanpassing =
            lidmaatschapWerdToegevoegd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, lidmaatschapWerdToegevoegd);
    }

    public void Apply(IEvent<LidmaatschapWerdGewijzigd> lidmaatschapWerdGewijzigd, PowerBiExportDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .UpdateSingle(
                                                identityFunc: l
                                                    => l.LidmaatschapId == lidmaatschapWerdGewijzigd.Data.Lidmaatschap.LidmaatschapId,
                                                update: l => l with
                                                {
                                                    Van = lidmaatschapWerdGewijzigd.Data.Lidmaatschap.DatumVan.FormatAsBelgianDate(),
                                                    Tot = lidmaatschapWerdGewijzigd.Data.Lidmaatschap.DatumTot.FormatAsBelgianDate(),
                                                    Identificatie = lidmaatschapWerdGewijzigd.Data.Lidmaatschap.Identificatie,
                                                    Beschrijving = lidmaatschapWerdGewijzigd.Data.Lidmaatschap.Beschrijving
                                                })
                                           .OrderBy(l => l.LidmaatschapId)
                                           .ToArray();

        document.DatumLaatsteAanpassing =
            lidmaatschapWerdGewijzigd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, lidmaatschapWerdGewijzigd);
    }

    public void Apply(IEvent<LidmaatschapWerdVerwijderd> lidmaatschapWerdVerwijderd, PowerBiExportDocument document)
    {
        document.Lidmaatschappen = document.Lidmaatschappen
                                           .Where(l => l.LidmaatschapId != lidmaatschapWerdVerwijderd.Data.Lidmaatschap.LidmaatschapId)
                                           .OrderBy(l => l.LidmaatschapId)
                                           .ToArray();

        document.DatumLaatsteAanpassing =
            lidmaatschapWerdVerwijderd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, lidmaatschapWerdVerwijderd);
    }

    public void Apply(IEvent<FeitelijkeVerenigingWerdGemigreerdNaarVerenigingZonderEigenRechtspersoonlijkheid> @event, PowerBiExportDocument document)
    {
        document.Verenigingstype = BeheerVerenigingDetailMapper.MapVerenigingsType(Verenigingstype.VZER);

        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<ContactgegevenKonNietOvergenomenWordenUitKBO> @event, PowerBiExportDocument document)
    {
        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<MaatschappelijkeZetelKonNietOvergenomenWordenUitKbo> @event, PowerBiExportDocument document)
    {
        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<AdresHeeftGeenVerschillenMetAdressenregister> @event, PowerBiExportDocument document)
    {
        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<MarkeringDubbeleVerengingWerdGecorrigeerd> @event, PowerBiExportDocument document)
    {
        document.Status = @event.Data.VorigeStatus;

        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<VerenigingAanvaarddeCorrectieDubbeleVereniging> @event, PowerBiExportDocument document)
    {
        document.CorresponderendeVCodes =
            document.CorresponderendeVCodes
                    .Where(x => x != @event.Data.VCodeDubbeleVereniging)
                    .ToArray();

        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<VerenigingWerdGemarkeerdAlsDubbelVan> @event, PowerBiExportDocument document)
    {
        document.Status = VerenigingStatus.Dubbel;

        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<VerenigingAanvaarddeDubbeleVereniging> @event, PowerBiExportDocument document)
    {
        document.CorresponderendeVCodes =
            document.CorresponderendeVCodes
                    .Append(@event.Data.VCodeDubbeleVereniging)
                    .ToArray();

        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<WeigeringDubbelDoorAuthentiekeVerenigingWerdVerwerkt> @event, PowerBiExportDocument document)
    {
        document.Status = @event.Data.VorigeStatus;

        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<SynchronisatieMetKboWasSuccesvol> @event, PowerBiExportDocument document)
    {
        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }

    public void Apply(IEvent<VerenigingWerdIngeschrevenOpWijzigingenUitKbo> @event, PowerBiExportDocument document)
    {
        document.DatumLaatsteAanpassing =
            @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ConvertAndFormatToBelgianDate();

        UpdateHistoriek(document, @event);
    }
}
