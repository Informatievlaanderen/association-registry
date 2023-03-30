namespace AssociationRegistry.Admin.Api.Projections.Historiek;

using System.Collections.Generic;
using System.Linq;
using Constants;
using Detail;
using Events;
using Events.CommonEventDataTypes;
using Framework;
using Infrastructure.Extensions;
using JasperFx.Core;
using Marten.Events;
using Marten.Events.Aggregation;
using Schema;
using IEvent = Marten.Events.IEvent;

public class BeheerVerenigingHistoriekProjection : SingleStreamAggregation<BeheerVerenigingHistoriekDocument>
{
    public BeheerVerenigingHistoriekDocument Create(IEvent<VerenigingWerdGeregistreerd> verenigingWerdGeregistreerd)
    {
        var beheerVerenigingHistoriekDocument = new BeheerVerenigingHistoriekDocument
        {
            VCode = verenigingWerdGeregistreerd.Data.VCode,
            Gebeurtenissen = new List<BeheerVerenigingHistoriekGebeurtenis>(),
            Metadata = new Metadata(Sequence: 0, Version: 0),
        };

        AddHistoriekEntry(
            verenigingWerdGeregistreerd,
            beheerVerenigingHistoriekDocument,
            $"Vereniging werd geregistreerd met naam '{verenigingWerdGeregistreerd.Data.Naam}'.",
            new VerenigingWerdgeregistreerdData(verenigingWerdGeregistreerd.Data));

        return beheerVerenigingHistoriekDocument;
    }

    public void Apply(IEvent<NaamWerdGewijzigd> naamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            naamWerdGewijzigd,
            document,
            $"Naam werd gewijzigd naar '{naamWerdGewijzigd.Data.Naam}'.",
            new NaamWerdGewijzigdData(naamWerdGewijzigd.Data.Naam));

    public void Apply(IEvent<KorteNaamWerdGewijzigd> korteNaamWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteNaamWerdGewijzigd,
            document,
            $"Korte naam werd gewijzigd naar '{korteNaamWerdGewijzigd.Data.KorteNaam}'.",
            new KorteNaamWerdGewijzigdData(korteNaamWerdGewijzigd.Data.KorteNaam));

    public void Apply(IEvent<KorteBeschrijvingWerdGewijzigd> korteBeschrijvingWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
        => AddHistoriekEntry(
            korteBeschrijvingWerdGewijzigd,
            document,
            $"Korte beschrijving werd gewijzigd naar '{korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving}'.",
            new KorteBeschrijvingWerdGewijzigdData(korteBeschrijvingWerdGewijzigd.Data.KorteBeschrijving));

    public void Apply(IEvent<StartdatumWerdGewijzigd> startdatumWerdGewijzigd, BeheerVerenigingHistoriekDocument document)
    {
        if (startdatumWerdGewijzigd.Data.Startdatum is { } startdatum)
        {
            var startDatumString = startdatum.ToString(WellknownFormats.DateOnly);
            AddHistoriekEntry(
                startdatumWerdGewijzigd,
                document,
                $"Startdatum werd gewijzigd naar '{startDatumString}'.",
                new StartdatumWerdGewijzigdData(startDatumString)
            );
        }
        else
            AddHistoriekEntry(
                startdatumWerdGewijzigd,
                document,
                $"Startdatum werd verwijderd.",
                new StartdatumWerdGewijzigdData(null)
            );

    }

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

    public void Apply(IEvent<ContactgegevenWerdToegevoegd> contactgegevenWerdToegevoegd, BeheerVerenigingHistoriekDocument document)
    {
        AddHistoriekEntry(
            contactgegevenWerdToegevoegd,
            document,
            $"{contactgegevenWerdToegevoegd.Data.Type} contactgegeven werd toegevoegd.",
            new ContactgegevenWerdToegevoegdData(
                contactgegevenWerdToegevoegd.Data.ContactgegevenId,
                contactgegevenWerdToegevoegd.Data.Type,
                contactgegevenWerdToegevoegd.Data.Waarde,
                contactgegevenWerdToegevoegd.Data.Omschrijving,
                contactgegevenWerdToegevoegd.Data.IsPrimair
            )
        );

        document.Metadata = new Metadata(contactgegevenWerdToegevoegd.Sequence, contactgegevenWerdToegevoegd.Version);
    }

    private static void AddHistoriekEntry(IEvent @event, BeheerVerenigingHistoriekDocument document, string beschrijving, IHistoriekData data)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                beschrijving,
                @event.Data.GetType().Name,
                data,
                initiator,
                tijdstip
            )).ToList();
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }

    private static void AddHistoriekEntry(IEvent @event, BeheerVerenigingHistoriekDocument document, string beschrijving)
    {
        var initiator = @event.GetHeaderString(MetadataHeaderNames.Initiator);
        var tijdstip = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip).ToBelgianDateAndTime();

        document.Gebeurtenissen = document.Gebeurtenissen.Append(
            new BeheerVerenigingHistoriekGebeurtenis(
                beschrijving,
                @event.Data.GetType().Name,
                @event.Data,
                initiator,
                tijdstip
            )).ToList();
        document.Metadata = new Metadata(@event.Sequence, @event.Version);
    }

    private static void AddToevoegingContactInfoHistoriekEntry(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo toevoeging)
    {
        AddHistoriekEntry(
            contactInfoLijstWerdGewijzigd,
            document,
            $"Contactinfo met naam '{toevoeging.Contactnaam}' werd toegevoegd.",
            new ContactInfoWerdToegevoegdData(toevoeging));
    }

    private static void AddWijzigContactInfoHistoriekEntries(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo wijziging)
    {
        if (wijziging.Email is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                document,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Email' werd gewijzigd naar '{wijziging.Email}'.",
                new EmailContactInfoWerdGewijzigdHistoriekData(wijziging.Contactnaam, wijziging.Email));
        if (wijziging.Telefoon is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                document,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Telefoon' werd gewijzigd naar '{wijziging.Telefoon}'.",
                new TelefoonContactInfoWerdGewijzigdHistoriekData(wijziging.Contactnaam, wijziging.Telefoon));
        if (wijziging.Website is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                document,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'Website' werd gewijzigd naar '{wijziging.Website}'.",
                new WebsiteContactInfoWerdGewijzigdHistoriekData(wijziging.Contactnaam, wijziging.Website));
        if (wijziging.SocialMedia is not null)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                document,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd gewijzigd, 'SocialMedia' werd gewijzigd naar '{wijziging.SocialMedia}'.",
                new SocialMediaContactInfoWerdGewijzigdHistoriekData(wijziging.Contactnaam, wijziging.SocialMedia));
        if (wijziging.PrimairContactInfo)
            AddHistoriekEntry(
                contactInfoLijstWerdGewijzigd,
                document,
                $"Contactinfo met naam '{wijziging.Contactnaam}' werd als primair aangeduid.",
                new PrimairContactInfoWerdGewijzigdHistoriekData(wijziging.Contactnaam, wijziging.PrimairContactInfo));
    }

    private static void AddVerwijderingContactInfoHistoriekEntry(IEvent contactInfoLijstWerdGewijzigd, BeheerVerenigingHistoriekDocument document, ContactInfo verwijdering)
    {
        AddHistoriekEntry(
            contactInfoLijstWerdGewijzigd,
            document,
            $"Contactinfo met naam '{verwijdering.Contactnaam}' werd verwijderd.",
            new ContactInfoWerdVerwijderdData(verwijdering.Contactnaam));
    }
}
