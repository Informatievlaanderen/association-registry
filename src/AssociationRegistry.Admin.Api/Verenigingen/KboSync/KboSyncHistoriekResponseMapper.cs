namespace AssociationRegistry.Admin.Api.Verenigingen.KboSync;

using Hosts.Configuration.ConfigurationBindings;
using ResponseModels;
using Schema.KboSync;
using System.Collections.Generic;
using System.Linq;

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
