namespace AssociationRegistry.Admin.ProjectionHost.Projections.Vertegenwoordiger;

using Detail;
using Events;
using Framework;
using JasperFx.Events;
using Marten;
using Marten.Events.Aggregation;
using NodaTime;
using Schema.Vertegenwoordiger;

public class VertegenwoordigerProjection : SingleStreamProjection<VertegenwoordigersPerVCodeDocument, string>
{
    private Instant? _globalKszPivotPoint;
    private readonly GlobalKszPivotPointQuery _query;

    public VertegenwoordigerProjection(Func<IQuerySession> querySessionFactory)
    {
        _query = new GlobalKszPivotPointQuery(querySessionFactory);

        DeleteEvent<IEvent<VerenigingWerdVerwijderd>>((x, y) => x.VCode == y.StreamKey);
    }

    public async Task<VertegenwoordigersPerVCodeDocument> Create(IEvent<FeitelijkeVerenigingWerdGeregistreerd> @event)
    {
        var toegevoegdOp = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);
        var initialStatus = await DetermineInitialStatus(toegevoegdOp);

        return new VertegenwoordigersPerVCodeDocument()
        {
            VCode = @event.StreamKey,
            VertegenwoordigersData = @event
                .Data.Vertegenwoordigers.Select(v => new VertegenwoordigerData(v.VertegenwoordigerId, initialStatus))
                .ToArray(),
        };
    }

    public async Task<VertegenwoordigersPerVCodeDocument> Create(
        IEvent<VerenigingZonderEigenRechtspersoonlijkheidWerdGeregistreerd> @event
    )
    {
        var toegevoegdOp = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);
        var initialStatus = await DetermineInitialStatus(toegevoegdOp);

        return new VertegenwoordigersPerVCodeDocument()
        {
            VCode = @event.StreamKey,
            VertegenwoordigersData = @event
                .Data.Vertegenwoordigers.Select(v => new VertegenwoordigerData(v.VertegenwoordigerId, initialStatus))
                .ToArray(),
        };
    }

    public async Task Apply(IEvent<VertegenwoordigerWerdToegevoegd> @event, VertegenwoordigersPerVCodeDocument document)
    {
        var toegevoegdOp = @event.GetHeaderInstant(MetadataHeaderNames.Tijdstip);
        var initialStatus = await DetermineInitialStatus(toegevoegdOp);

        document.VertegenwoordigersData = document
            .VertegenwoordigersData.Append(new VertegenwoordigerData(@event.Data.VertegenwoordigerId, initialStatus))
            .ToArray();
    }

    public void Apply(IEvent<VertegenwoordigerWerdVerwijderd> @event, VertegenwoordigersPerVCodeDocument document)
    {
        document.VertegenwoordigersData = document
            .VertegenwoordigersData.Where(x => x.VertegenwoordigerId != @event.Data.VertegenwoordigerId)
            .ToArray();
    }

    public void Apply(
        IEvent<KszSyncHeeftVertegenwoordigerBevestigd> @event,
        VertegenwoordigersPerVCodeDocument document
    )
    {
        document.VertegenwoordigersData = document
            .VertegenwoordigersData.UpdateSingle(
                identityFunc: (v => v.VertegenwoordigerId == @event.Data.VertegenwoordigerId),
                update: (v => v with { Status = VertegenwoordigerKszStatus.Bevestigd })
            )
            .ToArray();
    }

    public void Apply(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsOverleden> @event,
        VertegenwoordigersPerVCodeDocument document
    )
    {
        document.VertegenwoordigersData = document
            .VertegenwoordigersData.UpdateSingle(
                identityFunc: (v => v.VertegenwoordigerId == @event.Data.VertegenwoordigerId),
                update: (v => v with { Status = VertegenwoordigerKszStatus.Overleden })
            )
            .ToArray();
    }

    public void Apply(
        IEvent<KszSyncHeeftVertegenwoordigerAangeduidAlsNietGekend> @event,
        VertegenwoordigersPerVCodeDocument document
    )
    {
        document.VertegenwoordigersData = document
            .VertegenwoordigersData.UpdateSingle(
                identityFunc: (v => v.VertegenwoordigerId == @event.Data.VertegenwoordigerId),
                update: (v => v with { Status = VertegenwoordigerKszStatus.NietGekend })
            )
            .ToArray();
    }

    private async Task<string> DetermineInitialStatus(Instant toegevoegdOp)
    {
        _globalKszPivotPoint ??= await _query.ExecuteAsync();

        // Geen KSZ pivot point → nog nooit KSZ verificatie gebruikt
        if (_globalKszPivotPoint == null)
            return VertegenwoordigerKszStatus.NogNietGesynced;

        // Toegevoegd ná of op pivot point → al geverifieerd via GeefPersoon
        if (toegevoegdOp >= _globalKszPivotPoint.Value)
            return VertegenwoordigerKszStatus.Bevestigd;

        // Toegevoegd vóór pivot point → moet nog gesynced worden
        return VertegenwoordigerKszStatus.NogNietGesynced;
    }
}
