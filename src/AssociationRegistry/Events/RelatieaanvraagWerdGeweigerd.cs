namespace AssociationRegistry.Events;

/// <summary>
///     The ontvanger (the other partij than the aanvrager) refused the relatieaanvraag. Nothing changes on the
///     verenigingen. The reden is empty when none was given.
/// </summary>
public record RelatieaanvraagWerdGeweigerd(string AanvraagId, DateOnly WeigeringsDatum, string Reden)
    : IRelatieaanvraagEvent;
