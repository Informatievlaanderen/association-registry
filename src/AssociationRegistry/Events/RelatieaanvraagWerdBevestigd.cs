namespace AssociationRegistry.Events;

/// <summary>
///     The ontvanger (the other partij than the aanvrager) confirmed the relatieaanvraag. Saved in the same transaction
///     as the relatie on the vereniging: a lidmaatschap, or the subtype subvereniging.
/// </summary>
public record RelatieaanvraagWerdBevestigd(string AanvraagId, DateOnly BevestigingsDatum) : IRelatieaanvraagEvent;
