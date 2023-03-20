namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Framework;
using Detail;
using Events;
using Infrastructure.Extensions;
using JasperFx.Core;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;
using IEvent = Marten.Events.IEvent;

public class BeheerVerenigingHistoriekProjection : SingleStreamAggregation<BeheerVerenigingHistoriekDocument>
{
    public BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
        => AddHistoriekEntry(
            verenigingWerdGeregistreerd,
            (initiator, tijdstip) => $"Vereniging werd aangemaakt met naam '{verenigingWerdGeregistreerd.Data.Naam}' door {initiator} op datum {tijdstip}.",
            new BeheerVerenigingHistoriekDocument
            {
                VCode = verenigingWerdGeregistreerd.Data.VCode,
                Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
                Metadata = new Metadata(0, 0),
            });

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            naamWerdGewijzigd,
            (initiator, tijdstip) => $"Naam vereniging werd gewijzigd naar '{naamWerdGewijzigd.Data.Naam}' door {initiator} op datum {tijdstip}.",
            document);

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteNaamWerdGewijzigd,
            (initiator, tijdstip) =>
                $"Korte naam vereniging werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}' door {initiator} op datum {tijdstip}.",
            document);

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteBeschrijvingWerdGewijzigd,
            (initiator, tijdstip) =>
                $"Korte beschrijving vereniging werd gewijzigd naar '{korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}' door {initiator} op datum {tijdstip}.",
            document);


    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            startdatumWerdGewijzigd,
            (initiator, tijdstip) =>
                $"Startdatum vereniging werd gewijzigd naar '{startdatumWerdGewijzigd.Data.Startdatum!.Value.ToString(WellknownFormats.DateOnly)}' door {initiator} op datum {tijdstip}.",
            document);

    public void Apply(IEvent<ContactInfoLijstWerdGewijzigd> contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        contactInfoLijstWerdGewijzigd.Data.Toevoegingen.ForEach(
            toevoeging => AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                (initiator, tijdstip) =>
                    $"Contactinfo vereniging werd toegevoegd met naam '{toevoeging.Contactnaam}' door {initiator} op datum {tijdstip}.",
                document));
        contactInfoLijstWerdGewijzigd.Data.Verwijderingen.ForEach(
            toevoeging => AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                (initiator, tijdstip) =>
                    $"Contactinfo vereniging werd verwijderd met naam '{toevoeging.Contactnaam}' door {initiator} op datum {tijdstip}.",
                document));
        contactInfoLijstWerdGewijzigd.Data.Wijzigingen.ForEach(
            wijziging =>
            {
                if (wijziging.Email is not null)
                    AddHistoriekEntry(
                        contactInfoLijstWerdGewijzigd,
                        (initiator, tijdstip) =>
                            $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Email' werd gewijzgid naar '{wijziging.Email}', door {initiator} op datum {tijdstip}.",
                        document);
                if (wijziging.Telefoon is not null)
                    AddHistoriekEntry(
                        contactInfoLijstWerdGewijzigd,
                        (initiator, tijdstip) =>
                            $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzgid naar '{wijziging.Telefoon}', door {initiator} op datum {tijdstip}.",
                        document);
                if (wijziging.Website is not null)
                    AddHistoriekEntry(
                        contactInfoLijstWerdGewijzigd,
                        (initiator, tijdstip) =>
                            $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Website' werd gewijzgid naar '{wijziging.Website}', door {initiator} op datum {tijdstip}.",
                        document);
                if (wijziging.SocialMedia is not null)
                    AddHistoriekEntry(
                        contactInfoLijstWerdGewijzigd,
                        (initiator, tijdstip) =>
                            $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzgid naar '{wijziging.SocialMedia}', door {initiator} op datum {tijdstip}.",
                        document);
                if (wijziging.PrimairContactInfo)
                    AddHistoriekEntry(
                        contactInfoLijstWerdGewijzigd,
                        (initiator, tijdstip) =>
                            $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd als primair aangeduid door {initiator} op datum {tijdstip}.",
                        document);
            });
    }

    private static BeheerVerenigingHistoriekDocument AddHistoriekEntry(IEvent verenigingWerdGeregistreerd, Func<string?, string?, string> gebeurtenis, BeheerVerenigingHistoriekDocument document)
    {
        var initiator = verenigingWerdGeregistreerd.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = verenigingWerdGeregistreerd.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                gebeurtenis(initiator, tijdstip),
                initiator,
                tijdstip
            )).ToList();
        document.Metadata = new Metadata(verenigingWerdGeregistreerd.Sequence, verenigingWerdGeregistreerd.Version);

        return document;
    }
}

// TODO bekijken of Metadata weg kan?
public class BeheerVerenigingHistoriekDocument : IMetadata, IVCode
{
    [Identity] public string VCode { get; set; } = null!;
    public List<BeheerVerenigingHistoriekGebeurtenis> Gebeurtenissen { get; set; } = null!;
    public Metadata Metadata { get; set; } = null!;
}

public record BeheerVerenigingHistoriekGebeurtenis(
    string Gebeurtenis,
    string Initiator,
    string Tijdstip);
