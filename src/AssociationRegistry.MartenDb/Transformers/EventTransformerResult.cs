namespace AssociationRegistry.MartenDb.Transformers;

using AssociationRegistry.Events;
using AssociationRegistry.Persoonsgegevens;

public record EventTransformationResult(
    IEvent TransformedEvent,
    VertegenwoordigerPersoonsgegevens[] ExtractedPersoonsgegevens);
