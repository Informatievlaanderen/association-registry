namespace AssociationRegistry.Admin.ProjectionHost.Projections.Sync;

using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using JasperFx.Events;
using Marten.Events.Projections;

public class BeheerKszSyncHistoriekProjection : EventProjection
{
    public BeheerKszSyncHistoriekProjection()
    {
        Options.TeardownDataOnRebuild = true;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerKszSyncHistoriekGebeurtenisDocument>();

        Project<IEvent<KszSyncHeeftVertegenwoordigerBevestigd>>(
            (@event, operations) =>
            {
                operations.Insert(
                    new BeheerKszSyncHistoriekGebeurtenisDocument(
                        @event.Sequence.ToString(),
                        @event.StreamKey!,
                        @event.Data.VertegenwoordigerId,
                        Beschrijving: "Ksz sync heeft vertegenwoordiger bevestigd.",
                        @event.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }
        );

        Project<IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden>>(
            (@event, operations) =>
            {
                operations.Insert(
                    new BeheerKszSyncHistoriekGebeurtenisDocument(
                        @event.Sequence.ToString(),
                        @event.StreamKey!,
                        @event.Data.VertegenwoordigerId,
                        Beschrijving: "Ksz sync heeft vertegenwoordiger aangeduid als overleden.",
                        @event.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }
        );

        Project<IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend>>(
            (@event, operations) =>
            {
                operations.Insert(
                    new BeheerKszSyncHistoriekGebeurtenisDocument(
                        @event.Sequence.ToString(),
                        @event.StreamKey!,
                        @event.Data.VertegenwoordigerId,
                        Beschrijving: "Ksz sync heeft vertegenwoordiger aangeduid als niet gekend.",
                        @event.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }
        );
    }
}
