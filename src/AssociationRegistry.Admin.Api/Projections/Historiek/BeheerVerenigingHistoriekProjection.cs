namespace AssociationRegistry.Admin.Api.Projections.Historiek;

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
            $"Vereniging werd geregistreerd met naam '{verenigingWerdGeregistreerd.Data.Naam}'.",
            beheerVerenigingHistoriekDocument);

        return beheerVerenigingHistoriekDocument;
    }

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            naamWerdGewijzigd,
            $"Naam werd gewijzigd naar '{naamWerdGewijzigd.Data.Naam}'.",
            document);

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteNaamWerdGewijzigd,
            $"Korte naam werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}'.",
            document);

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteBeschrijvingWerdGewijzigd,
            $"Korte beschrijving werd gewijzigd naar '{korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}'.",
            document);


    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            startdatumWerdGewijzigd,
            $"Startdatum werd gewijzigd naar '{(startdatumWerdGewijzigd.Data.Startdatum is { } startdatum ? startdatum.ToString(WellknownFormats.DateOnly) : string.Empty)}'.",
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

    private static void AddHistoriekEntry(IEvent @event, string beschrijving, BeheerVerenigingHistoriekDocument document)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                beschrijving,
                @event.Data.GetType().Name,
                initiator,
                tijdstip
            )).ToList();
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }

    private static void AddToevoegingContactInfoHistoriekEntry(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo toevoeging)
    {
        AddHistoriekEntry(
            contactInfoLijstWerdGewijzigd,
            $"Contactinfo met naam '{toevoeging.Contactnaam}' werd toegevoegd.",
            document);
    }

    private static void AddWijzigContactInfoHistoriekEntries(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo wijziging)
    {
        if (wijziging.Email is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{wijziging.Email}'.",
                document);
        if (wijziging.Telefoon is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{wijziging.Telefoon}'.",
                document);
        if (wijziging.Website is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{wijziging.Website}'.",
                document);
        if (wijziging.SocialMedia is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{wijziging.SocialMedia}'.",
                document);
        if (wijziging.PrimairContactInfo)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd als primair aangeduid.",
                document);
    }

    private static void AddVerwijderingContactInfoHistoriekEntry(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo toevoeging)
    {
        AddHistoriekEntry(
            contactInfoLijstWerdGewijzigd,
            $"Contactinfo met naam '{toevoeging.Contactnaam}' werd verwijderd.",
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
    string Beschrijving,
    string Gebeurtenis,
    string Initiator,
    string Tijdstip);
