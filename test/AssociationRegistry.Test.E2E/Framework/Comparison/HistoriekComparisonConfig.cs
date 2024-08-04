namespace AssociationRegistry.Test.E2E.Framework.Comparison;

using AssociationRegistry.Admin.Api.Verenigingen.Historiek.ResponseModels;
using AssociationRegistry.Admin.Schema.Historiek.EventData;
using KellermanSoftware.CompareNetObjects;

public class HistoriekComparisonConfig: End2EndComparisonConfig
{
    public static readonly HistoriekComparisonConfig Instance = new();
    public HistoriekComparisonConfig()
    {
        // Ignore specific properties
        IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.Vertegenwoordigers);
        IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.HoofdactiviteitenVerenigingsloket);
        IgnoreProperty<HistoriekGebeurtenisResponse>(x => x.Tijdstip);

        CustomPropertyComparer<HistoriekGebeurtenisResponse>(
            x => x.Data, new HistoriekDataComparer(RootComparerFactory.GetRootComparer()));
    }
}
