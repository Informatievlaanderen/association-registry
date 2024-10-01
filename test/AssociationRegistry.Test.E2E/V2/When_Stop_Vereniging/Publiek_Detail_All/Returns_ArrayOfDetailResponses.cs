namespace AssociationRegistry.Test.E2E.V2.When_Stop_Vereniging.Publiek_Detail_All;

using Admin.Api.Verenigingen.Stop.RequestModels;using Data;
using Framework.AlbaHost;
using FluentAssertions;
using Framework.ApiSetup;
using Framework.TestClasses;
using Public.Api.Verenigingen.Detail.ResponseModels;
using Xunit;

[Collection(FullBlownApiCollection.Name)]
public class Returns_ArrayOfDetailResponses : End2EndTest<StopVerenigingContext, StopVerenigingRequest, PubliekVerenigingDetailResponse[]>
{
    public override Func<IApiSetup, PubliekVerenigingDetailResponse[]> GetResponse =>
        setup => setup.PublicApiHost.GetPubliekDetailAll<PubliekVerenigingDetailResponse>();

    public Returns_ArrayOfDetailResponses(StopVerenigingContext context) : base(context)
    {
    }

    [Fact]
    public void WithVereniging()
        => Response.Single().Should().BeEquivalentTo(new TeVerwijderenVereniging
        {
            Vereniging = new TeVerwijderenVerenigingData()
            {
                VCode = Context.VCode,
                TeVerwijderen = true,
            },
        });
}
