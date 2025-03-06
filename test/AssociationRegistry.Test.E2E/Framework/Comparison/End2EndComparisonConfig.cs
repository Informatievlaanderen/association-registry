namespace AssociationRegistry.Test.E2E.Framework.Comparison;

using KellermanSoftware.CompareNetObjects;
using System;

public class End2EndComparisonConfig : ComparisonConfig
{
    public End2EndComparisonConfig()
    {
        MaxDifferences = 10;
        MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;
    }
}
