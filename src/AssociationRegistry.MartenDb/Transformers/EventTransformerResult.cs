namespace AssociationRegistry.MartenDb.Transformers;

using Events;
using Persoonsgegevens;

public record EventTransformationResult(
    IEvent TransformedEvent,
    IPersoonsgegevens[] ExtractedPersoonsgegevens);
