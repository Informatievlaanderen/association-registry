namespace AssociationRegistry.Events;

/// <summary>
///     The ontvanger (the other partij than the aanvrager) refused the relatieaanvraag. Nothing changes on the
///     verenigingen.
/// </summary>
public record RelatieaanvraagWerdGeweigerd(string AanvraagId, DateOnly WeigeringsDatum) : IRelatieaanvraagEvent;
