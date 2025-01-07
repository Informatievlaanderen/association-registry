namespace AssociationRegistry.Admin.Api.DecentraalBeheer.Verenigingen.Historiek;

using AssociationRegistry.Admin.Schema.Historiek;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using ResponseModels;

public class VerenigingHistoriekResponseMapper
{
    private readonly AppSettings _appSettings;

    public VerenigingHistoriekResponseMapper(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public HistoriekResponse Map(string vCode, BeheerVerenigingHistoriekDocument historiek)
        => new()
        {
            Context = $"{_appSettings.PublicApiBaseUrl}/v1/contexten/beheer/historiek-vereniging-context.json",
            VCode = vCode,
            Gebeurtenissen = historiek.Gebeurtenissen
                                      .Select(Map)
                                      .ToArray(),
        };

    private static HistoriekGebeurtenisResponse Map(BeheerVerenigingHistoriekGebeurtenis gebeurtenis)
        => new()
        {
            Beschrijving = gebeurtenis.Beschrijving,
            Gebeurtenis = gebeurtenis.Gebeurtenis,
            Data = gebeurtenis.Data,
            Initiator = gebeurtenis.Initiator,
            Tijdstip = gebeurtenis.Tijdstip,
        };
}
