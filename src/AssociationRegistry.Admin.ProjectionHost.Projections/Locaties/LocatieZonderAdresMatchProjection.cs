namespace AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;

using DecentraalBeheer.Vereniging;
using Events;
using Integrations.Grar.Clients;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten.Events.Aggregation;
using Microsoft.Extensions.Logging;
using Schema.Locaties;

public class LocatieZonderAdresMatchProjection : SingleStreamProjection<LocatieZonderAdresMatchDocument, string>
{
    public static readonly ShardName ShardName = new("beheer.postgres.locatiezonderadresmatch");

    public LocatieZonderAdresMatchProjection(ILogger<LocatieZonderAdresMatchProjection> logger)
    {
        Name = ShardName.Name;
        Options.BatchSize = 1;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.MaximumHopperSize = 1;
        Options.DeleteViewTypeOnTeardown<LocatieZonderAdresMatchDocument>();

        DeleteEvent<Archived>();
        DeleteEvent<VerenigingWerdVerwijderd>();
    }

    public LocatieZonderAdresMatchDocument Create(FeitelijkeVerenigingWerdGeregistreerd e) =>
        new() { VCode = e.VCode, LocatieIds = e.Locaties.Select(x => x.LocatieId).ToArray() };

    public LocatieZonderAdresMatchDocument Create(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd e) =>
        new() { VCode = e.VCode, LocatieIds = e.Locaties.Select(x => x.LocatieId).ToArray() };

    public LocatieZonderAdresMatchDocument Apply(LocatieWerdToegevoegd @event, LocatieZonderAdresMatchDocument doc)
    {
        if (@event.Locatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde)
            return doc;

        if (@event.Locatie.Adres is null)
            return doc;

        return doc with
        {
            LocatieIds = doc.LocatieIds.Append(@event.Locatie.LocatieId).ToArray(),
        };
    }

#pragma warning disable CS0618
    // Overgenomen locaties worden net als toegevoegde locaties nog gematcht met het adressenregister.
    public LocatieZonderAdresMatchDocument Apply(
        DubbeleVerenigingWerdSamengevoegdMetAuthentiekeVereniging @event,
        LocatieZonderAdresMatchDocument doc
    ) =>
        doc with
        {
            LocatieIds = doc
                .LocatieIds.Concat(
                    @event
                        .OvergenomenGegevens.Locaties.Select(x => x.NieuweWaarde)
                        .Where(x =>
                            x.Locatietype != Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde && x.Adres is not null
                        )
                        .Select(x => x.LocatieId)
                )
                .Distinct()
                .ToArray(),
        };
#pragma warning restore CS0618

    public LocatieZonderAdresMatchDocument Apply(LocatieWerdGewijzigd @event, LocatieZonderAdresMatchDocument doc)
    {
        if (@event.Locatie.Locatietype == Locatietype.MaatschappelijkeZetelVolgensKbo.Waarde)
            return doc;

        if (@event.Locatie.Adres is null)
            return doc;

        if (doc.LocatieIds.Contains(@event.Locatie.LocatieId))
            return doc;

        return doc with
        {
            LocatieIds = doc.LocatieIds.Append(@event.Locatie.LocatieId).ToArray(),
        };
    }

    public LocatieZonderAdresMatchDocument Apply(LocatieWerdVerwijderd e, LocatieZonderAdresMatchDocument doc) =>
        LocatieZonderAdresId(doc, e.Locatie.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        AdresWerdOvergenomenUitAdressenregister e,
        LocatieZonderAdresMatchDocument doc
    ) => LocatieZonderAdresId(doc, e.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        AdresWerdNietGevondenInAdressenregister e,
        LocatieZonderAdresMatchDocument doc
    ) => LocatieZonderAdresId(doc, e.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        AdresNietUniekInAdressenregister e,
        LocatieZonderAdresMatchDocument doc
    ) => LocatieZonderAdresId(doc, e.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        AdresWerdOntkoppeldVanAdressenregister e,
        LocatieZonderAdresMatchDocument doc
    ) => LocatieZonderAdresId(doc, e.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieDuplicaatWerdVerwijderdNaAdresMatch e,
        LocatieZonderAdresMatchDocument doc
    ) => LocatieZonderAdresId(doc, e.VerwijderdeLocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        AdresHeeftGeenVerschillenMetAdressenregister e,
        LocatieZonderAdresMatchDocument doc
    ) => LocatieZonderAdresId(doc, e.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        AdresKonNietOvergenomenWordenUitAdressenregister e,
        LocatieZonderAdresMatchDocument doc
    ) =>
        e.Reden switch
        {
            GrarClient.BadRequestSuccessStatusCodeMessage
            or AdresKonNietOvergenomenWordenUitAdressenregister.RedenLocatieWerdVerwijderd
            or AdresKonNietOvergenomenWordenUitAdressenregister.RedenAdresKonNietGevalideerdWordenBijAdressenregister =>
                LocatieZonderAdresId(doc, e.LocatieId),
            _ => doc,
        };

    private static LocatieZonderAdresMatchDocument LocatieZonderAdresId(
        LocatieZonderAdresMatchDocument doc,
        int locatieId
    )
    {
        return doc with { LocatieIds = doc.LocatieIds.Where(x => x != locatieId).ToArray() };
    }
}
