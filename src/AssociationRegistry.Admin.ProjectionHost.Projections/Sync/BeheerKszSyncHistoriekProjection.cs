namespace AssociationRegistry.Admin.ProjectionHost.Projections.Sync;

using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;

public partial class BeheerKszSyncHistoriekProjection : EventProjection
{
    public static readonly ShardName ShardName = new("beheer.postgres.ksz.synchistoriek");

    public BeheerKszSyncHistoriekProjection()
    {
        Name = ShardName.Name;
        Options.TeardownDataOnRebuild = true;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerKszSyncHistoriekGebeurtenisDocument>();
    }

    public void Project(IEvent<KszSyncHeeftVertegenwoordigerBevestigd> @event, IDocumentOperations operations)
            {
                operations.Insert(
                    new BeheerKszSyncHistoriekGebeurtenisDocument(
                        @event.Sequence,
                        @event.StreamKey!,
                        @event.Data.VertegenwoordigerId,
                        Beschrijving: "Ksz sync heeft vertegenwoordiger bevestigd.",
                        @event.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }

    public void Project(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> @event,
        IDocumentOperations operations
    )
            {
                operations.Insert(
                    new BeheerKszSyncHistoriekGebeurtenisDocument(
                        @event.Sequence,
                        @event.StreamKey!,
                        @event.Data.VertegenwoordigerId,
                        Beschrijving: "Ksz sync heeft vertegenwoordiger aangeduid als overleden.",
                        @event.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }

    public void Project(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend> @event,
        IDocumentOperations operations
    )
            {
                operations.Insert(
                    new BeheerKszSyncHistoriekGebeurtenisDocument(
                        @event.Sequence,
                        @event.StreamKey!,
                        @event.Data.VertegenwoordigerId,
                        Beschrijving: "Ksz sync heeft vertegenwoordiger aangeduid als niet gekend.",
                        @event.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }
}
