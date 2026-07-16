namespace AssociationRegistry.MartenDb.Store;

using AssociationRegistry.EventStore;
using DecentraalBeheer.Vereniging;
using JasperFx.Events;
using Marten;

public class StreamArchivalService : IStreamArchivalService
{
    private const string ArchiveReason = "Gearchiveerd via maintenance endpoint";

    private readonly IDocumentSession _session;

    public StreamArchivalService(IDocumentSession session)
    {
        _session = session;
    }

    public async Task ArchiveStream(VCode vCode)
    {
        _session.Events.Append(vCode, new Archived(ArchiveReason));
        _session.Events.ArchiveStream(vCode);
        await _session.SaveChangesAsync();
    }
}
