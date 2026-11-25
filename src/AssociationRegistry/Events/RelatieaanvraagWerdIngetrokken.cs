namespace AssociationRegistry.Events;

/// <summary>
///     The aanvrager withdrew its relatieaanvraag. Nothing changes on the verenigingen. The reden is empty when none was
///     given.
/// </summary>
public record RelatieaanvraagWerdIngetrokken(string AanvraagId, DateOnly IntrekkingsDatum, string Reden)
    : IRelatieaanvraagEvent;
