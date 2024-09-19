namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Publiek_Detail_All;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Alba;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using KellermanSoftware.CompareNetObjects;
using NodaTime;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Public.ProjectionHost.Infrastructure.Extensions;
using Xunit;

[Collection(nameof(StopVerenigingCollection))]
public class Returns_ArrayOfDetailResponses(StopVerenigingContext<PublicApiSetup> context)
    : End2EndTest<StopVerenigingContext<PublicApiSetup>, StopVerenigingRequest, PubliekVerenigingDetailResponse[]>(
        context)
{
    protected override Func<IAlbaHost, PubliekVerenigingDetailResponse[]> GetResponse =>
        host => host.GetPubliekDetailAll(VCode);

    [Fact]
    public void With_Context()
    {
        Response.Single().Context.ShouldCompare("http://127.0.0.1:11004/v1/contexten/publiek/detail-vereniging-context.json");
    }

    [Fact]
    public void With_Metadata_DatumLaatsteAanpassing()
    {
        Response.Single().Metadata.DatumLaatsteAanpassing.ShouldCompare(Instant.FromDateTimeOffset(DateTimeOffset.Now).ToBelgianDate(),
                                                                        compareConfig: new ComparisonConfig
                                                                            { MaxMillisecondsDateDifference = 5000 });
    }

    [Fact]
    public void WithVereniging()
        => Response.Should().BeEmpty();
}
