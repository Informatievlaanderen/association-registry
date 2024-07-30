namespace AssociationRegistry.Test.E2E.When_Registreer_Vereniging.Beheer_Historiek;

using Admin.Api.Verenigingen.Historiek.ResponseModels;
using Admin.Schema.Historiek.EventData;
using Alba;
using KellermanSoftware.CompareNetObjects;
using Xunit;

[Collection(nameof(RegistreerVerenigingContext))]
public class Returns_Historiek : RegistreerVerenigingContext
{
    private readonly IAlbaHost theHost;

    public Returns_Historiek(AppFixture fixture) : base(fixture)
    {
        theHost = fixture.Host;
    }

    [Fact]
    public async Task JsonContentMatches()
    {
        var result = await theHost.GetAsJson<HistoriekResponse>(url: $"/v1/verenigingen/{ResultingVCode}/historiek");

        var comparisonConfig = new ComparisonConfig();
        comparisonConfig.MaxDifferences = 10;

        comparisonConfig.CustomPropertyComparer<HistoriekGebeurtenisResponse>(
            x => x.Data, new JObjectComparer(RootComparerFactory.GetRootComparer()));

        comparisonConfig.IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.Vertegenwoordigers);
        comparisonConfig.IgnoreProperty<FeitelijkeVerenigingWerdGeregistreerdData>(x => x.HoofdactiviteitenVerenigingsloket);
        comparisonConfig.IgnoreProperty<HistoriekGebeurtenisResponse>(x => x.Tijdstip);
        comparisonConfig.MaxMillisecondsDateDifference = (int)TimeSpan.FromSeconds(10).TotalMilliseconds;

        result!.Context.ShouldCompare("http://127.0.0.1:11003/v1/contexten/beheer/historiek-vereniging-context.json");
        result.VCode.ShouldCompare(ResultingVCode);
    }
}
