namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using Hosts.Configuration.ConfigurationBindings;
using ResponseModels;
using Schema.Historiek;

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
