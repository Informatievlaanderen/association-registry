namespace AssociationRegistry.DecentraalBeheer.Relatieaanvragen;

using EventStore;

public record RelatieaanvraagCommandResult(RelatieaanvraagId AanvraagId, long? Sequence, long? Version)
{
    public static RelatieaanvraagCommandResult Create(RelatieaanvraagId aanvraagId, StreamActionResult result) =>
        new(aanvraagId, result.Sequence, result.Version);
}
