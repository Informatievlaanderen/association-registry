namespace AssociationRegistry.Events.Enriched;

public record EnrichedVertegenwoordiger(
    Guid RefId,
    int VertegenwoordigerId,
    bool IsPrimair,
    VertegenwoordigerPersoonsgegevens? VertegenwoordigerPersoonsgegevens);
