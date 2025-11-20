namespace AssociationRegistry.MartenDb.Transformers;

using Events;
using JasperFx.Core;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;
using Store;

public class PersoonsgegevensProcessor : IPersoonsgegevensProcessor
{
    private readonly Dictionary<Type, IPersoonsgegevensEventTransformer> _transformers;
    private readonly IVertegenwoordigerPersoonsgegevensRepository _repository;
    private readonly ILogger<PersoonsgegevensProcessor> _logger;

    public PersoonsgegevensProcessor(
        PersoonsgegevensEventTransformers transformerses,
        IVertegenwoordigerPersoonsgegevensRepository repository,
        ILogger<PersoonsgegevensProcessor> logger)
    {
        _repository = repository;
        _logger = logger;

        _transformers = transformerses.ToDictionary(t => t.EventType, t => t);

        _logger.LogInformation("Initialized PersoonsgegevensProcessor with {Count} transformers",
            _transformers.Count);
    }

    public async Task<IEvent[]> ProcessEvents(string aggregateId, IEvent[] events)
    {
        var processedEvents = new IEvent[events.Length];
        var extractedPersoonsgegevens = new List<VertegenwoordigerPersoonsgegevens>();

        for (int i = 0; i < events.Length; i++)
        {
            var eventType = events[i].GetType();

            if (_transformers.TryGetValue(eventType, out var transformer))
            {
                _logger.LogDebug("Transforming event {EventType} for VCode {VCode}",
                    eventType.Name, aggregateId);

                var result = transformer.Transform(events[i], aggregateId);
                processedEvents[i] = result.TransformedEvent;

                if (result.ExtractedPersoonsgegevens.Length != 0)
                {
                    extractedPersoonsgegevens.AddMany(result.ExtractedPersoonsgegevens);
                }
            }
            else
            {
                processedEvents[i] = events[i];
            }
        }

        if (extractedPersoonsgegevens.Any())
        {
            foreach (var persoonsgegevens in extractedPersoonsgegevens)
            {
                await _repository.Save(persoonsgegevens);
                _logger.LogInformation("Saved persoonsgegevens with RefId {RefId} for VCode {VCode}",
                    persoonsgegevens.RefId, aggregateId);
            }
        }

        return processedEvents;
    }
}
