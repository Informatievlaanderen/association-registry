namespace AssociationRegistry.Public.ProjectionHost.Projections.Search;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten.Events;
using Schema.Search;
using Wolverine.Runtime.Routing;

public class MartenEventsConsumer : IMartenEventsConsumer
{
    private readonly IElasticRepository _elasticRepository;
    private readonly ElasticEventProjection _projection;

    public MartenEventsConsumer(IElasticRepository elasticRepository, ElasticEventProjection projection)
    {
        _elasticRepository = elasticRepository;
        _projection = projection;
    }

    public async Task ConsumeAsync(IReadOnlyList<StreamAction> streamActions)
    {
        foreach (var @event in streamActions.SelectMany(streamAction => streamAction.Events))
        {
            try
            {
                await TryCreateMethod(@event);
                await TryApplyMethod(@event);
            }
            catch (IndeterminateRoutesException)
            {
                //ignore
            }
        }
    }

    private async Task TryCreateMethod(IEvent @event)
    {
        var createMethod = typeof(ElasticEventProjection).GetMethod("Create", new[] { @event.GetType() });
        if (createMethod is not null && createMethod.ReturnType == typeof(VerenigingDocument))
        {
            await _elasticRepository.IndexAsync((createMethod.Invoke(_projection, new object?[] { @event }) as VerenigingDocument)!);
        }
    }

    private async Task TryApplyMethod(IEvent @event)
    {
        var applyMethod = typeof(ElasticEventProjection)
            .GetMethod("Apply", new[] { @event.GetType(), typeof(VerenigingDocument) });

        if (applyMethod is not null && applyMethod.ReturnType == typeof(void))
        {
            var document = new VerenigingDocument();
            applyMethod.Invoke(_projection, new object?[] { @event, document });
            await _elasticRepository.UpdateAsync(
                @event.StreamKey!,
                document
            );
        }
    }
}
