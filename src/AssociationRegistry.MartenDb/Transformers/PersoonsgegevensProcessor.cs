namespace AssociationRegistry.MartenDb.Transformers;

using Events;
using JasperFx.Core;
using Microsoft.Extensions.Logging;
using Persoonsgegevens;
using Store;

public class PersoonsgegevensProcessor : IPersoonsgegevensProcessor
{
    private readonly Dictionary<Type, IPersoonsgegevensEventTransformer> _transformers;
    private readonly IVertegenwoordigerPersoonsgegevensRepository _vertegenwoordigerRepository;
    private readonly IBankrekeningnummerPersoonsgegevensRepository _bankrekeningRepository;
    private readonly ILogger<PersoonsgegevensProcessor> _logger;

    public PersoonsgegevensProcessor(
        PersoonsgegevensEventTransformers transformerses,
        IVertegenwoordigerPersoonsgegevensRepository vertegenwoordigerRepository,
        IBankrekeningnummerPersoonsgegevensRepository bankrekeningRepository,
        ILogger<PersoonsgegevensProcessor> logger)
    {
        _vertegenwoordigerRepository = vertegenwoordigerRepository;
        _bankrekeningRepository = bankrekeningRepository;
        _logger = logger;
        _transformers = transformerses.ToDictionary(t => t.EventType, t => t);
    }

    public async Task<IEvent[]> ProcessEvents(string aggregateId, IEvent[] events)
    {
        var processedEvents = new IEvent[events.Length];
        var extracted = new List<IPersoonsgegevens>();

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
                    extracted.AddMany(result.ExtractedPersoonsgegevens);
            }
            else
            {
                processedEvents[i] = events[i];
            }
        }

        if (extracted.Any())
        {
            foreach (var item in extracted)
            {
                switch (item)
                {
                    case VertegenwoordigerPersoonsgegevens v:
                        await _vertegenwoordigerRepository.Save(v);
                        break;

                    case BankrekeningnummerPersoonsgegevens b:
                        await _bankrekeningRepository.Save(b);
                        break;

                    default:
                        _logger.LogWarning("Unknown extracted persoonsgegevens type {Type}", item.GetType().FullName);
                        break;
                }
            }
        }

        return processedEvents;
    }
}
