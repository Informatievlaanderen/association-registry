namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System;
using System.Collections.Generic;
using System.Linq;
using Constants;
using Framework;
using Detail;
using Events;
using Events.CommonEventDataTypes;
using Infrastructure.Extensions;
using JasperFx.Core;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Schema;
using IEvent = Marten.Events.IEvent;

public class BeheerVerenigingHistoriekProjection : SingleStreamAggregation<BeheerVerenigingHistoriekDocument>
{
    public BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = new BeheerVerenigingHistoriekDocument
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(0, 0),
        };

        AddHistoriekEntry(
            verenigingWerdGeregistreerd,
            (initiator, tijdstip) => $"Vereniging werd geregistreerd met naam '{verenigingWerdGeregistreerd.Data.Naam}' door {initiator} op datum {tijdstip}.",
            beheerVerenigingHistoriekDocument);

        return beheerVerenigingHistoriekDocument;
    }

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
                $"Startdatum vereniging werd gewijzigd naar '{(startdatumWerdGewijzigd.Data.Startdatum is { } startdatum ? startdatum.ToString(WellknownFormats.DateOnly) : string.Empty)}' door {initiator} op datum {tijdstip}.",
            document);

    public void Apply(IEvent<ContactInfoLijstWerdGewijzigd> contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        contactInfoLijstWerdGewijzigd.Data.Verwijderingen.ForEach(
            toevoeging => AddVerwijderingContactInfoHistoriekEntry(contactInfoLijstWerdGewijzigd, document, toevoeging));
        contactInfoLijstWerdGewijzigd.Data.Wijzigingen.ForEach(
            wijziging => AddWijzigContactInfoHistoriekEntries(contactInfoLijstWerdGewijzigd, document, wijziging));
        contactInfoLijstWerdGewijzigd.Data.Toevoegingen.ForEach(
            toevoeging => AddToevoegingContactInfoHistoriekEntry(contactInfoLijstWerdGewijzigd, document, toevoeging));

        document.Metadata = new Metadata(contactInfoLijstWerdGewijzigd.Sequence, contactInfoLijstWerdGewijzigd.Version);
    }

    private static void AddHistoriekEntry(IEvent @event, Func<string?, string?, string> gebeurtenis, BeheerVerenigingHistoriekDocument document)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                gebeurtenis(initiator, tijdstip),
                initiator,
                tijdstip
            )).ToList();
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }

    private static void AddToevoegingContactInfoHistoriekEntry(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo toevoeging)
    {
        AddHistoriekEntry(
            contactInfoLijstWerdGewijzigd,
            (initiator, tijdstip) =>
                $"Contactinfo vereniging werd toegevoegd met naam '{toevoeging.Contactnaam}' door {initiator} op datum {tijdstip}.",
            document);
    }

    private static void AddWijzigContactInfoHistoriekEntries(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo wijziging)
    {
        if (wijziging.Email is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                (initiator, tijdstip) =>
                    $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{wijziging.Email}', door {initiator} op datum {tijdstip}.",
                document);
        if (wijziging.Telefoon is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                (initiator, tijdstip) =>
                    $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{wijziging.Telefoon}', door {initiator} op datum {tijdstip}.",
                document);
        if (wijziging.Website is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                (initiator, tijdstip) =>
                    $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{wijziging.Website}', door {initiator} op datum {tijdstip}.",
                document);
        if (wijziging.SocialMedia is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                (initiator, tijdstip) =>
                    $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{wijziging.SocialMedia}', door {initiator} op datum {tijdstip}.",
                document);
        if (wijziging.PrimairContactInfo)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                (initiator, tijdstip) =>
                    $"Contactinfo vereniging met naam '{wijziging.Contactnaam}' werd als primair aangeduid door {initiator} op datum {tijdstip}.",
                document);
    }

    private static void AddVerwijderingContactInfoHistoriekEntry(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo toevoeging)
    {
        AddHistoriekEntry(
            contactInfoLijstWerdGewijzigd,
            (initiator, tijdstip) =>
                $"Contactinfo vereniging werd verwijderd met naam '{toevoeging.Contactnaam}' door {initiator} op datum {tijdstip}.",
            document);
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
