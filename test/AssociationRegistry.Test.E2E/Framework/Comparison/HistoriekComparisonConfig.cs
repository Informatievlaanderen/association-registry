namespace AssociationRegistry.Test.E2E.Framework.Comparison;

using Admin.Api.DecentraalBeheer.Verenigingen.Historiek.ResponseModels;
using Admin.Schema.Historiek.EventData;
using KellermanSoftware.CompareNetObjects;

public class HistoriekComparisonConfig : End2EndComparisonConfig
{
    public static readonly HistoriekComparisonConfig Instance = new();

    public HistoriekComparisonConfig()
    {
        // Ignore specific properties
        IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.Vertegenwoordigers);
        IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.HoofdactiviteitenVerenigingsloket);
        IgnoreProperty<HistoriekGebeurtenisResponse>(x => x.Tijdstip);
        IgnoreProperty<HistoriekGebeurtenisResponse>(x => x.Initiator);

        CustomPropertyComparer<HistoriekGebeurtenisResponse>(
            customProperty: x => x.Data, new HistoriekDataComparer(RootComparerFactory.GetRootComparer()));
    }
}
