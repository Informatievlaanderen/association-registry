namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using System.Linq;
using ResponseModels;
using Schema.Historiek;

public class VerenigingHistoriekResponseMapper
{
    public HistoriekResponse Map(string vCode, BeheerVerenigingHistoriekDocument historiek)
        => new()
        {
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
