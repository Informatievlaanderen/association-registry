namespace AssociationRegistry.Events;

public record RelatieaanvraagWerdIngediend(
    string AanvraagId,
    string KoepelVCode,
    string VerenigingVCode,
    string AangevraagdDoor,
    string RelatieType,
    string Beschrijving,
    DateOnly AanvraagDatum,
    DateOnly VervalDatum
) : IRelatieaanvraagEvent;
