namespace AssociationRegistry.Events;

/// <summary>The aanvrager withdrew its relatieaanvraag. Nothing changes on the verenigingen.</summary>
public record RelatieaanvraagWerdIngetrokken(string AanvraagId, DateOnly IntrekkingsDatum) : IRelatieaanvraagEvent;
