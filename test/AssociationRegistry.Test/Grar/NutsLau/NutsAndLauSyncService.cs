﻿namespace AssociationRegistry.Test.Grar.NutsLau;

using Marten;
using Microsoft.Extensions.Logging;

public interface INutsAndLauSyncService
{
    Task SyncNutsLauInfo();
}

public class NutsAndLauSyncService : INutsAndLauSyncService
{
    private readonly INutsLauFromGrarFetcher _nutsLauFromGrarFetcher;
    private readonly IPostcodesFromGrarFetcher _postcodesFromGrarFetcher;
    private readonly IDocumentSession _session;
    private readonly ILogger<NutsAndLauSyncService> _logger;

    public NutsAndLauSyncService(INutsLauFromGrarFetcher nutsLauFromGrarFetcher, IPostcodesFromGrarFetcher postcodesFromGrarFetcher, IDocumentSession session, ILogger<NutsAndLauSyncService> logger)
    {
        _nutsLauFromGrarFetcher = nutsLauFromGrarFetcher;
        _postcodesFromGrarFetcher = postcodesFromGrarFetcher;
        _session = session;
        _logger = logger;
    }

    public async Task SyncNutsLauInfo()
    {
        _logger.LogInformation("Start syncing nuts lau info");
        var postcodes = await _postcodesFromGrarFetcher.FetchPostalCodes();
        _logger.LogInformation($"PostcodesFromGrarFetcher returned {postcodes.Length} postcodes.");
        var nutsLauInfo = await _nutsLauFromGrarFetcher.GetFlemishNutsAndLauByPostcode(postcodes);
        _logger.LogInformation($"NutsLauFromGrarFetcher returned {nutsLauInfo.Length} nuts lau info.");

        if (nutsLauInfo.Length == 0)
            return;

        _session.Store(nutsLauInfo);
        await _session.SaveChangesAsync();
        _logger.LogInformation("Stopped syncing nuts lau info.");
    }
}
