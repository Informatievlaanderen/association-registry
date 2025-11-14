namespace AssociationRegistry.Events.Enriched;

public record EnrichedVertegenwoordiger(
    int VertegenwoordigerId,
    bool IsPrimair,
    VertegenwoordigerPersoonsgegevens? VertegenwoordigerPersoonsgegevens);
