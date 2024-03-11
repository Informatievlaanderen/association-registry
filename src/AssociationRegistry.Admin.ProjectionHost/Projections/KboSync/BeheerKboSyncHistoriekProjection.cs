namespace AssociationRegistry.Admin.ProjectionHost.Projections.KboSync;

using Events;
using Framework;
using Marten.Events;
using Marten.Events.Projections;
using Schema.KboSync;

public class BeheerKboSyncHistoriekProjection : EventProjection
{
    public BeheerKboSyncHistoriekProjection()
    {
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
    }
}
