namespace AssociationRegistry.Admin.ProjectionHost.Projections.Sync;

using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using JasperFx.Events;
using JasperFx.Events.Projections;
using Marten;
using Marten.Events.Projections;

public partial class BeheerKboSyncHistoriekProjection : EventProjection
{
    public static readonly ShardName ShardName = new("beheer.postgres.kbo.synchistoriek");

    public BeheerKboSyncHistoriekProjection()
    {
        Name = ShardName.Name;
        Options.TeardownDataOnRebuild = true;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerKboSyncHistoriekGebeurtenisDocument>();
    }

    public void Project(
        IEvent<VerenigingWerdIngeschrevenOpWijzigingenUitKbo> geregistreerd,
        IDocumentOperations operations
    )
            {
                operations.Insert(
                    new BeheerKboSyncHistoriekGebeurtenisDocument(
                        geregistreerd.Sequence.ToString(),
                        geregistreerd.Data.KboNummer,
                        geregistreerd.StreamKey!,
                        Beschrijving: "Registreer inschrijving geslaagd",
                        geregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }

    public void Project(IEvent<SynchronisatieMetKboWasSuccesvol> geregistreerd, IDocumentOperations operations)
            {
                operations.Insert(
                    new BeheerKboSyncHistoriekGebeurtenisDocument(
                        geregistreerd.Sequence.ToString(),
                        geregistreerd.Data.KboNummer,
                        geregistreerd.StreamKey!,
                        Beschrijving: "Vereniging succesvol up to date gebracht met data uit de KBO",
                        geregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)
                    )
                );
            }
}
