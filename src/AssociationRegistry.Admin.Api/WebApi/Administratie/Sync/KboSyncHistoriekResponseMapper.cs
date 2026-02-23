namespace AssociationRegistry.Admin.Api.WebApi.Administratie.Sync;

using AssociationRegistry.Admin.Schema.KboSync;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using ResponseModels;

public class KboSyncHistoriekResponseMapper
{
    private readonly AppSettings _appSettings;

    public KboSyncHistoriekResponseMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public KboSyncHistoriekResponse Map(IEnumerable<BeheerKboSyncHistoriekGebeurtenisDocument> gebeurtenissen)
        => new(gebeurtenissen.Select(Map).ToArray());

    private static KboSyncHistoriekGebeurtenisResponse Map(BeheerKboSyncHistoriekGebeurtenisDocument gebeurtenisDocument)
        => new()
        {
            Kbonummer = gebeurtenisDocument.Kbonummer,
            VCode = gebeurtenisDocument.VCode,
            Beschrijving = gebeurtenisDocument.Beschrijving,
            Tijdstip = gebeurtenisDocument.Tijdstip,
        };
}
