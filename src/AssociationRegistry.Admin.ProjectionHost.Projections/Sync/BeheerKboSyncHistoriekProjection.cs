namespace AssociationRegistry.Admin.ProjectionHost.Projections.Sync;

using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.Events;
using AssociationRegistry.Framework;
using JasperFx.Events;
using Marten.Events.Projections;

public class BeheerKboSyncHistoriekProjection : EventProjection
{
    public BeheerKboSyncHistoriekProjection()
    {
        Options.TeardownDataOnRebuild = true;
        Options.EnableDocumentTrackingByIdentity = true;
        Options.DeleteViewTypeOnTeardown<BeheerKboSyncHistoriekGebeurtenisDocument>();

        Project<IEvent<VerenigingWerdIngeschrevenOpWijzigingenUitKbo>>((geregistreerd, operations) =>
        {
            operations.Insert(new BeheerKboSyncHistoriekGebeurtenisDocument(
                                  geregistreerd.Sequence.ToString(),
                                  geregistreerd.Data.KboNummer,
                                  geregistreerd.StreamKey!,
                                  Beschrijving: "Registreer inschrijving geslaagd",
                                  geregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)
                              ));
        });

        Project<IEvent<SynchronisatieMetKboWasSuccesvol>>((geregistreerd, operations) =>
        {
            operations.Insert(new BeheerKboSyncHistoriekGebeurtenisDocument(
                                  geregistreerd.Sequence.ToString(),
                                  geregistreerd.Data.KboNummer,
                                  geregistreerd.StreamKey!,
                                  Beschrijving: "Vereniging succesvol up to date gebracht met data uit de KBO",
                                  geregistreerd.GetHeaderString(MetadataHeaderNames.Tijdstip)
                              ));
        });
    }
}
