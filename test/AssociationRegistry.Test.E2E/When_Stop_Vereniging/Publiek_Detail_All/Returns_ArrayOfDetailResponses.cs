namespace AssociationRegistry.Test.E2E.When_Stop_Vereniging.Publiek_Detail_All;

using Admin.Api.Verenigingen.Stop.RequestModels;
using Alba;
using FluentAssertions;
using Framework.AlbaHost;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(nameof(PubliekStopVerenigingCollection))]
public class Returns_ArrayOfDetailResponses(StopVerenigingContext<PublicApiSetup> context)
    : End2EndTest<StopVerenigingContext<PublicApiSetup>, StopVerenigingRequest, PubliekVerenigingDetailResponse[]>(
        context)
{
    protected override Func<IAlbaHost, PubliekVerenigingDetailResponse[]> GetResponse =>
        host => host.GetPubliekDetailAll(VCode);

    [Fact]
    public void WithVereniging()
        => Response.Should().BeEmpty();
}
