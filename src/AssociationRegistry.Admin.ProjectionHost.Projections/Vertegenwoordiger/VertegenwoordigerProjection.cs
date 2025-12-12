namespace AssociationRegistry.Admin.ProjectionHost.Projections.Vertegenwoordiger;

using Detail;
using Events;
using JasperFx.Events;
using Marten;
using Marten.Events.Aggregation;
using Schema.Vertegenwoordiger;

public class VertegenwoordigerProjection : SingleStreamProjection<VertegenwoordigersPerVCodeDocument, string>
{
    public VertegenwoordigerProjection()
    {
        DeleteEvent<IEvent<VerenigingWerdVerwijderd>>((x, y) => x.VCode == y.StreamKey);
    }

    public VertegenwoordigersPerVCodeDocument Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event)
    {
        return new VertegenwoordigersPerVCodeDocument()
        {
            VCode = @event.StreamKey,
            VertegenwoordigersData = @event.Data.Vertegenwoordigers.Select(v => new VertegenwoordigerData(v.VertegenwoordigerId, VertegenwoordigerKszStatus.Created))
                                           .ToArray(),
        };
    }
    public VertegenwoordigersPerVCodeDocument Create(IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> @event)
    {
        return new VertegenwoordigersPerVCodeDocument()
        {
            VCode = @event.StreamKey,
            VertegenwoordigersData = @event.Data.Vertegenwoordigers.Select(v => new VertegenwoordigerData(v.VertegenwoordigerId, VertegenwoordigerKszStatus.Created))
                                           .ToArray(),
        };
    }

    public void Apply (IEvent<VertegenwoordigerWerdToegevoegd> @event, VertegenwoordigersPerVCodeDocument document)
    {
        document.VertegenwoordigersData =
            document.VertegenwoordigersData.Append(new VertegenwoordigerData(@event.Data.VertegenwoordigerId, VertegenwoordigerKszStatus.Created)).ToArray();

    }

    public void Apply (IEvent<VertegenwoordigerWerdVerwijderd> @event, VertegenwoordigersPerVCodeDocument document)
    {
        document.VertegenwoordigersData =
            document.VertegenwoordigersData.Where(x => x.VertegenwoordigerId != @event.Data.VertegenwoordigerId).ToArray();

    }

    public void Apply (IEvent<KszSyncHeeftVertegenwoordigerBevestigd> @event, VertegenwoordigersPerVCodeDocument document)
    {
        document.VertegenwoordigersData =
            document.VertegenwoordigersData.UpdateSingle(
                identityFunc: (v => v.VertegenwoordigerId == @event.Data.VertegenwoordigerId),
                update: (v => v with
                {
                    Status = VertegenwoordigerKszStatus.Bevestigd,
                })).ToArray();
    }

    public void Apply (IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> @event, VertegenwoordigersPerVCodeDocument document)
    {
        document.VertegenwoordigersData =
            document.VertegenwoordigersData.UpdateSingle(
                identityFunc: (v => v.VertegenwoordigerId == @event.Data.VertegenwoordigerId),
                update: (v => v with
                {
                    Status = VertegenwoordigerKszStatus.Overleden,
                })).ToArray();
    }

    public void Apply (IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend> @event, VertegenwoordigersPerVCodeDocument document)
    {
        document.VertegenwoordigersData =
            document.VertegenwoordigersData.UpdateSingle(
                identityFunc: (v => v.VertegenwoordigerId == @event.Data.VertegenwoordigerId),
                update: (v => v with
                {
                    Status = VertegenwoordigerKszStatus.NietGekend,
                })).ToArray();
    }
}
