namespace AssociationRegistry.Admin.ProjectionHost.Projections.Locaties;

using DecentraalBeheer.Vereniging;
using Events;
using Integrations.Grar.Clients;
using JasperFx.Events.Projections;
using Marten.Events.Aggregation;
using Microsoft.Extensions.Logging;
using Schema.Locaties;

public partial class LocatieZonderAdresMatchProjection : SingleStreamProjection<LocatieZonderAdresMatchDocument, string>
{
    public static readonly ShardName ShardName = new("beheer.postgres.locatiezonderadresmatch");

    public LocatieZonderAdresMatchProjection(ILogger<LocatieZonderAdresMatchProjection> logger)
    {
        Name = ShardName.Name;
        Options.BatchSize = 1;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.MaximumHopperSize = 1;
        Options.DeleteViewTypeOnTeardown<LocatieZonderAdresMatchDocument>();

        DeleteEvent<VerenigingWerdVerwijderd>();
            }

    public LocatieZonderAdresMatchDocument Create(FeitelijkeVerenigingWerdGeregistreerd @event) =>
        new() { VCode = @event.VCode, LocatieIds = @event.Locaties.Select(x => x.LocatieId).ToArray() };

    public LocatieZonderAdresMatchDocument Create(VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd @event) =>
        new() { VCode = @event.VCode, LocatieIds = @event.Locaties.Select(x => x.LocatieId).ToArray() };

    public LocatieZonderAdresMatchDocument Apply(LocatieZonderAdresMatchDocument doc, LocatieWerdToegevoegd @event)
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

    public LocatieZonderAdresMatchDocument Apply(LocatieZonderAdresMatchDocument doc, LocatieWerdGewijzigd @event)
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

    public LocatieZonderAdresMatchDocument Apply(LocatieZonderAdresMatchDocument doc, LocatieWerdVerwijderd @event) =>
        LocatieZonderAdresId(doc, @event.Locatie.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieZonderAdresMatchDocument doc,
        AdresWerdOvergenomenUitAdressenregister @event
    ) => LocatieZonderAdresId(doc, @event.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieZonderAdresMatchDocument doc,
        AdresWerdNietGevondenInAdressenregister @event
    ) => LocatieZonderAdresId(doc, @event.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieZonderAdresMatchDocument doc,
        AdresNietUniekInAdressenregister @event
    ) => LocatieZonderAdresId(doc, @event.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieZonderAdresMatchDocument doc,
        AdresWerdOntkoppeldVanAdressenregister @event
    ) => LocatieZonderAdresId(doc, @event.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieZonderAdresMatchDocument doc,
        LocatieDuplicaatWerdVerwijderdNaAdresMatch @event
    ) => LocatieZonderAdresId(doc, @event.VerwijderdeLocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieZonderAdresMatchDocument doc,
        AdresHeeftGeenVerschillenMetAdressenregister @event
    ) => LocatieZonderAdresId(doc, @event.LocatieId);

    public LocatieZonderAdresMatchDocument Apply(
        LocatieZonderAdresMatchDocument doc,
        AdresKonNietOvergenomenWordenUitAdressenregister @event
    ) =>
        @event.Reden switch
                {
                    GrarClient.BadRequestSuccessStatusCodeMessage
                    or AdresKonNietOvergenomenWordenUitAdressenregister.RedenLocatieWerdVerwijderd
                    or AdresKonNietOvergenomenWordenUitAdressenregister.RedenAdresKonNietGevalideerdWordenBijAdressenregister =>
                LocatieZonderAdresId(doc, @event.LocatieId),
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
